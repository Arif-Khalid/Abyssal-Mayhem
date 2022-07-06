using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.IO;

namespace IndicatorPro
{
[CustomEditor (typeof(IndicatorDamage))]
public class IndicatorDamageEditor : Editor
{
	public static Color  		buttonColor 		= new Color ( 2.0f, 0.2f, 0.2f, 2.0f );

	SerializedProperty meshHQ;
	SerializedProperty radiusPercent;
	SerializedProperty widthPercent1;
	SerializedProperty lengthPercent1;
	SerializedProperty scalePercent1;
	SerializedProperty widthPercent2;
	SerializedProperty lengthPercent2;
	SerializedProperty scalePercent2;
	SerializedProperty meshCount;
	SerializedProperty segmentCountU1;
	SerializedProperty maxHitCount;
	SerializedProperty offsetU;
	SerializedProperty offsetV;
	SerializedProperty duration;
	SerializedProperty startColor;
	SerializedProperty endColor;
	SerializedProperty startAmplify;
	SerializedProperty endAmplify;
	SerializedProperty colorCurve;
	SerializedProperty alphaCurve;
	SerializedProperty moveCurve;
	SerializedProperty indicatorTex;
	SerializedProperty blendMode;
	SerializedProperty distortion;
	SerializedProperty distortionCurve;
	SerializedProperty distortionAmount1;
	SerializedProperty distortionSpeed1;
	SerializedProperty distortionStrength1;
	SerializedProperty distortionAmount2;
	SerializedProperty distortionSpeed2;
	SerializedProperty distortionStrength2;
	SerializedProperty dissolve;
	SerializedProperty dissolveCurve;
	SerializedProperty dissolveEdgeWidth;
	SerializedProperty dissolveEdgeColor;
	SerializedProperty dissolveEdgeAmplify;
	SerializedProperty customMesh;
	SerializedProperty alphaDistort;
	SerializedProperty previewAngle;
	SerializedProperty previewStrength;
	SerializedProperty previewPercent;
	SerializedProperty previewLockAlpha;
	SerializedProperty previewLockMovement;
	SerializedProperty packedTexture;
	SerializedProperty audioClip;

	SerializedProperty data;

	Rect curveRange = new Rect ( 0, 0, 1, 1);

	//============================================================
	public static void Line( int i_height = 1 )
	{
		Rect rect = EditorGUILayout.GetControlRect(false, i_height + 1);
		rect.height = i_height;
		EditorGUI.DrawRect(rect, new Color ( 0.5f,0.5f,0.5f, 1 ) );
	}

	//=========================================================================
	public void Toggle ( string ilabel, SerializedProperty isp, string itooltip = "" )
	{
		isp.boolValue = EditorGUILayout.Toggle ( new GUIContent (ilabel, itooltip ), isp.boolValue );
	}

	//=========================================================================
	public void SPInt ( string ilabel, SerializedProperty isp, string itooltip = "" )
	{
		isp.intValue = EditorGUILayout.IntField ( new GUIContent (ilabel, itooltip ), isp.intValue );

		if ( isp.intValue < 1 )
			isp.intValue = 1;
	}

	//=========================================================================
	public void MinMaxSlider ( string ilabel, SerializedProperty i_min, SerializedProperty i_max, float imin, float imax, string itooltip = "" )
	{
		float min = i_min.floatValue;
		float max = i_max.floatValue;

		EditorGUILayout.MinMaxSlider ( new GUIContent (ilabel, itooltip ), ref min, ref max, imin, imax );
	}

	//=========================================================================
	public void Slider ( string ilabel, SerializedProperty isp, float imin, float imax, string itooltip = "" )
	{
		isp.floatValue = EditorGUILayout.Slider ( new GUIContent (ilabel, itooltip ), isp.floatValue, imin, imax );
	}

	//=========================================================================
	public void Slider ( string ilabel, SerializedProperty isp, int imin, int imax, string itooltip = "" )
	{
		isp.intValue = EditorGUILayout.IntSlider ( new GUIContent (ilabel, itooltip ), isp.intValue, imin, imax );
	}

	//=========================================================================
	public SerializedProperty EnumGad ( string ilabel, SerializedProperty isp, Type ienumType, string itooltip = "" )
	{
		isp.enumValueIndex = (int)(object)EditorGUILayout.EnumPopup ( new GUIContent (ilabel, itooltip ), (Enum)Enum.GetValues(ienumType).GetValue(isp.enumValueIndex));

		return ( isp );
	}

	//=========================================================================
	public void SPCurve ( string ilabel, SerializedProperty isp, string itooltip = "" )
	{
		isp.animationCurveValue = EditorGUILayout.CurveField ( new GUIContent (ilabel, itooltip ), isp.animationCurveValue, new Color(0,1,1,1), curveRange );
	}

