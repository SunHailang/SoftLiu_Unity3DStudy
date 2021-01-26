Shader "SoftLiu/Unlit/MySimpleShader04"
{
    Properties
    {
        // 声明一个Color类型的属性
        _Color("Color Tint", Color) = (1.0, 1.0, 1.0, 1.0)
    }
    SubShader
    {
        Tags
        {
            //"Queue" = "Background" // 1000
            //"Queue" = "Geometry" // 2000 (默认值)
            //"Queue" = "AlphaTest" // 2450
            "Queue" = "Transparent" // 3000 此渲染队列在 Geometry 和 AlphaTest 之后渲染，按照从后到前的顺序。
            //"Queue" = "Overlay" // 4000

            "RenderType" = "Queue"
        }

        Pass
        {
            Tags
            {
                //"LightMode" = "Vertex"
                "LightMode" = "ForwardBase"
            }
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
                fixed4 color : COLOR0;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                // 储存color
                float3 N = normalize(v.normal);
                // 对模型不等比缩放造成法向量变形的解决
                N = normalize(mul(N, (float3x3)unity_WorldToObject).xyz);
                // 将法向量变换到世界坐标系
                N = normalize(mul((float3x3)unity_ObjectToWorld, N));
                float3 L = normalize(_WorldSpaceLightPos0.xyz);
                // 光向量变换到模型坐标空间
                //L= normalize(mul(unity_WorldToObject, float4(L, 0.0)));
                float ndot = saturate(dot(N, L));
                o.color = _LightColor0 * ndot;

                //o.color.rgb = ShadeVertexLights(v.vertex, v.normal);
                // 转换顶点坐标到世界空间
                float3 pos = mul(unity_ObjectToWorld, v.vertex);
                o.color.rgb += Shade4PointLights(unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                                unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                                unity_4LightAtten0,
                                pos, N);
                return o;
            }
            // 把用户输出的颜色渲染到目标物体上
            fixed4 frag(v2f i) : SV_TARGET // 等同于 Color
            {
                // 表示两个向量叉乘
                //fixed3 color = cross(c, _Color.rgb);
                // 表示两个向量各分量相乘
                fixed4 color = i.color * _Color + UNITY_LIGHTMODEL_AMBIENT;

                return color;
            }

            ENDCG
        }
    }
    FallBack "Diffuse"
}
