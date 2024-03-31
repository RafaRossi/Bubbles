Shader "Custom/RaymarchShader" {
    Properties {
        
    }
    SubShader {
        Tags { "Queue" = "Background" }
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            /*float3 rot2D(float angle) {
                float s = sin(angle);
                float c = cos(angle);
                return float3x3(c, -s, 0, s, c, 0, 0, 0, 1);
            }*/

            float3 rot3D(float3 p, float3 axis, float angle) {
                return lerp(dot(axis, p) * axis, p, cos(angle)) + cross(axis, p) * sin(angle);
            }

            float sdSphere(float3 p, float size) {
                return length(p) - size;
            }

            float sdBox(float3 p, float3 b) {
                float3 q = abs(p) - b;
                return length(max(q, 0)) + min(max(q.x, max(q.y, q.z)), 0);
            }

            float opSmoothUnion(float d1, float d2, float k) {
                float h = clamp(0.5 + 0.5 * (d2-d1)/k, 0, 1);
                return lerp(d2, d1, h) - k * h * (1 - h);
            }

            float smin(float a, float b, float k) {
                float h = max(k - abs(a - b), 0) / k;
                return min(a, b) - h * h * h * k * (1/6);
            }

            float3 palette(float t) {
                float3 a = float3(0.5, 0.5, 0.5);
                float3 b = float3(0.5, 0.5, 0.5);
                float3 c = float3(1, 1, 1);
                float3 d = float3(0.263, 0.416, 0.557);
                return a + b * cos(6.28318 * (c * t  + d));
            }

            float map(float3 p) {
                float3 boxPosition = float3(0, 0, 0);
                float boxScale = .15;

                float3 q = frac(p) - 0.5;
                //q.xz *= rot2D(_Time.y);
                float3 boxWorldPosition = (q - boxPosition);
                float box = sdBox(boxWorldPosition, float3(boxScale, boxScale, boxScale));

                return box;
            }

            fixed4 frag(v2f i) : SV_Target {
                float2 uv = (i.uv * 2. - _ScreenParams.xy) / _ScreenParams.y;

                float fieldOfView = 2.0;
                float3 rayOrigin = float3(0, 0, -3);
                float3 rayDirection = normalize(float3(uv * fieldOfView, 1));

                float traveledDistance = 0;

                for(int i = 0; i < 80; i++) {
                    float3 rayPositionAlongTime = rayOrigin + rayDirection * traveledDistance;
                    float distanceToObject = map(rayPositionAlongTime);
                    traveledDistance += distanceToObject;
                    if(distanceToObject < .001 || traveledDistance > 100.) break;
                }

                float3 col = palette(traveledDistance * .04);
                return float4(col, 1);
            }
            ENDCG
        }
    }
}
