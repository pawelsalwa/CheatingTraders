Shader "Unlit/SalwaMaskedTransparency"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}        
		_TransparencyMask("TransparencyMask", 2D) = "white" {}
		_playerPos("playerPos", Vector) = (0,0,0,0)
		_playerDir("playerDir", Vector) = (0,0,0,0)
		_offset("offset", Range(-1.0,1.0)) = 0.0
        _transparency("transparency", Range(0.0,1.0)) = 0.4
        _isWorking("isWorking", float) = 0
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
                float4 screenPos : TEXCOORD2;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
		    uniform sampler2D _TransparencyMask;
            float4 _TransparencyMask_TexelSize;
         
            uniform float3 _playerPos;
            uniform float3 _playerDir;
            uniform float _offset;
            uniform float _transparency;            
            float _isWorking;

            v2f vert (appdata v)
            {
                v2f o;
                
                o.worldPos = mul (unity_ObjectToWorld, v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture float4 tex2D(sampler2D samp, float2 s)
                fixed4 col = tex2D(_MainTex, i.uv);
                
                float2 textureCoordinate = i.screenPos.xy / i.screenPos.w;
                
                fixed4 maskVal = tex2D(_TransparencyMask, textureCoordinate);
                
                if( dot(_playerDir.xz, normalize(_playerPos.xz - i.worldPos.xz)) + _offset > 0  && _isWorking == 1) {   
                
                    
                    //col.a = _transparency;
                    col.a = 1 - maskVal.x;
                } else {
                    col.a = 1;         
                }

                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
                                          
                //maskVal.x = Remap(maskVal.x, float2(0.0, _ScreenParams.x), float2(0.0, _ScreenParams.y));
                //maskVal.xy = (float2( 0,0 ) + (maskVal.xy - float2( 0,0 )) * (_ScreenParams.y - float2( 0,0 )) / _ScreenParams.x - float2( 0,0 ));
                //value = minVal.y + (value - minVal.x) * (maxVal.y - minVal.y) / (maxVal.x - minVal.x);
                //  (float2( -1,-1 ) + ((ase_screenPosNorm).xy - float2( 0,0 )) * (float2( 1,1 ) - float2( -1,-1 )) / (float2( 1,1 ) - float2( 0,0 )))
                // float lenFromMiddle = length(_ScreenParams.xy/2 - i.screenPos);

                //float lenFromMiddle = length( (float2( 0,0 ) + (_ScreenParams.xy - float2( 0,0 )) * (float2( 1,1 ) - float2( 0,0 )) / (_ScreenParams.xy - float2( 0,0 ))) - 0.5 );                
                                     // length( (float2( 14,14 ) + ((ase_screenPosNorm).xy - float2( 12,12 )) * (float2( 15,15 ) - float2( 14,14 )) / (float2( 13,13 ) - float2( 12,12 ))) )
        }
    }
}
