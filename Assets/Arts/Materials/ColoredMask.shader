Shader "SoftLiu/Unlit/ColoredMask"
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_RADIUSBUCE("_RADIUSBUCE", Range(0, 0.5)) = 0.5
	}
	SubShader
	{
		pass
		{
			CGPROGRAM
			#pragma exclude_renderers gles
			#pragma vertex vert
			#pragma fragment frag

			#include "unitycg.cginc"

			float _RADIUSBUCE;
			sampler2D _MainTex;

			struct appdata
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD;
				fixed4 col : COLOR;
			};

			struct v2f
			{
				float4 pos : SV_POSITION ;
				float2 uv : TEXCOORD0;
				fixed4 col : COLOR;
				float2 radiusUV : TEXCOORD1;
			};
			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.pos);
				o.uv = v.uv;
				//将模型UV坐标原点置为中心原点,为了方便计算
				o.radiusUV = v.uv - float2(0.5, 0.5);
				o.col = v.col;
				return o;
			}

			fixed3 maxColorToGray(fixed4 col)
			{
				fixed maxV = max(max(col.r, col.g), col.b);
				return fixed3(maxV, maxV, maxV);
			}

			fixed4 frag(v2f i):COLOR
			{
				fixed4 col = i.col;

				if(abs(i.radiusUV.x) <= (0.5 - _RADIUSBUCE) || abs(i.radiusUV.y) <= (0.5 - _RADIUSBUCE))
				{
					col = tex2D(_MainTex, i.uv);
				}
				else
				{
					if(length(abs(i.radiusUV) - float2(0.5 - _RADIUSBUCE, 0.5 - _RADIUSBUCE)) <= _RADIUSBUCE)
					{
						col = tex2D(_MainTex, i.uv);
					}
					else
					{
						discard;
					}
				}
				//col.rgb = maxColorToGray(col);
				return col;
			}
			ENDCG
		}
	}
}