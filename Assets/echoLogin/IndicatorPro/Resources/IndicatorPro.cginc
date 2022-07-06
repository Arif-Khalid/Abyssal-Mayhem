	   		sampler2D 	_MainTex;
	   		float4  	_MainTex_ST;
	   		float4  	_MainTex_TexelSize;
	   		fixed4  	_echoRGBA;
	   		half4   	_echoAspectScale;
	   		half   		_echoOffsetU;
	   		half   		_echoOffsetV;
	   		float       _echoDistAmount1;
	   		float       _echoDistSpeed1;
	   		float       _echoDistStr1;
	   		float       _echoDistAmount2;
	   		float       _echoDistSpeed2;
	   		float       _echoDistStr2;
	   		half        _echoMix;
			half        _echoEdgeWidth;
			half4       _echoEdgeColor;
			half        _echoEdgeAmplify;

			float4       _echoBendPivot;
			float4       _echoBendCenter;
			float        _echoBend;
			float        _echoBendDistance;
			//fixed       _echoColorAmplify;

	        struct VertInput
	        {
	        	float4 vertex : POSITION;
				float3 normal   : NORMAL;
				float2 texcoord : TEXCOORD0;
	        };

	        struct Varys
	        {
	            half4 pos  : SV_POSITION;
	            half2 tc1  : TEXCOORD0;
	        };


			//=============================================
			inline void CalcDistortionWaves ( inout half2 i_uv )
			{
#ifdef IPRO_DIST_WAVES_ON
				half u1 =  sin ( ( _echoDistAmount1 * i_uv.y - ( _Time.y * _echoDistSpeed1 ) ) ) * _echoDistStr1;
				half v1 =  sin ( ( _echoDistAmount2 * i_uv.x - ( _Time.y * _echoDistSpeed2 ) ) ) * _echoDistStr2;
				i_uv += half2 ( u1 , v1 );
#endif
			}

			//=============================================
			inline void CalcDistortionRipple ( inout half2 i_uv )
			{
#ifdef IPRO_DIST_RIPPLE_ON
				half dist = 1.0 - ( distance ( i_uv, float2 ( 0.5, 0.5 ) ) * 2.0 );
				half offset = sin ( length ( dist ) * _echoDistAmount1 + _echoDistSpeed1 * _Time.y ) * ( _echoDistStr1 * dist );
				i_uv += half2 ( offset , offset );
#endif
			}

			//=============================================
			inline void CalcDissolve ( inout fixed3 i_finalRGB, inout fixed i_alpha, fixed i_dissolve, in fixed3 i_progressColor, in half i_amplify, in half i_edgeWidth )
			{
#ifdef IPRO_DISSOLVE_ON
				fixed edge 		= lerp ( i_dissolve + i_edgeWidth, i_dissolve - i_edgeWidth, i_dissolve );
				fixed alpha 	= smoothstep(  _echoMix + i_edgeWidth, _echoMix - i_edgeWidth, edge );
				i_finalRGB 		= lerp ( i_progressColor * i_amplify, i_finalRGB, alpha );
				//i_finalRGB  	*= alpha;
				i_alpha         *= alpha;
#endif
			}



