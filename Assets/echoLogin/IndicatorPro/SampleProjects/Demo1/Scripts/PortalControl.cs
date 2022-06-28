using UnityEngine;
using System.Collections;
using IndicatorPro;

public class PortalControl : MonoBehaviour
{
	public EchoList<OrbBrain>   orbPool;
	private GameObject   		_targetObject;
	private GameObject   		_pivot;
	private float               _shotNext = 0;
	public OrbBrain 			shotPrefab;
	public float                shotSpeed = 12;
	public int                  portalNumber = 0;

	void OnTriggerStay ( Collider col )
	{
		if ( col.name == "Player" && _shotNext <= 0 )
		{
			OrbBrain go = orbPool.GetFree();
			_shotNext = 2;

			 if ( go != null )
			 {
				 go.Shoot ( _pivot.transform.position, _targetObject.transform, shotSpeed );
			 }
		}
	}

	//================================================================================
	void Start ()
	{
		OrbBrain go;
		BoxCollider bc;
		Vector3 tmp;
		string portalName;

		_targetObject = GameObject.Find("Player");

		portalName = "Portal";

		if ( portalNumber > 0 )
		{
			portalName += portalNumber;
		}

		_pivot = IndicatorProManager.FindDeepChild ( transform, portalName ).gameObject;

		bc =  gameObject.AddComponent<BoxCollider>();

		tmp = new Vector3 ( 0, 0, -18 );
		bc.center = tmp;

		tmp = new Vector3 ( 5, 5, 15 );
		bc.size= tmp;

		bc.isTrigger = true;

		if (shotPrefab)
		{
			orbPool = new EchoList<OrbBrain>();

			for ( int loop = 0; loop < 2; loop++ )
			{
				go				= UnityEngine.Object.Instantiate ( shotPrefab ) as OrbBrain;
				go.name 		= shotPrefab.name;
				go.shootScript 	= this;
				orbPool.AddNewItem ( go );
			}
		}
	}

	//================================================================================
	void Update ()
	{
		if ( _shotNext > 0 )
			_shotNext -= Time.deltaTime;
	}
}
