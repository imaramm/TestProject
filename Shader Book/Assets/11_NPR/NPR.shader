Shader "Custom/NPR"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _ChangeCol ("Change Color", Color) = (1,1,1,1)
        _ChangeSize ("Change Size", Range(0.001,0.01)) = 0.001
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        cull front

        // 1st Pass
        CGPROGRAM
        #pragma surface surf Nolight vertex:vert noshadow noambient

        #pragma target 3.0

        sampler2D _MainTex;
        float4 _ChangeCol;
        float _ChangeSize;

        void vert(inout appdata_full v)
        {
            v.vertex.xyz += v.normal.xyz * _ChangeSize;
        }

        struct Input
        {
            float4 color : COLOR;
        };

        void surf (Input IN, inout SurfaceOutput o)
        {
            
        }

        float4 LightingNolight(SurfaceOutput s, float3 lightDir, float atten)
        {
            return float4 (0, 0, 0, 1);
        }

        ENDCG
    
        cull back

    // 2nd Pass
     CGPROGRAM
    #pragma surface surf Lambert

    sampler2D _MainTex;

    struct Input
    {
        float2 uv_MainTex;
    };

    void surf(Input IN, inout SurfaceOutput o)
    {
        fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
        o.Albedo = c.rgb;
        o.Alpha = c.a;
    }
        ENDCG
    }
    FallBack "Diffuse"
}
