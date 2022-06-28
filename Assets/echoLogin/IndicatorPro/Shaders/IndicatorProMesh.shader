Shader "Hidden/IndicatorProMesh"
{
 	Properties
    {
	}

 //=========================================================================
 	SubShader
 	{
  		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }

	    Pass
	  	{
		    Blend SrcAlpha OneMinusSrcAlpha
	  		ZTest Always
	    	ZWrite Off
	        Cull Off
	       	Fog {Mode Off}

	   		CGPROGRAM

	    	#pragma vertex vert
	   		#pragma fragment frag
	   		#pragma fragmentoption ARB_precision_hint_fastest
	   		#pragma exclude_renderers flash

	   		#include "UnityCG.cginc"

	        struct VertInput
	        {
	        	float4 vertex 	: POSITION;
	            float4 vcolor 	: COLOR;
	        };

	        struct Varys
	        {
	            half4 pos  		: SV_POSITION;
	            fixed4 vcolor  	: TEXCOORD0;
	        };

	    	//=============================================
	   		Varys vert ( VertInput ad )
	   		{
	    		Varys v;

				v.pos 		= UnityObjectToClipPos ( ad.vertex );
				v.vcolor  	= ad.vcolor;

	    		return v;
	   		}

	    	//=============================================
	   		fixed4 frag ( Varys v ):COLOR
	   		{
	    		return v.vcolor;
	   		}

		   	ENDCG
  		}
	}
}