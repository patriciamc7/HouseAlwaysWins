Shader "UI/BorderGlowURP"
{
    Properties
    {
        _GlowColor("Glow Color", Color) = (1,1,1,1)
        _GlowSize("Glow Size", Range(0,0.5)) = 0.2
        _GlowSoftness("Glow Softness", Range(0,0.5)) = 0.05
        _CornerRadius("Corner Radius", Range(0,0.5)) = 0.25
        _Hover("Hover", Range(0,1)) = 0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" "RenderPipeline"="UniversalPipeline" }

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

            float4 _GlowColor;
            float _GlowSize;
            float _GlowSoftness;
            float _CornerRadius;
            float _Hover;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                o.uv = v.uv;
                return o;
            }

            float sdfRoundedRect(float2 uv, float radius)
            {
                float2 d = abs(uv - 0.5) - (0.5 - radius);
                return length(max(d,0.0)) + min(max(d.x,d.y),0.0);
            }

            float4 frag(v2f i) : SV_Target
            {
                float dist = sdfRoundedRect(i.uv, _CornerRadius * 0.5);

                // Solo glow por fuera
                float glow = smoothstep(_GlowSize + _GlowSoftness, _GlowSize, dist);

                return _GlowColor * glow * _Hover;
            }

            ENDHLSL
        }
    }
}