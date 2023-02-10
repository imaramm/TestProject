Shader "Custom/Wave"
{
    Properties
    {
         _MainTex ("Albedo (RGB)", 2D) = "white" {}
         _RefStrength("Reflection Strength", Range(0, 0.1)) = 0.05
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque"}
        LOD 200

        zwrite off

        GrabPass {}

        CGPROGRAM
        #pragma surface surf Nolight noambient alpha:fade

        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _GrabTexture;
        float _RefStrength;

        struct Input
        {
            float2 uv_MainTex;
            float4 color:COLOR;
            float4 screenPos;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            float4 ref = tex2D(_MainTex, IN.uv_MainTex);
            float3 screenUV = IN.screenPos.rgb / IN.screenPos.a;
            
            //o.Emission = float3(screenUV.xy, 0);
            //o.Emission = tex2D(_GrabTexture, float2(screenUV.x+0.05, screenUV.y));
            o.Emission = tex2D(_GrabTexture, (screenUV.xy + ref.x * _RefStrength));
            
        }

        float4 LightingNolight(SurfaceOutput s, float3 lightDir, float atten)
        {
            return float4(0, 0, 0, 1);
        }

        ENDCG
    }
    FallBack "Regacy Shaders/Transparent/Vertexlit"
}
