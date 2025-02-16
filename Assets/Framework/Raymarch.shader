Shader "Unlit/S_Raymarch"
{
    Properties
    {
        _Color("Color", color) = (1, 0, 0, 1)
        _Size("Radius", Float) = 2.0
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

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
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 _Color;
            fixed4 _Size;

            float sdSphere(float2 p, float size) {
                return length(p) - size;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = sdSphere(i.uv, 5.0);
                return col;
            }
            
            ENDCG
        }
    }
}
