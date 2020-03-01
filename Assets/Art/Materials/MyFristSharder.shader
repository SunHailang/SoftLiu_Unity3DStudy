Shader "SoftLiu/Unlit/MyFristSharder"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
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
			

			fixed4 _Color;

			struct appdata
			{
				float4 vertex:POSITION; // 应用程序阶段的结构体
				float2 uv:TEXCOORD;
			};

			struct v2f
			{
				float4 pos:SV_POSITION; // 顶点着色器传递给片元着色器的结构体
				float2 uv:TEXCOORD;
			};
			
			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex); // 几何阶段中的顶点着色器
				o.uv = v.uv;
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
				fixed col = checker(i.uv);
				return col;//fixed4(i.uv,0,1); // 光栅化阶段中的片段着色器
			}

			ENDCG
		}
	}
		//FallBack "Deffuse"
		//CustomEditor "EditorName"
}
