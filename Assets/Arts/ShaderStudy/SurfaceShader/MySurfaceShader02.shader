Shader "SoftLiu/Custom/MySurfaceShader02"
{
    Properties
    {
        _Color("Color Tint", Color) = (1.0, 1.0, 1.0, 1.0)
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent" // 控制渲染顺序，指定该物体属于渲染队列
        }
        CGPROGRAM
        #pragma surface surf Lambert

        #include "Lighting.cginc"

        struct Input
        {
            float4 vertex : POSITION;
            fixed3 normal : NORMAL;
        };
        
        void surf(Input IN, inout SurfaceOutput o)
        {
            // 漫射颜色
            o.Albedo = fixed3(1.0, 0.0, 0.0);
        }

        ENDCG
    }

    // 默认是个漫反射
    Fallback "Diffuse"
}
