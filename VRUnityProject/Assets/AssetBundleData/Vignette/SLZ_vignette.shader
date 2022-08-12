// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "SLZ/Vignette"
{
	Properties
	{
		[Header(Vignette Settings)]_Inner("Inner", Range( 0 , 1)) = 0
		_Outter("Outter", Range( 0 , 1)) = 0
		_Color("Color", Color) = (0,0,0,0)
		_Brighten("Brighten", Range( 0 , 1)) = 0
		_ShutEyes("Shut Eyes", Range( 0 , 1)) = 0
		[NoScaleOffset]_EyeTexture("EyeTexture", 2D) = "gray" {}

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Overlay" "Queue"="Overlay" }
	LOD 0

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend DstColor SrcColor
		AlphaToMask Off
		Cull Back
		ColorMask RGBA
		ZWrite Off
		ZTest Always
		
		
		
		Pass
		{
			Name "Unlit"
			Tags { "LightMode"="ForwardBase" }
			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"
			#define ASE_NEEDS_FRAG_WORLD_POSITION


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform float _ShutEyes;
			uniform float4 _Color;
			uniform float _Outter;
			uniform float _Inner;
			uniform sampler2D _EyeTexture;
			uniform float _Brighten;
			float4x4 Inverse4x4(float4x4 input)
			{
				#define minor(a,b,c) determinant(float3x3(input.a, input.b, input.c))
				float4x4 cofactors = float4x4(
				minor( _22_23_24, _32_33_34, _42_43_44 ),
				-minor( _21_23_24, _31_33_34, _41_43_44 ),
				minor( _21_22_24, _31_32_34, _41_42_44 ),
				-minor( _21_22_23, _31_32_33, _41_42_43 ),
			
				-minor( _12_13_14, _32_33_34, _42_43_44 ),
				minor( _11_13_14, _31_33_34, _41_43_44 ),
				-minor( _11_12_14, _31_32_34, _41_42_44 ),
				minor( _11_12_13, _31_32_33, _41_42_43 ),
			
				minor( _12_13_14, _22_23_24, _42_43_44 ),
				-minor( _11_13_14, _21_23_24, _41_43_44 ),
				minor( _11_12_14, _21_22_24, _41_42_44 ),
				-minor( _11_12_13, _21_22_23, _41_42_43 ),
			
				-minor( _12_13_14, _22_23_24, _32_33_34 ),
				minor( _11_13_14, _21_23_24, _31_33_34 ),
				-minor( _11_12_14, _21_22_24, _31_32_34 ),
				minor( _11_12_13, _21_22_23, _31_32_33 ));
				#undef minor
				return transpose( cofactors ) / determinant( input );
			}
			

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
				#endif
				float clampResult99 = clamp( ( _ShutEyes + 0.15 ) , 0.0 , 1.0 );
				float clampResult89 = clamp( ( 1.0 - _ShutEyes ) , 0.02 , 1.0 );
				float3 ase_worldViewDir = UnityWorldSpaceViewDir(WorldPosition);
				ase_worldViewDir = normalize(ase_worldViewDir);
				float4x4 invertVal51 = Inverse4x4( unity_WorldToCamera );
				float2 temp_output_54_0 = (mul( float4( ase_worldViewDir , 0.0 ), invertVal51 ).xyz).xy;
				float2 appendResult79 = (float2(( clampResult89 * (temp_output_54_0).x ) , (temp_output_54_0).y));
				float clampResult84 = clamp( distance( appendResult79 , float2( 0,0 ) ) , 0.0 , 1.0 );
				float smoothstepResult87 = smoothstep( _ShutEyes , clampResult99 , ( 1.0 - clampResult84 ));
				float smoothstepResult97 = smoothstep( 0.0 , 0.45 , _ShutEyes);
				float clampResult96 = clamp( smoothstepResult97 , 0.0 , 1.0 );
				float lerpResult92 = lerp( 1.0 , ( smoothstepResult87 * 0.5 ) , clampResult96);
				float4 temp_cast_2 = (1.0).xxxx;
				float smoothstepResult10 = smoothstep( _Outter , _Inner , ( 1.0 - distance( temp_output_54_0 , float2( 0,0 ) ) ));
				float4 lerpResult23 = lerp( _Color , temp_cast_2 , smoothstepResult10);
				float4 lerpResult70 = lerp( tex2D( _EyeTexture, temp_output_54_0 ) , float4( 0.5,0.5,0.5,0 ) , smoothstepResult10);
				float4 blendOpSrc72 = lerpResult23;
				float4 blendOpDest72 = lerpResult70;
				float4 temp_output_72_0 = (( blendOpDest72 > 0.5 ) ? ( 1.0 - 2.0 * ( 1.0 - blendOpDest72 ) * ( 1.0 - blendOpSrc72 ) ) : ( 2.0 * blendOpDest72 * blendOpSrc72 ) );
				float4 lerpResult63 = lerp( ( temp_output_72_0 * 0.5 ) , temp_output_72_0 , _Brighten);
				
				
				finalColor = ( lerpResult92 * lerpResult63 );
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18800
1920;0;2560;1018;1331.347;323.424;1.361066;True;False
Node;AmplifyShaderEditor.CommentaryNode;56;-751.1795,7.53981;Inherit;False;935.2786;314.9697;Directional Vignette;6;54;53;50;26;51;55;;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldToCameraMatrix;55;-701.1795,212.2889;Inherit;False;0;1;FLOAT4x4;0
Node;AmplifyShaderEditor.CommentaryNode;100;-757.9235,-961.7456;Inherit;False;2580.083;814.9053;sHUT EYES;18;93;92;73;88;78;89;76;80;79;81;84;98;85;99;97;87;94;96;;1,1,1,1;0;0
Node;AmplifyShaderEditor.ViewDirInputsCoordNode;26;-574.8571,57.53981;Float;False;World;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.InverseOpNode;51;-486.1449,212.5095;Inherit;False;1;0;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;1;FLOAT4x4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-326.5875,170.206;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT4x4;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;73;-707.9235,-751.1451;Float;False;Property;_ShutEyes;Shut Eyes;4;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;54;-162.7319,180.7733;Inherit;False;FLOAT2;0;1;2;2;1;0;FLOAT3;0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;88;-523.5311,-877.0027;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;89;-297.007,-907.2059;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0.02;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;78;-401.3471,-604.1102;Inherit;False;FLOAT;0;0;1;1;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;76;-196.48,-788.7285;Inherit;True;2;2;0;FLOAT;0.1;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;80;-163.1305,-506.4405;Inherit;False;FLOAT;1;1;1;1;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;79;137.0247,-738.7033;Inherit;True;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DistanceOpNode;53;11.09906,226.5741;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;58;328.6952,381.1925;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;81;448.7442,-911.7456;Inherit;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;11;-204.7116,505.2733;Float;False;Property;_Outter;Outter;1;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;12;-200.7116,576.2731;Float;False;Property;_Inner;Inner;0;0;Create;True;0;0;0;False;1;Header(Vignette Settings);False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;10;535.7745,493.0585;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;17;298.6047,-36.0125;Inherit;True;Property;_EyeTexture;EyeTexture;5;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;22;359.6115,852.4279;Float;False;Property;_Color;Color;2;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,0,0,0;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;84;715.5709,-879.752;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;410.2563,772.9096;Float;False;Constant;_Float3;Float 3;4;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;98;621.6839,-618.8473;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.15;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;23;707.2563,781.9096;Inherit;False;3;0;COLOR;1,1,1,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ClampOpNode;99;829.6318,-618.8473;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;85;964.5828,-840.9836;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;70;634.504,-86.67548;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0.5,0.5,0.5,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;24;954.0177,375.5136;Float;False;Constant;_Float2;Float 2;4;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;72;953.5039,-23.67548;Inherit;True;Overlay;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;97;922.7354,-521.912;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0.45;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;87;1276.802,-689.763;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0.9;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;96;1306.346,-302.8404;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;93;1374.571,-433.0612;Float;False;Constant;_Float0;Float 0;6;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;94;1525.044,-564.0252;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;64;1065.047,523.1221;Float;False;Property;_Brighten;Brighten;3;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;1304.407,346.388;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0.5;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;63;2028.824,364.4176;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;92;1638.159,-407.6755;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;86;2220.423,228.574;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;2487.81,299.7733;Float;False;True;-1;2;ASEMaterialInspector;0;1;SLZ/Vignette;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;7;2;False;-1;3;False;-1;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;False;False;False;False;False;False;True;0;False;-1;True;0;False;-1;True;True;True;True;True;0;False;-1;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;2;False;-1;True;7;False;-1;True;False;0;False;-1;0;False;-1;True;2;RenderType=Overlay=RenderType;Queue=Overlay=Queue=0;True;2;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=ForwardBase;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;;False;0
WireConnection;51;0;55;0
WireConnection;50;0;26;0
WireConnection;50;1;51;0
WireConnection;54;0;50;0
WireConnection;88;0;73;0
WireConnection;89;0;88;0
WireConnection;78;0;54;0
WireConnection;76;0;89;0
WireConnection;76;1;78;0
WireConnection;80;0;54;0
WireConnection;79;0;76;0
WireConnection;79;1;80;0
WireConnection;53;0;54;0
WireConnection;58;0;53;0
WireConnection;81;0;79;0
WireConnection;10;0;58;0
WireConnection;10;1;11;0
WireConnection;10;2;12;0
WireConnection;17;1;54;0
WireConnection;84;0;81;0
WireConnection;98;0;73;0
WireConnection;23;0;22;0
WireConnection;23;1;25;0
WireConnection;23;2;10;0
WireConnection;99;0;98;0
WireConnection;85;0;84;0
WireConnection;70;0;17;0
WireConnection;70;2;10;0
WireConnection;72;0;23;0
WireConnection;72;1;70;0
WireConnection;97;0;73;0
WireConnection;87;0;85;0
WireConnection;87;1;73;0
WireConnection;87;2;99;0
WireConnection;96;0;97;0
WireConnection;94;0;87;0
WireConnection;20;0;72;0
WireConnection;20;1;24;0
WireConnection;63;0;20;0
WireConnection;63;1;72;0
WireConnection;63;2;64;0
WireConnection;92;0;93;0
WireConnection;92;1;94;0
WireConnection;92;2;96;0
WireConnection;86;0;92;0
WireConnection;86;1;63;0
WireConnection;1;0;86;0
ASEEND*/
//CHKSM=34D553D3596473748E241256D9223FDCEA329FD9