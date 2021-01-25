Shader "SoftLiu/Unlit/UnlitShaderStudy"
{
    Properties
    {
        
    }
    SubShader
    {
		// "Queue" 有5个可选值 ： 
			// "Background"	-> 1000
			// "Geometry"	-> 2000
			// "AlphaTest"	-> 2450
			// "Transparent" -> 3000
			// "Overlay"		-> 4000
		// "RenderType" 内置 量 ： "Opaque" , "Transparent" , "TransparentCutout" , "Background" , "Overlay"
        Tags 
		{

		}

        Pass
        {
			// 命名一个 Pass 块(名字必须大写), 可以在其他Shader中引用 语法： UsePass ".../(Pass名字)"
			Name "UNLITSTUDY"
			// 使用 GLSL 语言编写Shader ： GLSLPROGRAM ... ENDGLSL
			// 使用 Cg/HLSL 语言编写Shader ： CGPROGRAM ... ENDCG
            CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdataIn
			{
				float4 pos : SV_POSITION;
				fixed4 col : COLOR;
			};

			struct vertOut
			{
				float4 pos : SV_POSITION;
				fixed4 col : COLOR;
			};

			vertOut vert(appdataIn i)
			{
				vertOut o;
				o.pos = i.pos;
				o.col = i.col;
				return o;
			}
			
			fixed4 frag(vertOut o) : SV_Target
			{
				return o.col;
			}

            
            ENDCG
        }
    }
}
