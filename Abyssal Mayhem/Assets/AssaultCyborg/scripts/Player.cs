using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	public Animator anim;
	private float inputH;
	private float inputV;
	public Rigidbody rbody;
	private bool run;

	// Use this for initialization
	void Start () 
	{
		anim = GetComponent<Animator>();
		run = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown("z"))
		{
			anim.Play("AimingCrouching", -1, 0f);
			rbody = GetComponent<Rigidbody>();

		}

		if (Input.GetKey(KeyCode.LeftShift))
		{
			run = true;
		}
		else
		{
			run = false;	
		}

		inputH = Input.GetAxis ("Horizontal");
		inputV = Input.GetAxis ("Vertical");

		anim.SetFloat("inputH", inputH);
		anim.SetFloat("inputV", inputV);
		anim.SetBool ("run",run);

		float moveX = inputH*20f*Time.deltaTime;
		float moveZ = inputV*50f*Time.deltaTime;

		if (moveZ <= 0f)
		{
			moveX = 0f;
		}
		else if(run)
		{
			moveX*=3f;
			moveZ*=3f;
		}

		rbody.velocity = new Vector3(moveX,0f,moveZ);
	}
}
