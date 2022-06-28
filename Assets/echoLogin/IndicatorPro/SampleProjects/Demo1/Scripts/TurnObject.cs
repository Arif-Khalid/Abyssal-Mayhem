using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnObject : MonoBehaviour
{
	public float 	speed 	= 8;
	public Vector3 	axis 	= new Vector3(0,1,1);

	// Use this for initialization
	void Start ()
	{

	}

	// Update is called once per frame
	void Update ()
	{
		transform.Rotate(axis * Time.deltaTime * speed, Space.Self);
	}
}
