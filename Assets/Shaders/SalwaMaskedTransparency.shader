Shader "Unlit/SalwaMaskedTransparency"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_TransparencyMask("TransparencyMask", 2D) = "white" {}
		_playerPos("playerPos", Vector) = (0,0,0,0)
		_playerDir("playerDir", Vector) = (0,0,0,0)
		_offset("offset", Range(-1.0,1.0)) = 0.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        ZWrite on
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
                float4 worldPos : TEXCOORD1;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
		    uniform sampler2D _TransparencyMask;
            uniform float3 _playerPos;
            uniform float3 _playerDir;
            uniform float _offset;

            v2f vert (appdata v)
            {
                v2f o;
                
                o.worldPos = mul (unity_ObjectToWorld, v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture float4 tex2D(sampler2D samp, float2 s)
                fixed4 col = tex2D(_MainTex, i.uv);
                
                float4 maskVal = tex2D(_TransparencyMask, i.vertex.xy );                               
                
                float4 worldPos = i.worldPos;
                
                if( dot(_playerDir.xz, normalize(_playerPos.xz - worldPos.xz)) + _offset> 0 ) {
                    col.a = 0.34;
//                    col.a = maskVal.r;
                } else {
                    col.a = 1;         
                }

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
