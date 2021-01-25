// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
// 渲染流水线： 从三维场景出发生成(渲染)成二维图像
// 应用 -> 几何 -> 光栅化
// 应用： CPU完成， 程序猿主导，相机、光源、模型、素材。节省资源的环节，渲染图元：点、线、面
// 几何： GPU上完成的，与图元打交道，把顶点坐标转化到屏幕空间坐标
// 光栅化： GPU上完成的，将几何阶段的数据生成屏幕上的像素，最终渲染出图像

// CPU和GPU的通信： 把数据加载到缓存中，设置渲染状态，调用DrawCall
// 设置渲染状态：设置网络是怎么渲染的。
// DrawCall: 渲染命令，从CPU发出GPU接收执行

Shader "SoftLiu/Unlit/MySimpleShader03"
{
    Properties
    {
        // 声明一个Color类型的属性
        _Color("Color Tint", Color) = (1.0, 1.0, 1.0, 1.0)
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

            // 在CG程序中定义一个与属性名称和类型都一样的变量
            // 定义输入的颜色
            float4 _Color;


            // 定义一个输入结构体
            struct a2v
            {
                // 用 POSITION 告诉Unity用模型空间的顶点坐标填充vertex变量
                float4 vertex : POSITION; 
                // 用 NORMAL 告诉Unity，用模型空间的法线方向填充normal变量
                float3 normal : NORMAL;
                // 用 TEXCOORD0 告诉Unity，用模型的第一套纹理坐标来填充texcoord变量 ShaderModel2(SM)
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
            fixed4 frag(v2f i) : SV_TARGET // 等同于 Color
            {
                //return fixed4(i.color, 1.0);
                fixed3 c = i.color;
                // 表示两个向量叉乘
                //fixed3 color = cross(c, _Color.rgb);
                // 表示两个向量各分量相乘
                fixed3 color = c * _Color.rgb;

                return fixed4(color, 1.0);
            }

            ENDCG
        }
    }
    FallBack "Diffuse"
}
