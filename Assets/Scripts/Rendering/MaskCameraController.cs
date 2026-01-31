using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

public class MaskCameraController : MonoBehaviour
{
    public Camera maskCamera;
    public MasksProvider masksProvider;

    [Space]
    public RenderTexture maskCameraRT;
    public RTHandle maskRTHandle;

    private int _cameraWidth;
    private int _cameraHeight;

    private int _lastRenderedVersion;

    private void Awake()
    {
        maskCamera.enabled = false;
    }

    private void Update()
    {
        InitTexture();

        if (_lastRenderedVersion == masksProvider.Version)
        {
            return;
        }

        RenderCamera();
        _lastRenderedVersion = masksProvider.Version;
    }

    private void RenderCamera()
    {
        maskCamera.Render();

        Graphics.CopyTexture(maskCameraRT, 0, 0, maskRTHandle.rt, 0, 0);
    }

    private void InitTexture()
    {
        if (_cameraWidth == maskCamera.pixelWidth && _cameraHeight == maskCamera.pixelHeight)
        {
            return;
        }

        if (maskCameraRT != null)
        {
            maskRTHandle.Release();
            maskCameraRT.Release();
        }

        _cameraWidth = maskCamera.pixelWidth;
        _cameraHeight = maskCamera.pixelHeight;

        var maskCameraDescriptor = new RenderTextureDescriptor(_cameraWidth, _cameraHeight)
        {
            graphicsFormat = GraphicsFormat.R8G8B8A8_UNorm,
            depthBufferBits = 24,
            msaaSamples = 1,
            sRGB = QualitySettings.activeColorSpace == ColorSpace.Linear,
        };
        maskCameraRT = new RenderTexture(maskCameraDescriptor)
        {
            name = "_MasksCameraTexture", filterMode = FilterMode.Bilinear, wrapMode = TextureWrapMode.Clamp,
        };
        maskCameraRT.Create();
        maskCamera.targetTexture = maskCameraRT;

        maskRTHandle = RTHandles.Alloc(
            _cameraWidth,
            _cameraHeight,
            depthBufferBits: 0,
            colorFormat: GraphicsFormat.R8G8B8A8_UNorm,
            filterMode: FilterMode.Bilinear,
            wrapMode: TextureWrapMode.Clamp,
            name: "_MasksTexture"
        );
    }
}
