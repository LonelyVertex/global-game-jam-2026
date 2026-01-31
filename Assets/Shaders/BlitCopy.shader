Shader "Hidden/Custom/BlitCopy"
{
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
        }

        ZWrite Off
        ZTest Always
        Cull Off

        Pass
        {
            Name "CompositeMask"
            Blend Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D_X(_BlitTexture);
            SAMPLER(sampler_BlitTexture);

            TEXTURE2D_X(_MasksTexture);
            SAMPLER(sampler_MasksTexture);

            struct Attributes
            {
                uint vertexID : SV_VertexID;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert (Attributes v)
            {
                Varyings o;
                o.positionCS = GetFullScreenTriangleVertexPosition(v.vertexID);
                o.uv = GetFullScreenTriangleTexCoord(v.vertexID);
                return o;
            }

            half4 Frag (Varyings i) : SV_Target
            {
                half4 cam = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, i.uv);
                half4 mask = SAMPLE_TEXTURE2D_X(_MasksTexture, sampler_MasksTexture, i.uv);

                return lerp(cam, 0.0f, mask.a);
            }
            ENDHLSL
        }

        Pass
        {
            Name "FinalMask"
            Blend Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D_X(_BlitTexture);
            SAMPLER(sampler_BlitTexture);

            struct Attributes
            {
                uint vertexID : SV_VertexID;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings Vert (Attributes v)
            {
                Varyings o;
                o.positionCS = GetFullScreenTriangleVertexPosition(v.vertexID);
                o.uv = GetFullScreenTriangleTexCoord(v.vertexID);
                return o;
            }

            half4 Frag (Varyings i) : SV_Target
            {
                return SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, i.uv);
            }
            ENDHLSL
        }
    }
}
