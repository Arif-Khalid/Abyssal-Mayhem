#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
//using System.Numerics;
using IndicatorPro;

public class IndicatorPreview
{
	private static Vector2 	_aspectScale;
	private static Material _lineMaterial 	= null;
	private static float 	_radius 		= 0;
	private static float 	_offsetU 		= 0;
	private static float 	_offsetV 		= 0;
	private static float[] 	_width  		= new float[2];
	private static float[] 	_length  		= new float[2];
	private static float[] 	_scale  		= new float[2];
	private static float 	_segmentsU;
	private static float 	_segmentsV;

	//===========================================================================================================
	public static void Init ()
	{
		_lineMaterial = new Material ( Shader.Find( "Hidden/IndicatorProMesh" ) );
		_lineMaterial.hideFlags = HideFlags.HideAndDontSave;
		_lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
	}

	//===========================================================================================================
	public static void DisplayWireframe( float iradius, float ioffU, float ioffV, int isegmentsU, float ilength1, float iwidth1, float i_scale1, float ilength2, float iwidth2, float i_scale2 )
	{
		_radius 		= iradius * 0.5f;
		_offsetU 		= ( ioffU + 1.0f ) * 0.5f;
		_offsetV 		= ( ioffV + 1.0f ) * 0.5f;
		_segmentsU 		= isegmentsU;
		_segmentsV 		= 1;

		_length[0]      = ilength1;
		_width[0] 		= iwidth1;
		_scale[0]       = i_scale1;

		_length[1]      = ilength2;
		_width[1] 		= iwidth2;
		_scale[1]       = i_scale2;
	}

	//===========================================================================================================
	private static void DrawGLLine(Vector3 start, Vector3 end, Color icolor )
	{
		//Matrix4x4 matx = Matrix4x4.Ortho ( -Camera.main.aspect, Camera.main.aspect, -1.0f, 1.0f, 0.01f, 100.0f );

		Vector3 v1;
		Vector3 v2;
		Vector3 v3;
		Vector3 v4;
		Vector3 perpendicular = (new Vector3(end.y, start.x, end.z) - new Vector3(start.y, end.x, end.z)).normalized * 0.0014f;
		Color acolor = icolor;

//		acolor.a = 0.1f;
		acolor *= 0.6f;
				
		v1 = start - perpendicular;
		v2 = start + perpendicular;
		v3 = end + perpendicular;
		v4 = end - perpendicular;
				 
	    GL.Begin(GL.QUADS);
		GL.Color(acolor);
		GL.Vertex(v1);
		GL.Vertex(v2);
		GL.Vertex(v3);
		GL.Vertex(v4);
		GL.End();

		GL.Begin(GL.LINES);
		GL.Color(icolor);
		GL.Vertex(start);
		GL.Vertex(end);
		GL.End();

	}

	//============================================================
	private static Vector3 GetCirclePoint ( float radius, float angle )
	{
		Vector3 vec2 = new Vector3(0,0,0.005f);

		vec2.x = Mathf.Cos ( angle ) * radius;
		vec2.y = Mathf.Sin ( angle ) * radius;

		//aspect
		vec2.x *= _aspectScale.x;
		vec2.y *= _aspectScale.y;

		// translate
		vec2.x += _offsetU;
		vec2.y += _offsetV;

		return ( vec2 );
	}

	//===========================================================================================================
	private static void DrawHUDSize( int index )
	{
		int loopu;
		int loopv;
		Vector3 pa1;
		Vector3 pa2;
		Vector3 pb1;
		Vector3 pb2;
		float angleAdd;
		float angleAdd2;
		float curAngle;
		float curAngle2;
		float curWidth;
		float addWidth;
		Color curColor;

		if ( index == 0 )
			curColor 		= new Color ( 0.0f, 0.8f, 1.0f, 1.0f );
		else
			curColor 		= new Color ( 1.0f, 0.0f, 0.0f, 0.6f );

		angleAdd = ( ( Mathf.PI * 2.0f ) * _length[index] ) / _segmentsU;
		angleAdd2 = ( ( Mathf.PI * 2.0f ) * _length[index] * _scale[index] ) / _segmentsU;

		curAngle = - ( ( Mathf.PI * 2.0f ) * _length[index] ) / 2.0f;
		curAngle += Mathf.Deg2Rad * 90.0f;

		curAngle2 = - ( ( Mathf.PI * 2.0f ) * _length[index] * _scale[index] ) / 2.0f;
		curAngle2 += Mathf.Deg2Rad * 90.0f;

		for ( loopu = 0; loopu < _segmentsU; loopu++ )
		{
			curWidth = 0;
			addWidth = _width[index] / _segmentsV;

			for ( loopv = 0; loopv < _segmentsV; loopv++ )
			{
				pa1 = GetCirclePoint ( _radius + curWidth, curAngle );
				pa2 = GetCirclePoint ( _radius + curWidth, curAngle+angleAdd );

				DrawGLLine ( pa1, pa2, curColor );

				pb1 = GetCirclePoint ( _radius + curWidth + addWidth, curAngle2 );
				pb2 = GetCirclePoint ( _radius + curWidth + addWidth, curAngle2+angleAdd2 );

				DrawGLLine ( pb1, pb2, curColor );
				DrawGLLine ( pa1, pb1, curColor );

				if ( loopu >= _segmentsU-1 )
					DrawGLLine ( pa2, pb2, curColor );

				curWidth += addWidth;
			}

			curAngle += angleAdd;
			curAngle2 += angleAdd2;

		}
	}

	//===========================================================================================================
	private static void DrawHUDPlacement ( float iradiusAdd = 0.0f )
	{
		int 	loop;
		float 	angleAdd;
		Vector3 p1;
		Vector3 p2;
		float 	curAngle;
		Color 	colorCircle	= new Color ( 1.0f, 1.0f, 1.0f, 0.1f );

		angleAdd = ( Mathf.PI * 2.0f ) / 64.0f;

		curAngle = 0;

		for ( loop = 0; loop < 64.0f; loop++ )
		{
			p1 = GetCirclePoint ( _radius, curAngle );
			p2 = GetCirclePoint ( _radius, curAngle+angleAdd );

			DrawGLLine ( p1, p2, colorCircle );

			p1 = GetCirclePoint ( _radius + iradiusAdd , curAngle );
			p2 = GetCirclePoint ( _radius + iradiusAdd, curAngle+angleAdd );

			curAngle += angleAdd;

			DrawGLLine ( p1, p2, colorCircle );
		}
	}

	/*
	//===========================================================================================================
	void OnApplicationQuit()
	{
		_mode = 0;
	}
	*/

	//===========================================================================================================
	public static void Draw( IndicatorDamageData i_data, Camera i_camera )
	{
		//if (!Selection.Contains (i_go))
		//	return;

		if ( i_data == null )
			return;

	   // if ( Application.isPlaying )
	   // 	return;

		if ( Screen.width < Screen.height )
			_aspectScale = new Vector2 ( (float)Screen.height / (float)Screen.width, 1 );
		else
			_aspectScale = new Vector2 ( 1, i_camera.aspect );

		#if UNITY_EDITOR
			if ( _lineMaterial == null)
			{
				IndicatorPreview.Init();
			}
		#endif

		_lineMaterial.SetPass(0);

		GL.PushMatrix();
		GL.LoadOrtho();

		DrawHUDPlacement( _width[0] );

		if ( i_data.meshCount > 1 )
			DrawHUDSize( 1 );

		DrawHUDSize( 0 );

		GL.PopMatrix();

	}
}
#endif
