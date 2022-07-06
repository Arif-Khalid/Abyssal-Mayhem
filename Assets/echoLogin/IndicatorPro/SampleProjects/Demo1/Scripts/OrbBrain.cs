using UnityEngine;
using System.Collections;

public class OrbBrain : MonoBehaviour
{
	public PortalControl shootScript;
	Transform targetTransform = null;
	float speed;

	//===========================================================================
	void OnTriggerEnter ( Collider col )
	{
		if ( col.name == "Player" || col.name == "WorldPrefab")
		{
			gameObject.SetActive ( false );
			shootScript.orbPool.Deactivate ( this );
		}
	}

	//=========================================================================
	// Sets up this object to start moving towards a target object
	//=========================================================================
	public void Shoot ( Vector3 istartpos, Transform itargettransform, float ispeed )
	{
		Vector3 direction = ( itargettransform.position - istartpos );

		gameObject.SetActive ( true );
		transform.position		= istartpos;
		transform.rotation		= Quaternion.LookRotation ( direction );
		speed					= ispeed;
		targetTransform			= itargettransform;
	}

	//=========================================================================
	// makes cube home-in on target
	//=========================================================================
	void Update ()
	{
		Quaternion qRotation;

		if ( targetTransform != null )
		{
			transform.Translate ( Vector3.forward * Time.deltaTime * speed );

			qRotation = Quaternion.LookRotation ( targetTransform.position - transform.position );

			transform.rotation = Quaternion.Slerp ( transform.rotation, qRotation, Time.deltaTime * 8.0f );
		}
	}
}
