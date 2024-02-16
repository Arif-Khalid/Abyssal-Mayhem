using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IndicatorPro;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
#endif

[ExecuteInEditMode]
[System.Serializable]
public class IndicatorProManager : IndicatorProDLL
{
	private static Dictionary <string, IndicatorDamage> 	_dict;
	private static IndicatorDamage[] 				_DamageIndicators	= null;
	private int                                     _echoBend_ID;
	private int                                     _echoBendCenter_ID;
	private int                                     _echoBendDistance_ID;
#if UNITY_EDITOR
	private static IndicatorDamage          		_indicator 			= null;
	private static IndicatorDamage                  _indicatorPreview 	= null;
#endif
	public bool[]                                   indicatorsActive;
	public IndicatorDamage[]						indicators;
	public IndicatorDamage 							indicatorAdd;
	public bool										onRenderObject 		= false;

	private delegate void DrawFunc ( IndicatorDamageData i_data, float i_distance, float i_scale );
	private DrawFunc _drawFunc;

#if UNITY_EDITOR
	public int                          			diIndex;
	public bool 									drawPreview = false;
#endif

	//-------------------------------------------------------------------------
	public static Transform FindDeepChild( Transform aParent, string aName)
	{
		var result = aParent.Find(aName);

		if ( result != null )
			return result;

		foreach( Transform child in aParent )
		{
			result = FindDeepChild( child, aName );

			if ( result != null )
				return result;
		}
		return null;
	}

	//-------------------------------------------------------------------------
	public static void SetIndicatorAngle ( float i_angle )
	{
		_dll.indicatorAngle = i_angle;
	}

#if UNITY_EDITOR
	public static void SetPreviewIndicator ( IndicatorDamage i_indicator )
	{
		_indicator = i_indicator;
	}
#endif

#if UNITY_EDITOR
	//============================================================
	void OnDisable()
	{
		_indicatorPreview = null;
	}
#endif

	//============================================================
	void OnEnable()
	{
		int loop;

		Init();

#if UNITY_EDITOR
		IndicatorPreview.Init();
		_indicatorPreview = Resources.Load<IndicatorDamage>("AnglePreview");
		_indicatorPreview.Create();
#endif

		if ( audioOn && Application.isPlaying )
			IndicatorAudio.Init( audioVolumeMin, audioVolumeMax, audioChannels );

		if ( _dict == null)
		{
			_dict = new Dictionary <string, IndicatorDamage>();

			for ( loop = 0; loop < indicators.Length; loop++ )
			{
				if ( indicators[loop] != null )
				{
					indicators[loop].Create();
					_dict.Add ( indicators[loop].name, indicators[loop] );
				}
			}
		}


		_DamageIndicators 	= _dict.Values.ToArray();
//		_dict.CopyTo(_DamageIndicators, 0);
	}

	//============================================================
	public static int GetIndicatorID ( string i_name )
	{
		for ( int loop = 0; loop < _DamageIndicators.Length; loop++ )
		{
			if ( _DamageIndicators[loop].name == i_name )
			{
				return ( loop );
			}
		}

		return ( -1 );
	}

	//============================================================
	public static DamageIndicator Activate ( string i_key, Vector3 i_source, float i_strength = 1.0f )
	{
		if ( _dict.ContainsKey(i_key) )
		{
			IndicatorDamage id = _dict[i_key];

#if ECHO_USE_SOURCE
			if ( id.enabled )
#endif
				return ( _dict[i_key].Activate ( i_source, i_strength ) );
		}

		return(null);
	}

	//============================================================
	public static DamageIndicator Activate ( string i_key, Transform i_source )
	{
		if ( _dict.ContainsKey(i_key) )
		{
			IndicatorDamage id = _dict[i_key];

#if ECHO_USE_SOURCE
			if ( id.enabled )
#endif
				return ( _dict[i_key].Activate ( i_source ) );
		}

		return(null);
	}

	//============================================================
	public static void Deactivate ( DamageIndicator i_di )
	{
		if ( i_di != null )
		{
			i_di.ending = true;
		}
	}

