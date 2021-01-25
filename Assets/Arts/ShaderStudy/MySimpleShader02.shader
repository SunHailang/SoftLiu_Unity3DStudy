// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SoftLiu/Unlit/MySimpleShader02"
{
    Properties
    {

    }
    SubShader
    {

        Pass
        {
            CGPROGRAM
            // 定义 顶点着色器函数
            #pragma vertex vert
            // 定义片源着色器函数
            #pragma fragment frag
            // 最低支持版本
            #pragma target 3.0

            // 定义一个输入结构体
            struct a2v
            {
                // 用 POSITION 告诉Unity用模型空间的顶点坐标填充vertex变量
                float4 vertex : POSITION; 
                // 用 NORMAL 告诉Unity，用模型空间的法线方向填充normal变量
                float3 normal : NORMAL;
                // 用 TEXCOORD0 告诉Unity，用模型的第一套纹理坐标来填充texcoord变量
                float4 texcoord : TEXCOORD0;
            };
            // 定义一个顶点着色器输出结构体
            // 该结构体用于顶点着色器和片元着色器传递信息
            struct v2f
            {
                // pos 包含了顶点在剪裁空间中的位置信息
                float4 pos : SV_POSITION;
                // 存储颜色信息
                fixed3 color : COLOR0;
            };

            v2f vert(a2v v) : POSITION
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                // 储存color
                o.color = v.normal * 0.5 + fixed3(0.5, 0.5, 0.5);
                return o;
            }
            // 把用户输出的颜色渲染到目标物体上
            fixed4 frag(v2f i) : SV_TARGET
            {
                return fixed4(i.color, 1.0);
            }

            ENDCG
        }
    }
    FallBack "Diffuse"
}
