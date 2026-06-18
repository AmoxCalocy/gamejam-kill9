Shader "Custom/CRTEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScanlineIntensity ("Scanline Intensity", Range(0, 0.5)) = 0.08
        _ScanlineCount ("Scanline Count", Float) = 180
        _VignetteStrength ("Vignette Strength", Range(0, 1)) = 0.4
        _ColorShiftX ("Color Shift X", Range(0, 0.005)) = 0.002
        _ColorShiftY ("Color Shift Y", Range(0, 0.005)) = 0.001
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _ScanlineIntensity;
            float _ScanlineCount;
            float _VignetteStrength;
            float _ColorShiftX;
            float _ColorShiftY;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // RGB 色偏采样
                float2 rOffset = float2(_ColorShiftX, _ColorShiftY);
                float2 bOffset = float2(-_ColorShiftX, -_ColorShiftY);
                float r = tex2D(_MainTex, i.uv + rOffset).r;
                float g = tex2D(_MainTex, i.uv).g;
                float b = tex2D(_MainTex, i.uv + bOffset).b;
                fixed4 col = fixed4(r, g, b, 1);

                // 扫描线
                float scanline = sin(i.uv.y * _ScanlineCount * 3.14159 * 2);
                col.rgb *= 1 - abs(scanline) * _ScanlineIntensity;

                // 边缘暗角 (Vignette)
                float2 vignetteUv = i.uv * 2 - 1;
                float vignette = length(vignetteUv) * 0.5;
                col.rgb *= 1 - vignette * _VignetteStrength;

                return col;
            }
            ENDCG
        }
    }
}
