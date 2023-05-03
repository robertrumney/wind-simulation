Shader "Custom/WindInteractionShader" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _WindDirection ("Wind Direction", Vector) = (1, 0, 0)
        _WindIntensity ("Wind Intensity", Range(0, 5)) = 1
        _BendAmount ("Bend Amount", Range(0, 1)) = 0.1
    }

    SubShader {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        LOD 100

        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;
        float3 _WindDirection;
        float _WindIntensity;
        float _BendAmount;

        struct Input {
            float2 uv_MainTex;
            float3 vertex;
        };

        void surf (Input IN, inout SurfaceOutput o) {
            o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
            o.Alpha = tex2D(_MainTex, IN.uv_MainTex).a;
            o.Specular = 0.0;
            o.Gloss = 0.0;
            o.Normal = (tex2D(_MainTex, IN.uv_MainTex).g - 0.5) * _WindIntensity;

            float windBend = dot(normalize(_WindDirection), IN.vertex) * _BendAmount;
            o.Normal += windBend;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
