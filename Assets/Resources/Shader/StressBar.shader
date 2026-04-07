Shader "Custom/StressBar"
{
    Properties
    {
        _Value("Value (0-100)", Range(0,100)) = 100

        _Mid1("End of Green (0-1)", Range(0,1)) = 0.45
        _Mid2("End of Yellow (0-1)", Range(0,1)) = 0.70

        _ColorLow ("Low Color", Color) = (0,1,0,1)
        _ColorMid ("Mid Color", Color) = (1,1,0,1)
        _ColorHigh("High Color", Color) = (1,0,0,1)
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float _Value;
            float _Mid1;
            float _Mid2;

            float4 _ColorLow;
            float4 _ColorMid;
            float4 _ColorHigh;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float v = saturate(_Value / 100.0);

                if (i.uv.x > v)
                    return float4(0,0,0,0);

                float t = i.uv.x;

                float4 col;

                // Verde → Amarillo
                if (t < _Mid1)
                {
                    float f = saturate(t / _Mid1);
                    col = lerp(_ColorLow, _ColorMid, f);
                }
                // Amarillo → Rojo
                else if (t < _Mid2)
                {
                    float f = saturate((t - _Mid1) / (_Mid2 - _Mid1));
                    col = lerp(_ColorMid, _ColorHigh, f);
                }
                // Rojo final
                else
                {
                    col = _ColorHigh;
                }

                return col;
            }
            ENDCG
        }
    }
}