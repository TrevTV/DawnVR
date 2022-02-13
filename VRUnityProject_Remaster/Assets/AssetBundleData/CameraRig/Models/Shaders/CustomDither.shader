// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Custom/Dither"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 1.21
		_MainTex("Albedo", 2D) = "white" {}
		_StartDitheringFade("Start Dithering Fade", Float) = 0
		_EndDitheringFade("End Dithering Fade", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf StandardSpecular keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPosition;
		};

		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _EndDitheringFade;
		uniform float _StartDitheringFade;
		uniform float _Cutoff = 1.21;


		inline float Dither8x8Bayer( int x, int y )
		{
			const float dither[ 64 ] = {
				 1, 49, 13, 61,  4, 52, 16, 64,
				33, 17, 45, 29, 36, 20, 48, 32,
				 9, 57,  5, 53, 12, 60,  8, 56,
				41, 25, 37, 21, 44, 28, 40, 24,
				 3, 51, 15, 63,  2, 50, 14, 62,
				35, 19, 47, 31, 34, 18, 46, 30,
				11, 59,  7, 55, 10, 58,  6, 54,
				43, 27, 39, 23, 42, 26, 38, 22};
			int r = y * 8 + x;
			return dither[r] / 64; // same # of instructions as pre-dividing due to compiler magic
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float4 ase_screenPos = ComputeScreenPos( UnityObjectToClipPos( v.vertex ) );
			o.screenPosition = ase_screenPos;
		}

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			o.Albedo = tex2D(_MainTex, uv_MainTex).rgb;
			o.Alpha = 1;
			float4 ase_screenPos = i.screenPosition;
			half4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			half2 clipScreen26 = ase_screenPosNorm.xy * _ScreenParams.xy;
			half dither26 = Dither8x8Bayer( fmod(clipScreen26.x, 8), fmod(clipScreen26.y, 8) );
			clip( ( ( _EndDitheringFade - _StartDitheringFade ) - dither26 ) - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18800
1927;29;1906;1004;399.5914;297.3013;1;True;False
Node;AmplifyShaderEditor.TextureCoordinatesNode;47;-62.14664,-384.9758;Inherit;False;0;44;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;31;64,144;Float;False;Property;_EndDitheringFade;End Dithering Fade;3;0;Create;True;0;0;0;False;0;False;1;2.01;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;30;64,224;Float;False;Property;_StartDitheringFade;Start Dithering Fade;2;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DitheringNode;26;512,304;Inherit;False;1;False;4;0;FLOAT;0;False;1;SAMPLER2D;;False;2;FLOAT4;0,0,0,0;False;3;SAMPLERSTATE;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;44;371,-244;Inherit;True;Property;_MainTex;Albedo;1;0;Create;True;0;0;0;False;0;False;-1;None;2a0417d892b92bd488a61fdabb73abff;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;0,0;False;1;FLOAT2;1,0;False;2;FLOAT;1;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;29;528,176;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WireNode;52;867.2929,-172.9061;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;27;736,256;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;999.2003,-69.70003;Half;False;True;-1;2;ASEMaterialInspector;0;0;StandardSpecular;Custom/Dither;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;3;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;1.21;True;True;0;False;TransparentCutout;;AlphaTest;All;14;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;44;1;47;0
WireConnection;29;0;31;0
WireConnection;29;1;30;0
WireConnection;52;0;44;0
WireConnection;27;0;29;0
WireConnection;27;1;26;0
WireConnection;0;0;52;0
WireConnection;0;10;27;0
ASEEND*/
//CHKSM=5E4050219B83C719F61DB6BA60DD5B43D8E0015C