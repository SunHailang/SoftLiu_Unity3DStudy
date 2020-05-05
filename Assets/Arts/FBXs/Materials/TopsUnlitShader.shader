Shader "SoftLiu/Unlit/Player/TopsUnlitShader"
{
    Properties
    {
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
			Tags{ "LightMode" = "ForwardBase" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

			#include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
				float3 worldNormalDir : COLOR0;
            };


            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldNormalDir = mul(v.normal, (float3x3)unity_WorldToObject);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				// 环境光
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;
				// 法向量
				fixed3 normalDir = normalize(i.worldNormalDir);
				// 光照方向
				fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				// 漫反射计算
				fixed3 halfLambert = dot(normalDir, lightDir) * 0.5+ 0.5;
				fixed3 diffuse = _LightColor0.rgb * halfLambert;
				fixed3 resultColor = diffuse + ambient;
                return fixed4(resultColor, 1);
            }
            ENDCG
        }
    }
}
