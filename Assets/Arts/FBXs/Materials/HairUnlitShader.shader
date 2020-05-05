Shader "SoftLiu/Unlit/Player/HairUnlitShader"
{
    Properties
    {
        _Color("Color", COLOR) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }


        Pass
        {
			Tags
			{
				"LightMode" = "ForwardBase"
			}

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

			#include "Lighting.cginc"
            #include "UnityCG.cginc"
			#include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
				float3 worldNormalDir : COLOR0;
            };

			fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldNormalDir = mul(v.normal, (float3x3)unity_WorldToObject);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 环境光
                fixed4 ambient = UNITY_LIGHTMODEL_AMBIENT.rgba;
				fixed3 normalDir = normalize(i.worldNormalDir);
				fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				fixed3 diffuse = _LightColor0.rgb * dot(normalDir, lightDir);
                return _Color + ambient + fixed4(diffuse,1);
            }
            ENDCG
        }
    }
}
