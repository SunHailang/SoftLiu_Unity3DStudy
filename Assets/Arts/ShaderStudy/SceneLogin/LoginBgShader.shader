Shader "SoftLiu/Login/Unlit/LoginBgShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MainColor ("MainColor", Color) = (0.95, 0.0, 1.0, 0.36)
        _EdgeWidth ("EdgeWidth", Range(0, 10)) = 1.0
        _EdgeModulusX ("EdgeModulusX", Range(0, 1)) = 0.0
        _EdgeModulusY ("EdgeModulusY", Range(0, 1)) = 0.0
        _EdgeColor ("EdgeColor", Color) = (1.0, 1.0, 1.0, 1.0)
        _EdgeSinValue("EdgeSinValue", Range(1, 10)) = 5
        _RADIUSBUCE("_RADIUSBUCE",Range(0,0.5))=0.2
        _RADIUSBUCE1("_RADIUSBUCE1",Range(0,0.5))=0.2
    }
    SubShader
    {
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 clipPos : POSITION0;
                float2 uv : TEXCOORD0;
                float4 vertex : POSITION1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float4 _MainColor;
            float _EdgeModulusX;
            float _EdgeModulusY;
            float _EdgeWidth;
            float _EdgeSinValue;
            float4 _EdgeColor;
            float _RADIUSBUCE;
            float _RADIUSBUCE1;

            v2f vert (appdata v)
            {
                v2f o;
                float4 vertex = v.vertex;
                o.clipPos = UnityObjectToClipPos(vertex);
                float2 uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv = uv - float2(0.5, 0.5);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 color =_EdgeColor;
                if(abs(i.uv.x) < 0.5 - _RADIUSBUCE || abs(i.uv.y) < 0.5 - _RADIUSBUCE)    //即上面说的|x|<(0.5-r)或|y|<(0.5-r)
                {
                    //return color * abs(_SinTime.w) * _EdgeSinValue;
                    //discard;
                }
                else
                {
                    if(length(abs(i.uv) - float2(0.5 - _RADIUSBUCE, 0.5 - _RADIUSBUCE)) < _RADIUSBUCE)
                    {
                        //return color * abs(_SinTime.w) * _EdgeSinValue;
                        //discard;
                    }
                    else
                    {
                        discard;
                    } 
                }
                return fixed4(0.95, 0.0, 1.0, 0.36);
                //return color;
            }
            ENDCG
        }
       
    }
}
