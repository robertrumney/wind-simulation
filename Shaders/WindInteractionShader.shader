Shader "Custom/WindInteractionAdvanced"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _WindDirection ("Wind Direction", Vector) = (1,0,0,0)
        _WindIntensity ("Wind Intensity", Range(0,10)) = 1
        _BendAmount ("Bend Amount", Range(0,2)) = 0.2
        _BendAxisMask ("Bend Axis Mask (x,y,z)", Vector) = (0,1,0,0)

        _TurbulenceScale ("Turbulence Scale", Range(0,5)) = 0.5
        _TurbulenceStrength ("Turbulence Strength", Range(0,2)) = 0.5
        _GustStrength ("Gust Strength", Range(0,3)) = 1.0
        _GustSpeed ("Gust Speed", Range(0,10)) = 2.0

        _HeightInfluence ("Height Influence", Range(0,1)) = 0.75
        _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 300
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM
        #pragma surface surf Standard alpha:fade vertex:vert fullforwardshadows addshadow
        #pragma target 3.0
        #pragma multi_compile_instancing

        sampler2D _MainTex;
        fixed4 _Color;

        float3 _WindDirection;
        float  _WindIntensity;
        float  _BendAmount;
        float3 _BendAxisMask;

        float  _TurbulenceScale;
        float  _TurbulenceStrength;
        float  _GustStrength;
        float  _GustSpeed;

        float  _HeightInfluence;
        float  _Cutoff;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        struct Input
        {
            float2 uv_MainTex;
        };

        float hash(float n) { return frac(sin(n) * 43758.5453123); }

        float noise(float2 x)
        {
            float2 p = floor(x);
            float2 f = frac(x);
            f = f * f * (3.0 - 2.0 * f);
            float n = p.x + p.y * 57.0;
            float res = lerp(lerp(hash(n + 0.0), hash(n + 1.0), f.x),
                             lerp(hash(n + 57.0), hash(n + 58.0), f.x), f.y);
            return res;
        }

        void vert(inout appdata_full v)
        {
            float3 wdir = normalize(_WindDirection);
            float t = _Time.y;

            float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;

            float basePhase = dot(worldPos.xz, float2(0.07, 0.11)) + t;
            float gust = sin(basePhase * _GustSpeed) * 0.5 + 0.5;
            float gustTerm = lerp(1.0, 1.0 + _GustStrength, gust);

            float n1 = noise(worldPos.xz * (_TurbulenceScale + 1e-3) + t * 0.5);
            float n2 = noise(worldPos.xz * (_TurbulenceScale * 1.91 + 1e-3) - t * 0.35);
            float turb = (n1 * 2.0 - 1.0) * 0.7 + (n2 * 2.0 - 1.0) * 0.3;
            float turbTerm = 1.0 + turb * _TurbulenceStrength;

            float heightMask = 1.0 - saturate(v.vertex.y * _HeightInfluence);

            float bendMag = _BendAmount * _WindIntensity * gustTerm * turbTerm * heightMask;

            float sway = sin(basePhase * 1.3 + noise(worldPos.zy * 0.21 + t) * 6.28318);

            float3 bendVec = normalize(wdir + float3(0,0,1) * 0.0001) * bendMag * (0.6 + 0.4 * sway);

            bendVec *= _BendAxisMask;

            v.vertex.xyz += mul((float3x3)unity_WorldToObject, bendVec);
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Metallic = 0;
            o.Smoothness = 0;
            o.Alpha = c.a;

            if (_Cutoff > 0.0 && o.Alpha < _Cutoff) clip(-1);
        }
        ENDCG
    }
    FallBack "Transparent/Diffuse"
}
