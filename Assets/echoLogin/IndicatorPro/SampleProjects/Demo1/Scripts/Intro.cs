using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Intro : MonoBehaviour
{
	private GameObject _fadeGO;
	private GameObject _text1GO;
	private GameObject _text2GO;
	private Image _fade;
	private Image _text1;
	private Image _text2;
	private float _time;

	public  AnimationCurve fadeText1;
	public  AnimationCurve fadeText2;
	public  AnimationCurve fadeBackground;
	public  float          duration = 4;

	void Start ()
	{
		_fadeGO 	= IndicatorProManager.FindDeepChild( transform, "FadeBlack").gameObject;
		_text1GO 	= IndicatorProManager.FindDeepChild( transform, "Intro1").gameObject;
		_text2GO 	= IndicatorProManager.FindDeepChild( transform, "Intro2").gameObject;

		_fade	= _fadeGO.GetComponent<Image>();
   		_text1  = _text1GO.GetComponent<Image>();
		_text2  = _text2GO.GetComponent<Image>();

		_time = 0;
	}

	void Update ()
	{
		float per;
		float fadeVal;
		float text1Val;
		float text2Val;

		per = _time / duration;

		fadeVal = fadeBackground.Evaluate ( per );
		text1Val = fadeText1.Evaluate ( per );
		text2Val = fadeText2.Evaluate ( per );

		_fade.color = new Color ( 0,0,0, fadeVal );
		_text1.color = new Color ( text1Val, text1Val, text1Val, text1Val );
		_text2.color = new Color ( text2Val, text2Val, text2Val, text2Val );

		_time += Time.deltaTime;

		if ( per > 1.0f )
		{
			gameObject.SetActive ( false );
		}
	}
}
