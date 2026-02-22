
Shader "Custom/VertexSnapping"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _GridSize ("Grid Size (px)", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 200

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float _GridSize;
                float2 _ViewportResolution;
            CBUFFER_END

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;

                // Object space to clip space
                float4 clipPos = TransformObjectToHClip(v.vertex.xyz);

                // NDC (Normalized Device Coordinates) = clip.xy / clip.w
                float2 ndc = clipPos.xy / clipPos.w;

                // Convert NDC (-1..1) -> screen UV (0..1)
                float2 screenUV = (ndc + 1.0) * 0.5;

                // If GridSize is <= 0, avoid divide-by-zero
                float gridPx = max(1e-6, _GridSize);

                // Compute how many grid cells across the screen:
                // Multiply by resolution (pixels) and divide by grid size (px per cell)
                float2 cells = screenUV * (_ViewportResolution / gridPx);

                // Snap to integer cell boundaries
                cells = floor(cells + 0.5); // +0.5 to round; use floor(cells) to always bias down

                // Convert back to screenUV in 0..1
                screenUV = (cells * gridPx) / _ViewportResolution;

                // Convert screenUV back to NDC (-1..1)
                ndc = screenUV * 2.0 - 1.0;

                // Put into clip space again using the original clip.w
                clipPos.xy = ndc * clipPos.w;

                o.pos = clipPos;

                // pass UVs & normals as usual
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normalWS = TransformObjectToWorldNormal(v.normal);

                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv) * _Color;
                return col;
            }
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Lit"
}