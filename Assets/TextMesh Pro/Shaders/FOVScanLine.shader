Shader "Custom/FOVScanLine"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1, 1, 0, 0.15)
        _ScanColor ("Scan Line Color", Color) = (1, 1, 0.5, 0.8)
        _ScanPos ("Scan Position", Range(0,1)) = 0.0
        _LineWidth ("Scan Line Width", Range(0.001,0.1)) = 0.02
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

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
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _BaseColor;
            float4 _ScanColor;
            float _ScanPos;
            float _LineWidth;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = _BaseColor;

                // Distance from scan position
                float dist = abs(i.uv.x - _ScanPos);

                // If within scan line range, overlay scan color
                if (dist < _LineWidth)
                {
                    float fade = 1.0 - saturate(dist / _LineWidth);
                    col = lerp(col, _ScanColor, fade);
                }

                return col;
            }
            ENDCG
        }
    }
}
