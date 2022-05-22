// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TCL987/3D/Two Texture 3D"
{
	Properties
	{
		[Toggle(_)]_SwapEyes("Swap-Eyes", Int) = 0
		_LeftEyeTexture("Left Eye Texture", 2D) = "white" {}
		_RightEyeTexture("RightEyeTexture", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#pragma target 3.5
		#pragma exclude_renderers switch nomrt 
		#pragma surface surf Unlit keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform int _SwapEyes;
		uniform sampler2D _RightEyeTexture;
		uniform float4 _RightEyeTexture_ST;
		uniform sampler2D _LeftEyeTexture;
		uniform float4 _LeftEyeTexture_ST;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float localStereoEyeIndex72 = ( unity_StereoEyeIndex );
			float lerpResult68 = lerp( localStereoEyeIndex72 , ( -localStereoEyeIndex72 + 1.0 ) , (float)_SwapEyes);
			float2 uv_RightEyeTexture = i.uv_texcoord * _RightEyeTexture_ST.xy + _RightEyeTexture_ST.zw;
			float2 uv_LeftEyeTexture = i.uv_texcoord * _LeftEyeTexture_ST.xy + _LeftEyeTexture_ST.zw;
			float4 ifLocalVar74 = 0;
			if( lerpResult68 > 0.0 )
				ifLocalVar74 = tex2D( _RightEyeTexture, uv_RightEyeTexture );
			else if( lerpResult68 == 0.0 )
				ifLocalVar74 = tex2D( _LeftEyeTexture, uv_LeftEyeTexture );
			o.Emission = ifLocalVar74.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18912
1937;11;2546;978;2286.54;-415.8511;2.077703;True;False
Node;AmplifyShaderEditor.CustomExpressionNode;72;-962.245,1257.518;Float;False;unity_StereoEyeIndex;1;Create;0;StereoEyeIndex;True;False;0;;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;69;-752.1439,1349.034;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;71;-574.1444,1349.034;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.IntNode;70;-566.7838,1444.937;Float;False;Property;_SwapEyes;Swap-Eyes;0;0;Create;True;0;0;0;False;1;Toggle(_);False;0;0;False;0;1;INT;0
Node;AmplifyShaderEditor.LerpOp;68;-296.2345,1258.788;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;75;-112.0065,1329.58;Inherit;False;Constant;_Float4;Float 4;2;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;76;-203.7981,1603.546;Inherit;True;Property;_LeftEyeTexture;Left Eye Texture;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;77;-211.3717,1409.609;Inherit;True;Property;_RightEyeTexture;RightEyeTexture;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ConditionalIfNode;74;158.3793,1209.15;Inherit;False;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;401.6865,1204.589;Float;False;True;-1;3;ASEMaterialInspector;0;0;Unlit;TCL987/3D/Two Texture 3D;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;d3d9;d3d11_9x;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;ps4;psp2;n3ds;wiiu;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;0;4;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;1;False;-1;1;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;69;0;72;0
WireConnection;71;0;69;0
WireConnection;68;0;72;0
WireConnection;68;1;71;0
WireConnection;68;2;70;0
WireConnection;74;0;68;0
WireConnection;74;1;75;0
WireConnection;74;2;77;0
WireConnection;74;3;76;0
WireConnection;0;2;74;0
ASEEND*/
//CHKSM=82496C4C602FA78406E2FBF80327DA7EF1305178