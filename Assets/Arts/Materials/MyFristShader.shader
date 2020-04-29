Shader "SoftLiu/Unlit/MyFristShader"
{
	Properties
	{
		_Color1("Color1", Color) = (1,1,1,1)
		_Color2("Color2", Color) = (1,1,1,1)
		_Int("int", Range(1,100)) = 10
	}
		
	SubShader
	{
		Tags
		{
			"Queue" = "Geometry"
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			

			fixed4 _Color1;
			fixed4 _Color2;
			int _Int;

			struct appdata
			{
				float4 vertex:POSITION; // 应用程序阶段的结构体
				float2 uv:TEXCOORD;
				fixed4 col : COLOR;
			};

			struct v2f
			{
				float4 pos:SV_POSITION; // 顶点着色器传递给片元着色器的结构体
				float2 uv:TEXCOORD;
				fixed4 col : COLOR;
			};
			
			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex); // 几何阶段中的顶点着色器
				o.uv = v.uv;
				o.col = v.col;
				return o;
			}

			fixed checker(float2 uv)
			{
				float2 repeatUV = uv*10;
				float2 c = floor(repeatUV)/2;
				float checker = frac(c.x + c.y) * 2;
				return checker;
				
			}
			
			
			fixed4 frag(v2f i):SV_Target
			{
				//fixed col = checker(i.uv);

				float2 reUV = floor(i.uv * floor(_Int))/2;
				float minUV = frac(reUV.x + reUV.y);
				fixed4 col = lerp(_Color1, _Color2, step(0.5, minUV.x));
				// 光栅化阶段中的片段着色器
				return col;
			}

			ENDCG
		}
	}
	FallBack "Deffuse"
	//CustomEditor "EditorName"
}
