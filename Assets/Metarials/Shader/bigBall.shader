Shader "Custom/DoubleSided_Transparent_Glow"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,0.2)
        _EmissionColor ("Emission Color", Color) = (0.5, 0.2, 1, 1)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Lighting Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            fixed4 _Color;
            fixed4 _EmissionColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Base transparency with glow
                return _Color + _EmissionColor * 0.2;
            }
            ENDCG
        }
    }
    FallBack "Unlit/Transparent"
}