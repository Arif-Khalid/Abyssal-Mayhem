using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using System;
using System.IO;
using IndicatorPro;

[System.Serializable]
[CustomEditor (typeof(IndicatorProManager))]
public class IndicatorProManagerGUI : Editor
{
	public static Color  		buttonColor 		= new Color ( 1.0f, 0.2f, 0.2f, 2.0f );
	public SerializedProperty 	indicators;
	public SerializedProperty 	indicatorAdd;
	public SerializedProperty 	indicatorDistance;
	public SerializedProperty 	indicatorScale;
	public SerializedProperty 	indicatorBend;
	public SerializedProperty 	indicatorAngle;
	public SerializedProperty 	indicatorPreview;
	public SerializedProperty 	audioVolumeMin;
	public SerializedProperty 	audioVolumeMax;
	public SerializedProperty 	audioChannels;
	public SerializedProperty 	audioDistance;
	public SerializedProperty 	audioOn;
	public SerializedProperty 	camera;
	public SerializedProperty 	onRenderObject;

	//=========================================================================
	void OnEnable()
	{
		indicators 			= serializedObject.FindProperty("indicators");
		indicatorAdd 		= serializedObject.FindProperty("indicatorAdd");
		indicatorDistance 	= serializedObject.FindProperty("indicatorDistance");
		indicatorScale 		= serializedObject.FindProperty("indicatorScale");
		indicatorBend 		= serializedObject.FindProperty("indicatorBend");
		indicatorAngle 		= serializedObject.FindProperty("indicatorAngle");
		indicatorPreview	= serializedObject.FindProperty("indicatorPreview");

		audioVolumeMin 		= serializedObject.FindProperty("audioVolumeMin");
		audioVolumeMax 		= serializedObject.FindProperty("audioVolumeMax");
		audioChannels  		= serializedObject.FindProperty("audioChannels");
		audioDistance  		= serializedObject.FindProperty("audioDistance");
		audioOn 	 		= serializedObject.FindProperty("audioOn");

		camera 	 			= serializedObject.FindProperty("indicatorCamera");
		onRenderObject 	 	= serializedObject.FindProperty("onRenderObject");

		if ( indicators.arraySize < 1 )
		{
			indicators.InsertArrayElementAtIndex ( 0 );
			indicators.GetArrayElementAtIndex ( 0 ).objectReferenceValue = null;
			serializedObject.ApplyModifiedProperties();
		}
	}

	//============================================================
	public static void Line( int i_height = 1 )
	{
		Rect rect = EditorGUILayout.GetControlRect(false, i_height + 1);
		rect.height = i_height;
		EditorGUI.DrawRect(rect, new Color ( 0.5f,0.5f,0.5f, 1 ) );
	}

	//=========================================================================
	public void MinMaxSlider ( string ilabel, SerializedProperty i_min, SerializedProperty i_max, float imin, float imax, string itooltip = "" )
	{
		float min = i_min.floatValue;
		float max = i_max.floatValue;

		EditorGUILayout.MinMaxSlider ( new GUIContent (ilabel, itooltip ), ref min, ref max, imin, imax );

		i_min.floatValue = min;
		i_max.floatValue = max;
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
	public override void OnInspectorGUI()
	{
		int loop;
		Color old_color = GUI.color;
		SerializedProperty sp;

		serializedObject.Update();

		Event current = Event.current;

		if ( current.type == EventType.MouseUp || current.type == EventType.KeyUp )
		{
			UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
		}

		camera.objectReferenceValue = EditorGUILayout.ObjectField ( "Camera", camera.objectReferenceValue, typeof(Camera), true );

		EditorGUILayout.Space();
		EditorGUILayout.LabelField ( "Damage Indicator Options:" );
		Line();

		onRenderObject.boolValue = EditorGUILayout.Toggle ( new GUIContent ( "OnRenderObject", "ON = Render After Scene using DrawMeshNow\nOFF = Render Scene in LateUpdate using DrawMesh.\nIf using SRP leave OFF" ), onRenderObject.boolValue );

		indicatorPreview.boolValue = EditorGUILayout.Toggle ( new GUIContent ( "Preview", "Show Grid Preview" ), indicatorPreview.boolValue );
		Slider ( "Angle", indicatorAngle, 0.0f, 90.0f, "Tilt on X Axis" );
		Slider ( "Distance", indicatorDistance, 0.1f, 16.0f, "distance from camera" );
		Slider ( "Scale", indicatorScale, 0.1f, 16.0f, "Scale indicators" );
		Slider ( "Curve", indicatorBend, -2.0f, 2.0f, "Bend Amount" );

		EditorGUILayout.Space();
		EditorGUILayout.LabelField ( "Audio:" );
		Line();
		audioOn.boolValue = EditorGUILayout.Toggle ( new GUIContent ( "Audio On", "Turn on/off built in Audio" ), audioOn.boolValue );

		if ( audioOn.boolValue )
		{
//			audioTarget.objectReferenceValue = EditorGUILayout.ObjectField ("Target", audioTarget.objectReferenceValue, typeof(Transform), false );
			Slider ( "Distance", audioDistance, 0.0f, 4.0f, "how far sound hit is from player" );
			MinMaxSlider ( "Volume",audioVolumeMin,audioVolumeMax, 0.0f, 1.0f, "Set volume min max for strength" );
			Slider ( "Channels", audioChannels, 1, 8, "how many simultaneous sounds" );
			EditorGUILayout.Space();
		}

		EditorGUILayout.Space();
		EditorGUILayout.LabelField ( "Damage Indicators" );
		Line();

		if ( indicators.arraySize >= 0 )
		{
			for ( loop = 0; loop < indicators.arraySize; loop++ )
			{
				GUILayout.BeginHorizontal();
				sp = indicators.GetArrayElementAtIndex(loop);

				EditorGUILayout.PropertyField ( sp, new GUIContent ( "", "" ) );

				if ( indicators.arraySize > 1 )
				{

#if ECHO_DEBUG
					if (sp!=null && sp.objectReferenceValue != null )
						((IndicatorDamage)sp.objectReferenceValue).enabled = EditorGUILayout.Toggle ( new GUIContent ( "", "Enabled/Disable" ), ((IndicatorDamage)sp.objectReferenceValue).enabled, GUILayout.Width(24) );
#endif

					if ( GUILayout.Button ( new GUIContent( "-", "Remove this Indicator"), GUILayout.Width(24) ) )
					{
						if ( EditorUtility.DisplayDialog("Remove This Indicator ?", "Are you sure ?", "Yes", "No" ) )
						{
							if ( sp.objectReferenceValue != null )
								((IndicatorDamage)sp.objectReferenceValue).enabled = true;

							sp.objectReferenceValue = null;

							indicators.DeleteArrayElementAtIndex ( loop );
							serializedObject.ApplyModifiedProperties();
							break;
						}
					}
				}

				GUILayout.EndHorizontal();
			}
		}
		else
		{
			EditorGUILayout.Space();
			EditorGUILayout.LabelField ( "No Damage Indicators:" );
			EditorGUILayout.Space();
		}

		EditorGUILayout.Space();
		if ( GUILayout.Button ( new GUIContent( "Add Indicator Slot") ) )
		{
			int pos = indicators.arraySize;

			indicators.InsertArrayElementAtIndex ( pos );

			indicators.GetArrayElementAtIndex ( pos ).objectReferenceValue = null;

			serializedObject.ApplyModifiedProperties();
		}
		EditorGUILayout.Space();

		//buttonColor = EditorGUILayout.ColorField ( "Color", buttonColor );

		serializedObject.ApplyModifiedProperties();

	}
}
