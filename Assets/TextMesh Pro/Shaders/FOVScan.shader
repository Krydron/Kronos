Shader "Custom/RadarFOV"
{
    Properties
    {
        _BaseColor ("Base FOV Color", Color) = (1,1,0,0.15)
        _ScanColor ("Scan Line Color", Color) = (1,0,0,0.6)
        _FOVAngle ("FOV Angle", Float) = 90
        _Range ("Range", Float) = 10
        _ScanWidth ("Scan Line Width (deg)", Float) = 2
        _CurrentAngle ("Current Sweep Angle", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
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
                float3 worldPos : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            float4 _BaseColor;
            float4 _ScanColor;
            float _FOVAngle;
            float _Range;
            float _ScanWidth;
            float _CurrentAngle;

            float3 _Origin;
            float3 _ForwardDir;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Relative position from origin
                float2 rel = i.worldPos.xz - _Origin.xz;

                // Convert to local enemy space
                float2 fwd = normalize(_ForwardDir.xz);
                float2 right = float2(-fwd.y, fwd.x);
                float localX = dot(rel, right);
                float localY = dot(rel, fwd);

                // Distance & angle from forward
                float dist = length(rel);
                if (dist > _Range) discard; // outside range

                float angle = degrees(atan2(localX, localY));

                // Cull outside FOV fan
                if (abs(angle) > _FOVAngle * 0.5) discard;

                // Distance from scan line
                float diff = abs(angle - _CurrentAngle);
                diff = min(diff, 360 - diff); // wrap around

                // Inside scan line?
                if (diff < _ScanWidth * 0.5)
                {
                    return _ScanColor;
                }

                return _BaseColor;
            }
            ENDCG
        }
    }
}
