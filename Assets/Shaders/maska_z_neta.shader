Shader "Unlit/Mask"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		_Mask ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		blend SrcAlpha oneminussrcalpha	
		zwrite off
		cull off
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
			sampler2D _Mask;
			fixed4 _Color;

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
				// sample the texture
				fixed2 uv = i.uv;
				uv = uv*4.0;
				uv = frac(uv);
				fixed4 col = tex2D(_MainTex, uv) * _Color;
				fixed4 Maskcol = tex2D(_Mask, i.uv);
				col.a *= Maskcol.r;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);

				//float2 cola = float2(col.x,col.y);
				//cola = cola/1; 
				//col = float4(cola , 1.0 , 1.0);
				return col;
			}
			ENDCG
		}
	}
}
