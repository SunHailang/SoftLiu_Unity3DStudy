// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "SoftLiu/Unlit/MySimpleShader01"
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

            struct Input
            {
                float3 pos : POSITION;
            };

            struct v2f
            {
                float4 pos : POSITION;
            };

            v2f vert(Input input) : SV_POSITION
            {
                v2f o;
                o.pos = UnityObjectToClipPos(input.pos);
                return o;
            }
            // 把用户输出的颜色渲染到目标物体上
            fixed4 frag(v2f v) : SV_TARGET
            {
                return fixed4(1.0, 1.0, 1.0, 1.0);
            }

            ENDCG
        }
    }
    FallBack "Diffuse"
}
