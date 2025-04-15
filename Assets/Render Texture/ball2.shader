Shader "Custom/RT_ZoomCenter_CutBlack"
{
    Properties
    {
        _MainTex ("Render Texture", 2D) = "white" {}
        _AlphaThreshold ("Black Cutoff", Range(0, 0.3)) = 0.05
        _Zoom ("Zoom Factor", Range(0.5, 4)) = 2.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Zoom;
            float _AlphaThreshold;

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                // 缩放 UV：以中心为基准放大贴图
                float2 centered = v.uv - 0.5;
                centered *= 1.0 / _Zoom;
                o.uv = centered + 0.5;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                // 如果颜色接近黑，就做透明处理
                float brightness = dot(col.rgb, float3(0.299, 0.587, 0.114)); // 人眼感知亮度
                if (brightness < _AlphaThreshold)
                {
                    discard;
                }

                return col;
            }
            ENDCG
        }
    }
}