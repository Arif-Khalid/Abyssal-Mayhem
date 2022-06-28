using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IndicatorPro;

public class Radiation : MonoBehaviour
{
	private DamageIndicator _di = null;
	private float _enterDist;


	void Start ()
	{
	}

	void Update ()
	{
	}

	private void OnTriggerEnter(Collider other)
    {
		if (_di == null)
			_di = IndicatorProManager.Activate( "Radiation", transform );
    }

    private void OnTriggerExit(Collider other)
    {
		IndicatorProManager.Deactivate ( _di );
		_di = null;
    }
}
