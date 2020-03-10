Shader "SoftLiu/Unlit/GrayShader"
{
    Properties
    {
        //_MainTex ("Texture", 2D) = "white" {}
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
			
			sampler2D _MainTex;			

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD;
				fixed4 col : COLOR;
			};
			
			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD;
				fixed4 col : COLOR;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.col = v.col;
				return o;
			}

			fixed3 maxColorToGray(fixed4 col)
			{
				fixed maxV = max(max(col.r, col.g), col.b);
				return fixed3(maxV, maxV, maxV);
			}
			fixed3 minColorToGray(fixed4 col)
			{
				fixed minV = min(min(col.r, col.g), col.b);
				return fixed3(minV, minV, minV);
			}
			
			fixed4 frag(v2f i):SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				//fixed maxV = max(max(col.r, col.g), col.b);
				col.rgb = minColorToGray(col);
				return col;
			}
            
            ENDCG
        }
    }
	FallBack "Deffuse"
	//CustomEditor "EditorName"
}