	//============================================================
	public static void SetAngle ( int i_id, float i_angle )
	{
		_DamageIndicators[i_id].SetAngle ( i_angle );
	}

	//============================================================
	public static void SetAngle ( string i_key, float i_angle )
	{
		_dict[i_key].SetAngle ( i_angle );
	}

	//============================================================
	void Start()
	{
		if ( onRenderObject )
			_drawFunc = IndicatorProDLL.Draw3D;
		else
			_drawFunc = IndicatorProDLL.Draw3DUpdate;

		_echoBend_ID 			= Shader.PropertyToID ( "_echoBend" );
		_echoBendCenter_ID 		= Shader.PropertyToID ( "_echoBendCenter" );
		_echoBendDistance_ID 	= Shader.PropertyToID ( "_echoBendDistance" );
	}

	//============================================================
	void RenderIndicators()
	{
		float scale;
		Vector3 bendCenter;

#if UNITY_EDITOR
		if ( indicatorCamera == null )
			Init();
#endif

		//if ( onRenderObject && indicatorCamera != Camera.current )
		//	return;

		Shader.SetGlobalFloat ( _echoBend_ID, indicatorBend );
		Shader.SetGlobalFloat ( _echoBendDistance_ID, indicatorDistance );

		bendCenter = indicatorCamera.transform.position + indicatorCamera.transform.forward * indicatorDistance;

		Shader.SetGlobalVector ( _echoBendCenter_ID, bendCenter );

		scale = indicatorCamera.aspect * 2.0f * indicatorScale * 0.59f;

		float clipHold = indicatorCamera.nearClipPlane;

		indicatorCamera.nearClipPlane = 0.001f;

#if UNITY_EDITOR
		if ( Application.isPlaying )
		{
			for ( int loop = 0; loop < indicators.Length; loop++ )
			{
				if ( indicators[loop] != null )
					_drawFunc ( indicators[loop].data, indicatorDistance, scale );
			}
		}
		else
		{
			if ( indicatorPreview )
			{
				if ( _indicatorPreview == null )
				{
					_indicatorPreview = Resources.Load<IndicatorDamage>("AnglePreview");
					_indicatorPreview.Create();
				}

				if ( _indicatorPreview != null )
				{
					Init();

					if ( _indicatorPreview.data.hitMaterial == null || _indicatorPreview.data.mesh == null )
						_indicatorPreview.Create();

					if ( onRenderObject )
						IndicatorProDLL.DrawPreviewAngle ( _indicatorPreview.data, indicatorDistance, scale );
					else
					{
						IndicatorProDLL.DrawPreviewAngleUpdate ( _indicatorPreview.data, indicatorDistance, scale );
					}
				}
			}

			if ( _indicator )
			{
				if ( _indicator.data.hitMaterial == null || _indicator.data.mesh == null )
				{
					_indicator.Create();
				}

				if (onRenderObject)
					IndicatorProDLL.DrawPreview ( _indicator.data, indicatorDistance, scale );
				else
					IndicatorProDLL.DrawPreviewUpdate ( _indicator.data, indicatorDistance, scale );
			}
		}
#else
		for ( int loop = 0; loop < indicators.Length; loop++ )
			_drawFunc ( indicators[loop].data, indicatorDistance, scale );
#endif

		indicatorCamera.nearClipPlane = clipHold;
	}

	//============================================================
	void OnRenderObject()
	{
		if ( onRenderObject )
			RenderIndicators();
	}

	void LateUpdate()
	{
		if ( !onRenderObject )
		{
			RenderIndicators();
		}
	}

#if UNITY_EDITOR
	//============================================================
	void OnGUI()
	{
		if ( _indicator && !Application.isPlaying )
		{
			if ( indicatorCamera == null )
				Init();

			if ( indicatorCamera != null && _indicator.data.customMesh == null )
				IndicatorPreview.Draw ( _indicator.data, indicatorCamera );
		}
	}

#endif

}
