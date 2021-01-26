Shader "SoftLiu/Unlit/MySimpleShader06"
{
    Properties
    {
        // 声明一个Color类型的属性
        _MainColor("Main Color", Color) = (1.0, 1.0, 1.0, 1.0)

        _SpacularColor("Spacular Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _Shininess("Shininess", Range(1, 64)) = 8
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
            float4 _MainColor;
            float4 _SpacularColor;
            int _Shininess;


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
                
                float3 normal : NORMAL;

                float4 vertex : COLOR;
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                // 储存color
                o.normal = normalize(v.normal);
                // 对模型不等比缩放造成法向量变形的解决
                // N = normalize(mul(N, (float3x3)unity_WorldToObject).xyz);
                // // 将法向量变换到世界坐标系
                // o.normal = UnityObjectToWorldNormal(N);
                // o.lightDir = normalize(WorldSpaceLightDir(v.vertex).xyz);
                o.vertex = v.vertex;
        
                return o;
            }
            // 把用户输出的颜色渲染到目标物体上
            fixed4 frag(v2f i) : SV_TARGET // 等同于 Color
            {
                fixed4 color = UNITY_LIGHTMODEL_AMBIENT;
                // 顶点法向量
                float3 N = UnityObjectToWorldNormal(i.normal);
                // 顶点指向灯光的向量
                float3 L = normalize(WorldSpaceLightDir(i.vertex));
                // 顶点指向摄像机的向量
                float3 V = normalize(WorldSpaceViewDir(i.vertex));

                // diffuse color 漫反射
                //float diffuseScale = saturate(dot(N, L));
                float diffuseScale = max(0, dot(N, L));
                color += _LightColor0 * diffuseScale;

                // 镜面反射
                float3 H = normalize(L + V);
                float spacularScal = saturate(dot(H, N));
                color += _LightColor0 * pow(spacularScal, 32);

                // compuer 4 point light
                // 转换顶点坐标到世界空间
                float3 posWorld = mul(unity_ObjectToWorld, i.vertex).xyz;
                color.rgb += Shade4PointLights(unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
                                unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
                                unity_4LightAtten0,
                                posWorld, N);

                return color * _MainColor;
            }

            ENDCG
        }
    }
    //FallBack "Diffuse"
}
