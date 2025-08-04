Shader "Custom/FOVTransparent"
{
    Properties
    {
        _Color("Color", Color) = (1,1,0,0.15)
        _ScanColor("Scan Line Color", Color) = (1,1,0.5,0.8)
        _ScanPos("Scan Position", Range(0,1)) = 0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color;
            fixed4 _ScanColor;
            float _ScanPos;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float angleNorm : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.angleNorm = v.uv.x;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
            return _Color; // Just draw base transparent cone, no scan line
            }

            ENDCG
        }
    }
}