	//=========================================================================
	public void SPColor ( string ilabel, SerializedProperty isp, string itooltip = "" )
	{
		isp.colorValue = EditorGUILayout.ColorField ( new GUIContent (ilabel, itooltip ), isp.colorValue );
	}

	//=========================================================================
	public SerializedProperty SPTexture ( string ilabel, SerializedProperty isp, string itooltip = "" )
	{
		EditorGUILayout.PropertyField ( isp, new GUIContent (ilabel, itooltip ) );

		return ( isp );
	}

	//=========================================================================
	void OnDisable()
	{
		IndicatorProManager.SetPreviewIndicator(null);
		UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
	}

	//=========================================================================
	void OnEnable()
	{
		IndicatorProManager.SetPreviewIndicator((IndicatorDamage)target);
		UnityEditorInternal.InternalEditorUtility.RepaintAllViews();

		data				= serializedObject.FindProperty( "data" );

		meshHQ				= data.FindPropertyRelative( "meshHQ" );
		radiusPercent		= data.FindPropertyRelative( "radiusPercent" );
		widthPercent1		= data.FindPropertyRelative( "widthPercent1" );
		lengthPercent1		= data.FindPropertyRelative( "lengthPercent1" );
		scalePercent1		= data.FindPropertyRelative( "scalePercent1" );
		widthPercent2		= data.FindPropertyRelative( "widthPercent2" );
		lengthPercent2		= data.FindPropertyRelative( "lengthPercent2" );
		scalePercent2		= data.FindPropertyRelative( "scalePercent2" );
		meshCount			= data.FindPropertyRelative( "meshCount" );
		segmentCountU1		= data.FindPropertyRelative( "segmentCountU1" );
		maxHitCount			= data.FindPropertyRelative( "maxHitCount" );
		offsetU				= data.FindPropertyRelative( "offsetU" );
		offsetV				= data.FindPropertyRelative( "offsetV" );
		duration			= data.FindPropertyRelative( "duration" );
		startColor			= data.FindPropertyRelative( "startColor" );
		endColor			= data.FindPropertyRelative( "endColor" );
		startAmplify		= data.FindPropertyRelative( "startAmplify" );
		endAmplify			= data.FindPropertyRelative( "endAmplify" );
		colorCurve			= data.FindPropertyRelative( "colorCurve" );
		alphaCurve			= data.FindPropertyRelative( "alphaCurve" );
		moveCurve			= data.FindPropertyRelative( "moveCurve" );
		indicatorTex		= data.FindPropertyRelative( "indicatorTex" );
		blendMode			= data.FindPropertyRelative( "blendMode" );
		distortion			= data.FindPropertyRelative( "distortion" );
		distortionCurve		= data.FindPropertyRelative( "distortionCurve" );
		distortionAmount1	= data.FindPropertyRelative( "distortionAmount1" );
		distortionSpeed1	= data.FindPropertyRelative( "distortionSpeed1" );
		distortionStrength1	= data.FindPropertyRelative( "distortionStrength1" );
		distortionAmount2	= data.FindPropertyRelative( "distortionAmount2" );
		distortionSpeed2	= data.FindPropertyRelative( "distortionSpeed2" );
		distortionStrength2	= data.FindPropertyRelative( "distortionStrength2" );
		dissolve			= data.FindPropertyRelative( "dissolve" );
		dissolveCurve		= data.FindPropertyRelative( "dissolveCurve" );
		dissolveEdgeWidth	= data.FindPropertyRelative( "dissolveEdgeWidth" );
		dissolveEdgeColor	= data.FindPropertyRelative( "dissolveEdgeColor" );
		dissolveEdgeAmplify	= data.FindPropertyRelative( "dissolveEdgeAmplify" );
		customMesh			= data.FindPropertyRelative( "customMesh" );
		alphaDistort		= data.FindPropertyRelative( "alphaDistort" );
		previewAngle		= data.FindPropertyRelative( "previewAngle" );
		previewStrength		= data.FindPropertyRelative( "previewStrength" );
		previewPercent		= data.FindPropertyRelative( "previewPercent" );
		previewLockAlpha	= data.FindPropertyRelative( "previewLockAlpha" );
		previewLockMovement	= data.FindPropertyRelative( "previewLockMovement" );
		packedTexture		= data.FindPropertyRelative( "packedTexture" );
		audioClip        	= data.FindPropertyRelative( "audioClip" );
	}

