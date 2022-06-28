Shader "Hidden/IndicatorPro"
{
 	Properties
    {
  		_MainTex ("Texture", 2D)      			= "gray" {}
    	_echoUV("UV Offset u1 v1", Vector )   	= ( 0, 0, 0, 0 )
    	_echoScale ("Scale XYZ", Vector )   	= ( 1.0, 1.0, 1.0, 1.0 )
    	_echoRGBA ("Scale XYZ", Vector )   		= ( 1.0, 1.0, 1.0, 1.0 )

    	MySrcMode ("SrcMode", Float) = 1
    	MyDstMode ("DstMode", Float) = 0
	}

 //=========================================================================
 	SubShader
 	{
  		Tags { "Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Transparent" }

		Pass
	  	{
	    	ZWrite Off
	  		ZTest Always
	        Blend [MySrcMode] [MyDstMode]
	        Cull Off
	       	Fog {Mode Off}

	   		CGPROGRAM

	    	#pragma vertex vert
	   		#pragma fragment frag
	   		#pragma fragmentoption ARB_precision_hint_fastest
	   		#pragma exclude_renderers flash
	   		#pragma multi_compile __ IPRO_DIST_WAVES_ON
	   		#pragma multi_compile __ IPRO_DIST_RIPPLE_ON
	   		#pragma multi_compile __ IPRO_DISSOLVE_ON
			#pragma multi_compile __ IPRO_PREMULTIPLY
			#pragma multi_compile __ IPRO_LOCKALPHA
			#pragma multi_compile __ IPRO_PACKEDFORMAT
			#pragma multi_compile __ IPRO_CURVE

	   		#include "UnityCG.cginc"
			#include "IndicatorPro.cginc"


	    	//=============================================
	   		Varys vert ( VertInput ad )
	   		{
	    		Varys v;

	    		v.tc1 = ad.texcoord.xy;

				float4 	wpos 		= mul( unity_ObjectToWorld, float4 ( ad.vertex.xyz, 1.0) );
				float 	dist 		= length ( wpos.xyz - _echoBendCenter.xyz );
				float3 	worldNormal = mul( unity_ObjectToWorld, float4( ad.normal, 0.0 ) ).xyz;

				wpos.xyz = wpos.xyz + worldNormal * ( _echoBend * dist );

				v.pos = mul( UNITY_MATRIX_VP, wpos );
				v.pos.xy += half2 ( _echoOffsetU, _echoOffsetV );

#if UNITY_UV_STARTS_AT_TOP
	    		if ( _MainTex_TexelSize.y < 0 )
	     		v.tc1.y = 1.0-v.tc1.y;
#endif

	    		return v;
	   		}

	    	//=============================================
	   		fixed4 frag ( Varys v ):COLOR
	   		{
				fixed3 	finalRGB;
				fixed  	dissolve;
				fixed 	alpha;
				half2  	uvHold = v.tc1;

				CalcDistortionWaves ( v.tc1 );
				CalcDistortionRipple ( v.tc1 );

				fixed4 fcolor = tex2D ( _MainTex, v.tc1 );

				//fcolor.rgb *= 2.0f;

#ifdef IPRO_LOCKALPHA
				alpha = tex2D ( _MainTex, uvHold ).a * _echoRGBA.a;
#else
	#if defined (IPRO_DISSOLVE_ON) && !defined(IPRO_PACKEDFORMAT)
				alpha = ( fcolor.r + fcolor.g + fcolor.b ) * 0.3333333 * _echoRGBA.a;
	#else
				alpha = fcolor.a * _echoRGBA.a;
	#endif
#endif

#if IPRO_PACKEDFORMAT
				finalRGB = fcolor.rrr * _echoRGBA.rgb;
	#ifdef IPRO_DISSOLVE_ON
				dissolve = fcolor.g;
	#endif
#else
				finalRGB = fcolor.rgb * _echoRGBA.rgb;
	#ifdef IPRO_DISSOLVE_ON
				dissolve = fcolor.a;
	#endif

#endif

				CalcDissolve ( finalRGB, alpha, dissolve, _echoEdgeColor, _echoEdgeAmplify, _echoEdgeWidth );

#ifdef IPRO_PREMULTIPLY
				finalRGB *= alpha;
#endif

				return fixed4 ( finalRGB, alpha );
	   		}

		   	ENDCG
  		}
	}
}