Shader "SoftLiu/BgLoading/Unlit/BgUnlitShader"
{
    Properties
    {
        _MainTex("MainTex", 2D) = "white"{}
        _A("A", Range(0.0001, 0.01)) = 0.005
        _W("W", Range(1, 20)) = 1
        _R("R", Range(0.2, 1)) = 0.2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _A;
            float _W;
            float _R;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                
                float dis = distance(uv, float2(0.5, 0.5));
                float scale = _A * sin(_W * _Time.x * UNITY_PI);

                //uv +=  _A * sin(uv * UNITY_PI * _W + _Time.y);
                _A *= saturate(1 - dis / _R);
                scale = _A * sin(-dis * 3.14 * _W + _Time.y);
                uv = uv + uv * scale;
                //if(uv.x >= 1.0) uv.x = (uv.x - 1.0);
                // sample the texture
                fixed4 col = tex2D(_MainTex, uv);
                return col;
            }
            ENDCG
        }
    }
}
