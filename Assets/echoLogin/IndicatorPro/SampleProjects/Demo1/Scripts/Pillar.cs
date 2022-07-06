using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : MonoBehaviour
{
	private Material _mat;

	void Start ()
	{
		_mat = GetComponent<Renderer>().material;
	}

	void Update ()
	{
		float intensity = Random.Range ( 0.7f, 1.0f );
		_mat.SetColor("_EmissionColor", new Color(1.0f,1.0f,1.0f,1.0f) * intensity);
	}
}
