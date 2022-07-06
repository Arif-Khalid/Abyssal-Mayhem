Shader "echoLogin/echoPortalSolid"
{
	Properties
	{
		_MainTex ("Layer0", 2D) 					= "black" {}
 		_Color0 ("Color", Color ) 					= ( 1, 1, 1, 1 )
   		_Layer0Amplify ("Amplify", Range (0.0, 4.0))	= 1
		_Layer0Rotation ("Speed", float )    		= 0.0
		_Layer0RotationU ("U Offset", float )    	= 0.5
		_Layer0RotationV ("V Offset", float )    	= 0.5

		_Layer0DistortionCount ("Distortion Count", float ) 		= 8
		_Layer0DistortionSpeed ("Distortion Speed", float ) 		= 8
		_Layer0DistortionStrength ("Distortion Strength", Range ( 0, 0.2 ) ) 	= 0

		_Layer1Tex ("Layer1", 2D) 					= "black" {}
 		_Color1 ("Color", Color ) 					= ( 1, 1, 1, 1 )
   		_Layer1Amplify ("Amplify", Range (0.0, 4.0))	= 1
		_Layer1Rotation ("Speed", float )    		= 0.0
		_Layer1RotationU ("U Offset", float )    	= 0.5
		_Layer1RotationV ("V Offset", float )    	= 0.5

		_Layer1DistortionCount ("Distortion Count", float ) 		= 8
		_Layer1DistortionSpeed ("Distortion Speed", float ) 		= 8
		_Layer1DistortionStrength ("Distortion Strength", Range ( 0, 0.2 ) ) 	= 0


   		_MixLayers ("Mix Layers", Range (0.0, 1.0))	= 0.5
	}

	SubShader
	{
//	    Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
	    Cull Off Lighting Off
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			//-------------------------------------------------------------------------------------
			inline half2 CalcLayerUVRotation ( in float2 iuv,in float4 itexST, in float i_angle, in float irotationU, in float irotationV )
			{
				float2x2 	rotationMatrix;
				float 		sinX;
				float 		cosX;

				sincos ( i_angle, sinX, cosX );
				rotationMatrix = float2x2 ( cosX, -sinX, sinX, cosX );

				iuv -= float2 ( irotationU, irotationV );

				iuv = mul ( iuv, rotationMatrix );

				iuv += float2 ( irotationU, irotationV );

				return ( iuv );
			}


			//-------------------------------------------------------------------------------------
			inline half2 CalcLayerUVRotationAuto ( in float2 iuv,in float4 itexST, in float irotationSpeed, in float irotationU, in float irotationV )
			{
				float angle;

				angle = irotationSpeed * _Time.y;

				iuv = CalcLayerUVRotation ( iuv, itexST, angle, irotationU, irotationV );

				return ( iuv );
			}

			//-------------------------------------------------------------------------------------
			inline half2 DistortionCircular ( in half2 i_uv, in half i_count, in half i_speed, in half i_strength, in half i_u, in half i_v )
			{
				half dist = 1.0 - distance ( i_uv, half2(i_u,i_v) );

				half offset = cos ( length ( dist ) * i_count - i_speed * _Time.y ) * i_strength;

				i_uv.xy += half2 ( offset, offset );

				return ( i_uv );
			}


			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv1 : TEXCOORD0;
				float2 uv2 : TEXCOORD1;

				UNITY_FOG_COORDS(2)

				float4 vertex : SV_POSITION;
			};

			sampler2D 			_MainTex;
			float4 				_MainTex_ST;
			sampler2D 			_Layer1Tex;
			float4 				_Layer1Tex_ST;

			fixed4              _Color0;
			fixed4              _Color1;
			fixed               _MixLayers;


			uniform float       _Layer0Amplify;
			uniform float       _Layer1Amplify;
			uniform float       _Layer0Rotation;
			uniform float       _Layer0RotationU;
			uniform float       _Layer0RotationV;
			uniform half       _Layer0DistortionCount;
			uniform half       _Layer0DistortionSpeed;
			uniform half       _Layer0DistortionStrength;

			uniform half       _Layer1Rotation;
			uniform half       _Layer1RotationU;
			uniform half       _Layer1RotationV;
			uniform half       _Layer1DistortionCount;
			uniform half       _Layer1DistortionSpeed;
			uniform half       _Layer1DistortionStrength;

			v2f vert (appdata v)
			{
				v2f o;
				float2 uv;
				o.vertex = UnityObjectToClipPos(v.vertex);
				uv = TRANSFORM_TEX(v.uv, _MainTex);

				o.uv1 = CalcLayerUVRotationAuto ( uv, _MainTex_ST, _Layer0Rotation, _Layer0RotationU, _Layer0RotationV );
				o.uv2 = CalcLayerUVRotationAuto ( uv, _MainTex_ST, _Layer1Rotation, _Layer1RotationU, _Layer1RotationV );


				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				i.uv1 = DistortionCircular ( i.uv1, _Layer0DistortionCount, _Layer0DistortionSpeed, _Layer0DistortionStrength,_Layer0RotationU, _Layer0RotationV );
				i.uv2 = DistortionCircular ( i.uv2, _Layer1DistortionCount, _Layer1DistortionSpeed, _Layer1DistortionStrength, _Layer1RotationU, _Layer1RotationV );

				fixed4 layer0 = tex2D( _MainTex, i.uv1 ) * _Color0;
				fixed4 layer1 = tex2D( _Layer1Tex, i.uv2 ) * _Color1;

				layer0.rgb *= _Layer0Amplify;
				layer1.rgb *= _Layer1Amplify;

				fixed4 col = lerp ( layer0, layer1, _MixLayers );

				UNITY_APPLY_FOG(i.fogCoord, col);

				return col;
			}
			ENDCG
		}
	}

	Fallback "VertexLit"
}