	public override void OnInspectorGUI()
    {
        IndicatorDamage instance = (IndicatorDamage)target;
		Color oldColor = GUI.color;

		Event current = Event.current;

		serializedObject.Update();
  		if ( current.type == EventType.MouseUp || current.type == EventType.KeyUp || ( current.type == EventType.KeyDown && current.character =='r' ) )
		{
			UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
		}

		EditorGUI.BeginChangeCheck();

		audioClip.objectReferenceValue = EditorGUILayout.ObjectField ("AudioClip", audioClip.objectReferenceValue, typeof(AudioClip), false );
		EnumGad ("Blend Mode", blendMode, typeof(BLENDMODE) );
		SPInt ( "Pre Allocate Count", maxHitCount, "Number of hit objecst to pool" );


		EditorGUILayout.PropertyField( customMesh, new GUIContent ( "Custom Mesh", "Draw any custom meshes to this slot Z+ is forward to screen" ) );
		SPTexture ( "Texture", indicatorTex, "Texture used for damage indicator" );
		Toggle ( "Packed Texture", packedTexture );

		if ( packedTexture.boolValue )
		{
			GUILayout.BeginVertical("box");
			EditorGUILayout.LabelField ( "Texture Format:" );
			EditorGUILayout.LabelField ( "R: Greyscale image" );
			EditorGUILayout.LabelField ( "G: Dissolve" );
			EditorGUILayout.LabelField ( "B: Unused" );
			EditorGUILayout.LabelField ( "A: Alpha" );
			GUILayout.EndVertical();
		}
		else
		{
			GUILayout.BeginVertical("box");
			EditorGUILayout.LabelField ( "Texture Format:" );
			EditorGUILayout.LabelField ( "RGB: image" );
			EditorGUILayout.LabelField ( "A: Alpha" );
			GUILayout.EndVertical();
		}

		if ( EditorGUI.EndChangeCheck() )
		{
			serializedObject.ApplyModifiedProperties();
			instance.ResetPreview();
		}

		EditorGUI.BeginChangeCheck();

		EditorGUILayout.Space();
		EditorGUILayout.LabelField ( "Mesh:" );
		Line();
		Slider ( "Horizontal Offset", offsetU, -1.0f, 1.0f, "Position indicator center on screen, 0.0 is center" );
		Slider ( "Vertical Offset", offsetV, -1.0f, 1.0f, "Position indicator center on screen, 0.0 is center" );

		if ( customMesh.objectReferenceValue == null )
		{
			Slider ( "Radius", radiusPercent, 0.01f, 1.0f, "Percent of Screen" );
			Slider ( "Mesh Segments", segmentCountU1, 1, 32, "# of Segments wide" );
			Toggle ( "HQ Mesh", meshHQ );
			Slider ( "Strength Levels", meshCount, 1, 32, "how many meshes to make from base size to strenth size" );

			EditorGUILayout.Space();

			if ( meshCount.intValue > 1)
				EditorGUILayout.LabelField ( "Strength Minimum:" );

			Slider ( "Width", widthPercent1, 0.001f, 0.25f, "Width" );
			Slider ( "Length", lengthPercent1, 0.001f, 1.0f, "Length" );
			Slider ( "Scale", scalePercent1, 0.00f, 5.0f, "Scale Outter Part" );

			if ( meshCount.intValue > 1)
			{

				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				if ( GUILayout.Button ( new GUIContent( "Copy Min To Max") ) )
				{
					widthPercent2.floatValue  =  widthPercent1.floatValue;
					lengthPercent2.floatValue =  lengthPercent1.floatValue;
					scalePercent2.floatValue  =  scalePercent1.floatValue;
				}
				EditorGUILayout.EndHorizontal();

				EditorGUILayout.Space();
				EditorGUILayout.LabelField ( "Strength Maximum:" );
				Slider ( "Width", widthPercent2, 0.01f, 0.25f, "Width to Add ( for max Strength ) " );
				Slider ( "Length", lengthPercent2, 0.001f, 1.0f, "Length To Add ( for max Strength )" );
				Slider ( "Scale", scalePercent2, 0.00f, 5.0f, "Scale outter part" );
			}
		}

		if ( EditorGUI.EndChangeCheck() )
		{
			serializedObject.ApplyModifiedProperties();
			instance.ResetPreview();
		}

		EditorGUI.BeginChangeCheck();

		EditorGUILayout.Space();
		EditorGUILayout.LabelField ( "Time Transition:" );
		Line();
		Slider ( "Lifetime", duration, 0.001f, 8.0f, "Duration Of Time to fade" );
		SPColor ("Start Color", startColor, "Texture is mutlipled from start to end color over fadeTime" );
		Slider ( "Start Amplify", startAmplify, 0.0f, 8.0f, "Color Amplify Start" );
		SPColor ("Color End", endColor, "Texture is mutlipled from start to end color over fadeTime" );
		Slider ( "End Amplify", endAmplify, 0.0f, 8.0f, "Color Amplify End" );
		SPCurve ("Color Curve", colorCurve, "Adjust indicator color over duration" );
		SPCurve ("Alpha Curve", alphaCurve, "Adjust indicator alpha Over duration" );
		SPCurve ("Movement Curve", moveCurve, "Adjust indicator movement" );

		EditorGUILayout.Space();
		EditorGUILayout.LabelField ( "Dissolve Options:" );
		Line();
		Toggle ( "Active", dissolve );
		if ( dissolve.boolValue )
		{
			SPCurve ("Curve", dissolveCurve, "Use dissolve Curve to adjust the fade along timeline" );
			SPColor ("Edge Color", dissolveEdgeColor, "Burn/Edge color of Dissolve" );
			Slider ( "Edge Width", dissolveEdgeWidth, 0.0f, 1.0f, "Edge of Dissolve Size" );
			Slider ( "Edge Amplify", dissolveEdgeAmplify, 1.0f, 8.0f, "Amplify Egde Color" );
		}

		EditorGUILayout.Space();
		EditorGUILayout.LabelField ( "Distortion Options:" );
		Line();
		EnumGad ("Type", distortion, typeof(DISTORTION) );

		if ( (DISTORTION)distortion.enumValueIndex == DISTORTION.WAVES || (DISTORTION)distortion.enumValueIndex == DISTORTION.RIPPLE )
		{
			SPCurve ("Curve", distortionCurve, "Distortion Amount over Duration time" );

			// make max sengemnt width
			if ( (DISTORTION)distortion.enumValueIndex == DISTORTION.WAVES )
			{
				EditorGUILayout.Space();
				EditorGUILayout.LabelField ( "Horizontal:" );
			}

			Slider ( "Amount", distortionAmount1, 1.0f, 64.0f, "Number of wavs horisontally" );
			Slider ( "Speed", distortionSpeed1, 1.0f, 16.0f, "Animation Speed" );
			Slider ( "Strength", distortionStrength1, 0.0f, 1.0f, "Strength of Effect" );

		}

		if ( (DISTORTION)distortion.enumValueIndex == DISTORTION.WAVES )
		{
			EditorGUILayout.Space();
			EditorGUILayout.LabelField ( "Vertical:" );
			Slider ( "Amount", distortionAmount2, 1.0f, 64.0f, "Number of wavs horisontally" );
			Slider ( "Speed", distortionSpeed2, 1.0f, 16.0f, "Animation Speed" );
			Slider ( "Strength", distortionStrength2, 0.0f, 1.0f, "Strength of Effect" );
		}

		if ( (DISTORTION)distortion.enumValueIndex != DISTORTION.OFF )
		{
			Toggle ( "Alpha Distort", alphaDistort );
		}

		EditorGUILayout.Space();
		EditorGUILayout.LabelField ( "Preview" );
		Line();

		Slider ( "Lifetime", previewPercent, 0.0f, 1.0f, "Slide to see what indicator looks like over its duration" );
		EditorGUILayout.Space();
		Slider ( "Angle", previewAngle, 0.0f, 360.0f, "Slide to changes the angle of indicator" );
		Slider ( "Strength", previewStrength, 0.0f, 1.0f, "Slide to see the changes in size/strength" );
		Toggle ( "Lock Alpha", previewLockAlpha );
		Toggle ( "Lock Movement", previewLockMovement );
		EditorGUILayout.Space();


 		if ( GUILayout.Button ( new GUIContent( "Reset Preview") ) )
		{
			previewPercent.floatValue 		= 0.0f;
			previewAngle.floatValue 		= 270.0f;
			previewStrength.floatValue		= 0.0f;
			previewLockAlpha.boolValue 		= true;
			previewLockMovement.boolValue	= true;
		}


		if ( EditorGUI.EndChangeCheck() )
		{
			serializedObject.ApplyModifiedProperties();
			IndicatorProDLL.SetBlendMode ( instance.data );
			IndicatorProDLL.SetKeywords ( instance.data );
		}

		if ( customMesh.objectReferenceValue == null )
		{
		 IndicatorPreview.DisplayWireframe ( radiusPercent.floatValue,
								 offsetU.floatValue,
								 offsetV.floatValue,
								 segmentCountU1.intValue,
								 lengthPercent1.floatValue,
								 widthPercent1.floatValue,
								 scalePercent1.floatValue,
								 lengthPercent2.floatValue,
								 widthPercent2.floatValue,
								 scalePercent2.floatValue
								 );
		}

    }
}
}

