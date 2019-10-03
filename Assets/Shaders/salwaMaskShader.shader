Shader "Unlit/salwaMaskShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_TransparencyMask("TransparencyMask", 2D) = "white" {}
		_playerPos("playerPos", Vector) = (0,0,0,0)
		_playerDir("playerDir", Vector) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        ZWrite off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
		    uniform sampler2D _TransparencyMask;
            uniform float3 _playerPos;
            uniform float3 _playerDir;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
            //    if (tex2D(_TransparencyMask, i.uv).r > 0.5)
            //         discard;


                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                col.a = 0.4
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
