Shader "SoftLiu/Unlit/MySimpleShader09"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white"{}	
        _Float("Float", Range(0.01, 5)) = 1.0
        // _Outer("Other", Range(0.0, 1.0)) = 0.2
    }
    SubShader
    {
        Tags
        { 
            "Queue" = "Transparent"
            "RenderType" = "Queue"
        }

        Pass
        {
            // Blend SrcAlpha OneMinusSrcAlpha
            // ZWrite Off

            CGPROGRAM
            // CG 与 HLSL 代码
            //#pragma surface surf Lambert
            // 定义 顶点着色器函数
            #pragma vertex vert
            // 定义片源着色器函数
            #pragma fragment frag
            // 最低支持版本
            #pragma target 3.0        

            #include "UnityCG.cginc"
            #include "Lighting.cginc" 
            #include "UnityShaderVariables.cginc"

            float _Float;

            sampler2D _MainTex;	
            float4 _MainTex_ST;

            struct appdata
            {
                float4 vertex : POSITION0;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            // 定义一个顶点着色器输出结构体
            // 该结构体用于顶点着色器和片元着色器传递信息
            struct v2f
            {
                // pos 包含了顶点在剪裁空间中的位置信息
                float4 clipPos : POSITION0;                
                float3 normal : NORMAL;
                float4 vertex : POSITION1;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.clipPos = UnityObjectToClipPos(v.vertex);
                // 储存color
                o.normal = normalize(v.normal);
                o.vertex = v.vertex;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
        
                return o;
            }
            // 把用户输出的颜色渲染到目标物体上
            fixed4 frag(v2f i) : SV_TARGET // 等同于 Color
            {
                float2 uv = i.uv;
                uv.x -= _Time.x * 10;
                if(saturate(uv.x) == 0.0) uv.x = 1.0;
                else if(saturate(uv.x) == 1.0) uv.x = 0.0;
                fixed4 color = tex2D(_MainTex, uv);

                
                //float scale = saturate(dot(i.normal, normalize(WorldSpaceLightDir(i.vertex))));
                float3 diffuse = color.rgb;


                return fixed4(diffuse, 1.0);// + UNITY_LIGHTMODEL_AMBIENT;
            }

            ENDCG
        }
    }
    //FallBack "Diffuse"
}
