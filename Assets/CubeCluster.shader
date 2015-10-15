Shader "Custom/CubeCluster"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        CGPROGRAM

        #pragma surface surf Standard fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        float2 _select;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex / 4 + _select) * _Color;
            fixed s = IN.uv_MainTex.x < 0;
            //o.Albedo = lerp(c.rgb, 1, IN.uv_MainTex.x < 0);
            o.Albedo = lerp(0, 1, s);
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
            o.Emission = lerp(c.rgb, 0, s) * abs(sin(IN.uv_MainTex.y * 80 + _Time.y * 10));
        }
        ENDCG
    }
    FallBack "Diffuse"
}
