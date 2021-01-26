Shader "SoftLiu/Custom/MySurfaceShader01"
{
    Properties
    {
        _Int("Int", Int) = 2
        _Float("Float", Float) = 1.5
        _Range("Range", Range(0.0, 5.0)) = 1.0

        _Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)// 颜色
        _Vector("Vector", Vector) = (2, 3, 6, 1)// 向量

        _2D("2D", 2D) = ""{}// 2D
        _Cube("Cube", Cube) = "white"{}// Cube
        _3D("3D", 3D) = "black"{}// 3D
    }

    SubShader
    {
        // 可选的
        //[Tags]//标签
        Tags
        {
            "Queue" = "Transparent" // 控制渲染顺序，指定该物体属于渲染队列
        }
        // 可选的
        //[RenderSetup]
        // 尽量使用最小数目的Pass
        Pass
        {
            CGPROGRAM

            ENDCG
        }
    }

    // 默认是个漫反射
    Fallback "Diffuse"
    Fallback Off // 关闭Fallback功能
}
