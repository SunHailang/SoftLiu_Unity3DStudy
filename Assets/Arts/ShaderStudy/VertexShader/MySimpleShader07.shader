Shader "SoftLiu/Unlit/MySimpleShader07"
{
    Properties
    {
        _Float("Float", Range(0.01, 5)) = 1.0
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
            Blend SrcAlpha OneMinusSrcAlpha

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

            // 定义一个顶点着色器输出结构体
            // 该结构体用于顶点着色器和片元着色器传递信息
            struct v2f
            {
                // pos 包含了顶点在剪裁空间中的位置信息
                float4 clipPos : POSITION0;                
                float3 normal : NORMAL;
                float4 vertex : POSITION1;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.clipPos = UnityObjectToClipPos(v.vertex);
                // 储存color
                o.normal = normalize(v.normal);
                o.vertex = v.vertex;
        
                return o;
            }
            // 把用户输出的颜色渲染到目标物体上
            fixed4 frag(v2f i) : SV_TARGET // 等同于 Color
            {
                // 对模型不等比缩放造成法向量变形的解决
                float3 N = mul(i.normal, (float3x3)unity_ObjectToWorld);
                N = normalize(N);
                float3 V = normalize(WorldSpaceViewDir(i.vertex));
                float scale = 1.0 - saturate(dot(N, V));
                    

                return fixed4(1.0, 1.0, 1.0, 1.0) * pow(scale, _Float);
            }

            ENDCG
        }
    }
    //FallBack "Diffuse"
}
