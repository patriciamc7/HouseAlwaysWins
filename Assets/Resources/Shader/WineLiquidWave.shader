Shader "Custom/URP/WineLiquidWave"
{
    Properties
    {
        // === Colores del Vino ===
        _WineColorDeep    ("Wine Color (Deep)",    Color)  = (0.35, 0.02, 0.08, 1.0)
        _WineColorMid     ("Wine Color (Mid)",     Color)  = (0.55, 0.05, 0.15, 1.0)
        _WineColorLight   ("Wine Color (Light)",   Color)  = (0.75, 0.12, 0.25, 1.0)

        // === Onda de Click ===
        // _ClickPosLocal: posición XZ en espacio LOCAL del objeto (no UV).
        // El C# lo pasa directamente desde hit.point transformado a local space.
        _ClickPosLocal    ("Click Pos (Local XZ)", Vector) = (0, 0, 0, 0)
        _WaveTime         ("Wave Time",            Float)  = -999.0
        _WaveSpeed        ("Wave Speed",           Float)  = 0.5
        _WaveFrequency    ("Wave Frequency",       Float)  = 22.0
        _WaveAmplitude    ("Wave Amplitude",       Float)  = 0.06
        _WaveDuration     ("Wave Duration (s)",    Float)  = 2.0
        _WaveFalloff      ("Wave Falloff",         Float)  = 2.5
        _WaveBrightness   ("Wave Brightness",      Float)  = 2.0

        // === Movimiento Líquido Idle ===
        _IdleStrength     ("Idle Liquid Strength", Float)  = 0.012
        _IdleSpeed        ("Idle Liquid Speed",    Float)  = 0.6

        // === Reflejo / Brillo ===
        _ReflectionColor  ("Reflection Color",     Color)  = (0.95, 0.7, 0.75, 1.0)
        _ReflectionPower  ("Reflection Power",     Float)  = 6.0
        _ReflectionStrength("Reflection Strength", Float)  = 0.35

        // === Textura (opcional) ===
        _MainTex          ("Surface Texture (opt)", 2D)    = "white" {}

        // === Borde / Menisco ===
        _MeniscusWidth    ("Meniscus Width",       Float)  = 0.06
        _MeniscusColor    ("Meniscus Color",       Color)  = (0.9, 0.6, 0.65, 1.0)
    }

    SubShader
    {
        Tags
        {
            "RenderType"     = "Transparent"
            "Queue"          = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "WineLiquidWave"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex   vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _WineColorDeep;
                float4 _WineColorMid;
                float4 _WineColorLight;

                float3 _ClickPosLocal;  // XYZ en local space (usamos XZ para distancia)
                float  _WaveTime;
                float  _WaveSpeed;
                float  _WaveFrequency;
                float  _WaveAmplitude;
                float  _WaveDuration;
                float  _WaveFalloff;
                float  _WaveBrightness;

                float  _IdleStrength;
                float  _IdleSpeed;

                float4 _ReflectionColor;
                float  _ReflectionPower;
                float  _ReflectionStrength;

                float  _MeniscusWidth;
                float4 _MeniscusColor;

                float4 _MainTex_ST;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            struct Attributes
            {
                float4 positionOS : POSITION;   // posición en object/local space
                float2 uv         : TEXCOORD0;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
                float3 normalWS   : TEXCOORD1;
                float3 viewDirWS  : TEXCOORD2;
                // Posición LOCAL del vértice — interpolada por el rasterizador
                // dando posición local exacta por fragmento. Perfecta para distancias.
                float3 posLocal   : TEXCOORD3;
            };

            // ── Helpers noise ──────────────────────────────────────────────

            float2 hash2(float2 p)
            {
                p = float2(dot(p, float2(127.1, 311.7)),
                           dot(p, float2(269.5, 183.3)));
                return -1.0 + 2.0 * frac(sin(p) * 43758.5453123);
            }

            float smoothNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float2 u = f * f * (3.0 - 2.0 * f);
                float a = dot(hash2(i + float2(0,0)), f - float2(0,0));
                float b = dot(hash2(i + float2(1,0)), f - float2(1,0));
                float c = dot(hash2(i + float2(0,1)), f - float2(0,1));
                float d = dot(hash2(i + float2(1,1)), f - float2(1,1));
                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            float fbm(float2 p)
            {
                float v = 0.0, a = 0.5;
                float2 shift = float2(100, 100);
                float2x2 rot = float2x2(cos(0.5), sin(0.5), -sin(0.5), cos(0.5));
                for (int i = 0; i < 4; i++)
                {
                    v += a * smoothNoise(p);
                    p  = mul(rot, p) * 2.0 + shift;
                    a *= 0.5;
                }
                return v;
            }

            // ── Vertex ─────────────────────────────────────────────────────
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.uv        = IN.uv;
                OUT.posLocal  = IN.positionOS.xyz;   // <-- guardamos posición local
                OUT.normalWS  = TransformObjectToWorldNormal(IN.normalOS);
                OUT.viewDirWS = normalize(GetWorldSpaceViewDir(
                                    TransformObjectToWorld(IN.positionOS.xyz)));
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            // ── Fragment ───────────────────────────────────────────────────
            float4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;
                float  t  = _Time.y;

                // ── 1. Idle FBM (en UV para el color) ─────────────────────
                float2 idleOff;
                idleOff.x = fbm(uv * 3.5 + float2(t * _IdleSpeed * 0.7,
                                                   t * _IdleSpeed * 0.4));
                idleOff.y = fbm(uv * 3.5 + float2(t * _IdleSpeed * 0.5,
                                                   t * _IdleSpeed * 0.8) + 5.2);
                float2 uvD = uv + idleOff * _IdleStrength;

                // ── 2. ONDA — distancia en espacio LOCAL 3D ────────────────
                // Usamos XZ del plano local (Y es la normal del círculo en Blender).
                // La distancia en local space es uniforme → onda perfectamente circular.
                float elapsed = t - _WaveTime;

                float waveColorMask = 0.0;
                float waveUVDisp    = 0.0;

                [branch]
                if (elapsed >= 0.0 && elapsed < _WaveDuration)
                {
                    // Distancia 2D en plano local XZ
                    float2 fragXZ  = IN.posLocal.xz;
                    float2 clickXZ = _ClickPosLocal.xz;
                    float  dist    = distance(fragXZ, clickXZ);

                    // Frente de onda circular que se expande
                    float waveFront = elapsed * _WaveSpeed;
                    float dToFront  = dist - waveFront;

                    // Banda visible alrededor del frente
                    float proximity = 1.0 - saturate(abs(dToFront) / 0.10);

                    // Envolvente temporal
                    float tNorm = saturate(elapsed / _WaveDuration);
                    float env   = sin(tNorm * 3.14159) * (1.0 - tNorm * 0.5);

                    // Falloff radial
                    float radFalloff = exp(-dist * _WaveFalloff);

                    // Oscilación senoidal
                    float osc = sin(dToFront * _WaveFrequency
                                   - elapsed * _WaveFrequency * _WaveSpeed);

                    float waveMask = osc * proximity * env * radFalloff;

                    waveColorMask = waveMask;
                    waveUVDisp    = waveMask * _WaveAmplitude;
                }

                float2 uvFinal = uvD + float2(waveUVDisp, waveUVDisp * 0.6);

                // ── 3. Color base del vino ─────────────────────────────────
                float noiseVal = fbm(uvFinal * 4.0 + t * 0.15) * 0.5 + 0.5;
                // Para el círculo usamos distancia desde centro en local space
                float distCenter = length(IN.posLocal.xz);
                float radial     = 1.0 - saturate(distCenter * 2.2);

                float3 wineColor = lerp(_WineColorDeep.rgb, _WineColorMid.rgb, noiseVal);
                       wineColor = lerp(wineColor, _WineColorLight.rgb, radial * 0.4);

                // ── 4. Aplicar onda al color ───────────────────────────────
                float wavePos = saturate( waveColorMask);
                float waveNeg = saturate(-waveColorMask);

                wineColor = lerp(wineColor, _WineColorLight.rgb, wavePos * 0.7);
                wineColor *= 1.0 + wavePos * (_WaveBrightness - 1.0);
                wineColor = lerp(wineColor, _WineColorDeep.rgb, waveNeg * 0.5);

                // ── 5. Fresnel ─────────────────────────────────────────────
                float3 N      = normalize(IN.normalWS);
                float3 V      = normalize(IN.viewDirWS);
                float  NdotV  = saturate(dot(N, V));
                float  fresnel= pow(1.0 - NdotV, _ReflectionPower);
                wineColor    += _ReflectionColor.rgb * fresnel * _ReflectionStrength;

                // ── 6. Menisco ─────────────────────────────────────────────
                // Para un círculo el menisco es el borde exterior en local space
                float edgeDist = saturate(1.0 - distCenter * 2.1);
                float meniscus = smoothstep(0.0, _MeniscusWidth, edgeDist);
                wineColor = lerp(_MeniscusColor.rgb, wineColor, meniscus);

                // ── 7. Textura opcional ────────────────────────────────────
                float4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvFinal);
                wineColor *= tex.rgb;

                // ── 8. Alpha ───────────────────────────────────────────────
                float alpha = saturate(0.88 + fresnel * 0.12 + wavePos * 0.1);

                return float4(saturate(wineColor), alpha);
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Lit"
}
