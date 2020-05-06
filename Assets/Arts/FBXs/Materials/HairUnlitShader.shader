Shader "SoftLiu/Unlit/Player/HairUnlitShader"
{
    Properties
    {
		_MainTex("Texture", 2D) = "while" {}
        _Color("Color", Color) = (1, 1, 1, 1)
		_Specular("_Specular Color", Color) = (1,1,1,1)
		_Gloss("Gloss", Range(8, 200)) = 10
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

			#include "Lighting.cginc"
            #include "UnityCG.cginc"
			#include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
				float3 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float3 worldVertex : TEXCOORD1;
				float3 texcoord : TEXCOORD2;
            };
			sampler2D _MainTex;
			fixed4 _Color;
			half _Gloss;
			fixed4 _Specular;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldNormal = mul(v.normal, (float3x3)unity_WorldToObject);
				o.worldVertex = mul(v.vertex, unity_WorldToObject).xyz;
				o.texcoord = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // 环境光
				float4 texColor = tex2D(_MainTex, i.texcoord);
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.rgb;
				fixed3 normalDir = normalize(i.worldNormal);
				fixed3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
				fixed3 diffuse = _LightColor0.rgb * max(dot(normalDir, lightDir),0)*_Color.rgb;
				fixed3 reflectDir = reflect(-lightDir, normalDir);
				fixed3 viewDir = normalize(_WorldSpaceCameraPos.xyz - i.worldVertex);
				fixed3 specular = _LightColor0.rgb * pow(max(0, dot(viewDir, reflectDir)), _Gloss)*_Specular;
				fixed3 temColor = diffuse+ambient+specular;
                return fixed4(temColor*texColor.rgb,1);
            }
            ENDCG
        }
    }
	FallBack "VertexLit"
}
