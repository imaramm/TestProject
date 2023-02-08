Shader "Custom/Hologram"
{
    Properties
    {
        _BumpMap("BumpMap", 2D) ="bump" {}
        _MovingSpeed("MovingSpeed", Range(-5,5)) = 0.5
        _CColor("Color", Color) = (1,1,1,1)
        _SSize("Size", Range(1,10)) = 3
        _Thickness("Thickness", Range(-10,10)) = 1
    }
        SubShader
        {
            //Tags { "RenderType"="Opaque" }
            Tags {"Render Type" = "Transparent" "Queue" = "Transparent"}
            LOD 200

            CGPROGRAM
            #pragma surface surf nolight noambient alpha:fade

            #pragma target 3.0

        sampler2D _BumpMap;
        fixed _MovingSpeed;
        fixed _Thickness;
        float4 _CColor;
        float _SSize;
        

        struct Input
        {
            float2 uv_BumpMap;
            float3 viewDir;
            float3 worldPos;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
            o.Emission = _CColor.rgb;
            float rim = saturate(dot(o.Normal, IN.viewDir));
            rim = saturate(pow(1 - rim, 3) + pow(frac(IN.worldPos.b * _SSize - _Time.y * _MovingSpeed), 5) * _Thickness);
            //rim = pow(1 - rim, 3) + pow(frac(IN.worldPos.b * 3 - _Time.y),30);
            o.Alpha = rim;
        }

        float4 Lightingnolight(SurfaceOutput s, float3 lightDir, float atte)
        {
            return float4(0, 0, 0, s.Alpha);
        }

        ENDCG
    }
    FallBack "Diffuse"
}
