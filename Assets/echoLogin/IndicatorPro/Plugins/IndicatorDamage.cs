using UnityEngine;
using System.Collections;
using IndicatorPro;

//namespace IndicatorPro
	//----------------------------------------------------------------------------------------------------------------
	[System.Serializable, CreateAssetMenu(menuName = "IndicatorPro/Damage Indicator")]
	public class IndicatorDamage : ScriptableObject
	{
		public IndicatorDamageData  data 		= new IndicatorDamageData();
		public bool 				enabled 	= true;


		//============================================================
		public void ResetPreview()
		{
			data.mesh 			= null;
			data.hitMaterial 	= null;
		}

		//============================================================
		private void SendData()
		{
			data.hitMaterial.SetFloat ( data.offsetU_ID , data.offsetU );
			data.hitMaterial.SetFloat ( data.offsetV_ID, data.offsetV );

			if ( data.dissolve )
			{
				data.hitMaterial.SetFloat ( data.dissolve_ID, data.dissolveCurve.Evaluate ( 0 ) );
				data.hitMaterial.SetFloat ( data.dissolveEdgeWidth_ID, data.dissolveEdgeWidth * 0.2f );
				data.hitMaterial.SetFloat ( data.dissolveEdgeAmplify_ID, data.dissolveEdgeAmplify );
				data.hitMaterial.SetColor ( data.dissolveEdgeColor_ID, data.dissolveEdgeColor );
			}

			if ( data.distortion != DISTORTION.OFF )
			{
				float strength = data.distortionCurve.Evaluate ( 0 ) * 0.5f;
				data.hitMaterial.SetFloat ( data.distortionAmount1_ID, data.distortionAmount1 );
				data.hitMaterial.SetFloat ( data.distortionSpeed1_ID, data.distortionSpeed1 );
				data.hitMaterial.SetFloat ( data.distortionStrength1_ID, data.distortionStrength1 * strength );

				data.hitMaterial.SetFloat ( data.distortionAmount2_ID, data.distortionAmount2 );
				data.hitMaterial.SetFloat ( data.distortionSpeed2_ID, data.distortionSpeed2 );
				data.hitMaterial.SetFloat ( data.distortionStrength2_ID, data.distortionStrength2 * strength );
			}
		}

		//============================================================
		public void Create()
		{
			DamageIndicator[] hobjs = new DamageIndicator[data.maxHitCount];
			int loop;

			data.hitMaterial = new Material ( Shader.Find( "Hidden/IndicatorPro" ) );

			IndicatorProDLL.GetShaderPropertyIDS ( data );
			IndicatorProDLL.CreateMesh ( data );
			IndicatorProDLL.SetBlendMode ( data );
			IndicatorProDLL.SetKeywords ( data );

			SendData();

			for ( loop = 0; loop < hobjs.Length; loop++ )
				hobjs[loop] = new DamageIndicator();

			data.DamageIndicators = new EchoList<DamageIndicator>( hobjs );
		}

		//============================================================
		public DamageIndicator Activate ( Vector3 i_source, float i_strength )
		{
			return ( IndicatorProDLL.Activate ( data, i_source, i_strength ) );
		}

		//============================================================
		public DamageIndicator Activate ( Transform i_source )
		{
			return ( IndicatorProDLL.Activate ( data, i_source ) );
		}

		//============================================================
		public void SetAngle ( float i_angle )
		{
			data.manualAngle = i_angle;
		}

		//============================================================
		public float GetManualAngle ( DamageIndicator i_ho )
		{
			return ( data.manualAngle );
		}

	}

