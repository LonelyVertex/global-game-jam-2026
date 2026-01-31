using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

public class CompositionRendererFeature : ScriptableRendererFeature
{
    [Serializable]
    public class CompositionRendererFeatureSettings
    {
        public Material maskMaterial;
    }

    [SerializeField] private CompositionRendererFeatureSettings settings;

    private CompositionRendererFeaturePass _scriptablePass;

    public override void Create()
    {
        _scriptablePass = new CompositionRendererFeaturePass(settings) {
            renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing
        };

        _scriptablePass.ConfigureInput(ScriptableRenderPassInput.Color);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        var cam = renderingData.cameraData.camera;
        var masksProvider = cam.GetComponent<MaskTextureProvider>();
        _scriptablePass.Setup(masksProvider);

        if (masksProvider == null || masksProvider.MaskRTHandle == null)
        {
            return;
        }

        renderer.EnqueuePass(_scriptablePass);
    }

    class CompositionRendererFeaturePass : ScriptableRenderPass
    {
        private const string _kCompositePassName = "Composition Render Pass";
        private const string _kFinalPassName = "Composition Final Render Pass";

        private readonly CompositionRendererFeatureSettings _settings;
        private MaskTextureProvider _maskTextureProvider;

        private class CompositePassData
        {
            public TextureHandle colorHandle;
            public TextureHandle maskCameraHandle;
            public Material maskMaterial;
        }

        private class FinalPassData
        {
            public TextureHandle src;
            public Material maskMaterial;
        }

        public CompositionRendererFeaturePass(CompositionRendererFeatureSettings settings)
        {
            _settings = settings;
        }

        public void Setup(MaskTextureProvider maskTextureProvider)
        {
            _maskTextureProvider = maskTextureProvider;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (_maskTextureProvider == null)
            {
                return;
            }

            var resourceData = frameData.Get<UniversalResourceData>();
            var cameraData = frameData.Get<UniversalCameraData>();

            var desc = cameraData.cameraTargetDescriptor;
            var tempDesc = new TextureDesc(desc.width, desc.height)
            {
                name = "_CompositeTemp",
                colorFormat = desc.graphicsFormat,
                clearBuffer = false,
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp,
                msaaSamples = MSAASamples.None
            };

            var temp = renderGraph.CreateTexture(tempDesc);
            var camera = resourceData.activeColorTexture;
            var mask   = renderGraph.ImportTexture(_maskTextureProvider.MaskRTHandle);

            using (var builder = renderGraph.AddRasterRenderPass<CompositePassData>(_kCompositePassName, out var passData))
            {
                builder.AllowGlobalStateModification(true);

                passData.colorHandle = camera;
                passData.maskCameraHandle = mask;
                passData.maskMaterial = _settings.maskMaterial;

                builder.UseTexture(passData.colorHandle, AccessFlags.Read);
                builder.UseTexture(passData.maskCameraHandle, AccessFlags.Read);
                builder.SetRenderAttachment(temp, 0);
                builder.SetRenderFunc<CompositePassData>(ExecuteCompositePass);
            }

            using (var builder = renderGraph.AddRasterRenderPass<FinalPassData>(_kFinalPassName, out var passData))
            {
                passData.src = temp;
                passData.maskMaterial = _settings.maskMaterial;

                builder.UseTexture(temp);
                builder.SetRenderAttachment(resourceData.activeColorTexture, 0);
                builder.SetRenderFunc<FinalPassData>(ExecuteFinalPass);
            }
        }

        private static void ExecuteCompositePass(CompositePassData data, RasterGraphContext ctx)
        {
            ctx.cmd.SetGlobalTexture("_MasksTexture", data.maskCameraHandle);

            Blitter.BlitTexture(ctx.cmd, data.colorHandle, Vector4.one, data.maskMaterial, 0);
        }

        private static void ExecuteFinalPass(FinalPassData data, RasterGraphContext ctx)
        {
            Blitter.BlitTexture(ctx.cmd, data.src, Vector4.one, data.maskMaterial, 1);
        }
    }
}
