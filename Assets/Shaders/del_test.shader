// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "del test"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_offset("offset", Range( -90 , 90)) = 1.317647
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_playerPos("playerPos", Vector) = (0,0,0,0)
		_playerDir("playerDir", Vector) = (0,0,0,0)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float3 worldPos;
			float4 screenPos;
		};

		uniform float _offset;
		uniform float3 _playerPos;
		uniform float3 _playerDir;
		uniform sampler2D _TextureSample0;
		uniform float _Cutoff = 0.5;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Alpha = 1;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float3 normalizeResult38 = normalize( ( _playerDir - ase_vertex3Pos ) );
			float dotResult29 = dot( _playerPos , normalizeResult38 );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float2 temp_output_10_0 = (ase_screenPosNorm).xy;
			float4 tex2DNode19 = tex2D( _TextureSample0, temp_output_10_0 );
			float ifLocalVar21 = 0;
			if( ( _offset + dotResult29 ) > 0.0 )
				ifLocalVar21 = 0.0;
			else if( ( _offset + dotResult29 ) == 0.0 )
				ifLocalVar21 = 1.0;
			else if( ( _offset + dotResult29 ) < 0.0 )
				ifLocalVar21 = ( 1.0 - tex2DNode19.r );
			clip( ifLocalVar21 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16900
2632;38;1809;935;672.6855;715.0972;1;True;True
Node;AmplifyShaderEditor.Vector3Node;31;-685.0984,-501.4059;Float;False;Property;_playerDir;playerDir;5;0;Create;True;0;0;False;0;0,0,0;-0.893644,0,0.4487768;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PosVertexDataNode;30;-691.7984,-339.3059;Float;True;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;32;-367.9984,-396.5059;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;2;-979.0955,2.053391;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NormalizeNode;38;-200.6169,-389.8387;Float;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector3Node;28;-323.3984,-630.1058;Float;False;Property;_playerPos;playerPos;4;0;Create;True;0;0;False;0;0,0,0;18.09734,0.1719069,-2.957035;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ComponentMaskNode;10;-717.1304,1.584717;Float;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;36;-16.61688,-656.8387;Float;False;Property;_offset;offset;2;0;Create;True;0;0;False;0;1.317647;0;-90;90;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;29;4.001587,-500.5059;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;19;-85.66602,56.2421;Float;True;Property;_TextureSample0;Texture Sample 0;3;0;Create;True;0;0;False;0;None;21600197df6c3fe4b95d60886e78c973;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;20;129.6771,-165.9412;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;23;-16.99841,-292.5059;Float;False;Constant;_Float0;Float 0;4;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;37;175.3831,-523.8387;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;33;-20.99841,-358.5059;Float;False;Constant;_Float1;Float 1;4;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;6;-963.4955,-484.1467;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GrabScreenPosition;5;-1060.996,-290.4467;Float;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;11;-333.1304,-91.4153;Float;False;Property;_transparency;transparency;1;0;Create;True;0;0;False;0;1.317647;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;34;309.3831,-113.8387;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;12;-3.130402,-74.41528;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LengthOpNode;4;-267.4954,6.753342;Float;True;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ConditionalIfNode;21;341.0016,-494.5059;Float;False;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;3;-483.1954,6.053345;Float;False;5;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT2;1,1;False;3;FLOAT2;-1,-1;False;4;FLOAT2;1,1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;35;300.3831,-187.8387;Float;False;Constant;_Float2;Float 2;4;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;13;808.4,-170;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;del test;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;32;0;31;0
WireConnection;32;1;30;0
WireConnection;38;0;32;0
WireConnection;10;0;2;0
WireConnection;29;0;28;0
WireConnection;29;1;38;0
WireConnection;19;1;10;0
WireConnection;20;0;19;1
WireConnection;37;0;36;0
WireConnection;37;1;29;0
WireConnection;34;0;35;0
WireConnection;34;1;19;1
WireConnection;12;0;11;0
WireConnection;12;1;4;0
WireConnection;4;0;3;0
WireConnection;21;0;37;0
WireConnection;21;1;33;0
WireConnection;21;2;33;0
WireConnection;21;3;23;0
WireConnection;21;4;20;0
WireConnection;3;0;10;0
WireConnection;13;10;21;0
ASEEND*/
//CHKSM=9D5A25ABCC7266439413817740D60389455F040D