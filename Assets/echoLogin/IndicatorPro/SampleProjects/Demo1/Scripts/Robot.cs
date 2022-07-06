using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ROBOT_MODE
{
	TRAVEL_NODE,
	TRAVEL_NODE_SHOOT,
	PAUSE,
	ATTACK,
}

public class Robot : MonoBehaviour
{
	private int             _tick = 0;
	private float           _coolDown   = 0;
	private bool            _attack     = false;
	private float           _attackTime = 0;
	private ROBOT_MODE  	_mode 		= ROBOT_MODE.TRAVEL_NODE;
	private int 			_curNode	= 0;
	private float 			_pauseTime  = 0;
	private GameObject 		_curTarget  = null;
	private Color           _white 	   	= new Color (1,1,1,1);
	private Color           _blue    	= new Color (0,1,1,1);
	private Color           _red   	   	= new Color (1,0,0,1);
	private Color           _eyeColor1;
	private Color           _eyeColor2;
	private Material        _mat;
	private Animation 		_anim;
	private GameObject      _flash1;
	private GameObject      _flash2;
	private GameObject      _parent;
	private GameObject      _doorObject;
	private GameObject      _shootPoint1;
	private GameObject      _shootPoint2;
	private GameObject 		_player;
	private bool            _shootMode;
	private float           _shootTime;
	private float           _shootPause;
	private bool            _doorOpen = false;
	private AudioSource     _audioGun1;
	private AudioSource     _audioGun2;
	private AudioSource     _audioDoor;
	private AudioSource     _audioSiren;
	private RaycastHit[] 	_results = new RaycastHit[6];
	public  float 			speed = 3;
	public  GameObject []	nodes = new GameObject[4];
	public  Material        mat;
	public  Material        matEye;
	public  float           shootBurstTime = 0.5f;
	public  float           shootPauseTime = 0.5f;


	//===========================================================================
	void OnTriggerEnter ( Collider col )
	{
		if ( col.name == "Player" )
		{
			if ( _mode == ROBOT_MODE.ATTACK )
			{
				_curNode 	= ( _curNode + 1 ) % nodes.Length;
				_mode 		= ROBOT_MODE.TRAVEL_NODE;
				_coolDown 	= 4.0f;
				mat.SetColor ( "_Color0", _white );
				mat.SetColor ( "_Color1", _white );
				matEye.SetColor ( "_Color0", _eyeColor1 );
				matEye.SetColor ( "_Color1", _eyeColor2 );
				_audioGun1.Stop();
				_audioGun2.Stop();

				if ( _doorOpen )
				{
					_anim.Play("DoorClose");
					_doorOpen = false;
				}

				IndicatorProManager.Activate ( "BluntForce", transform.position, 1.0f );
			}
			else
				IndicatorProManager.Activate ( "BluntForce", transform.position, 0.0f );
		}
	}

	//===========================================================================
	void Start ()
	{
		SphereCollider sc;

		_player = GameObject.Find("Player");

		// trigger collider
		sc = gameObject.AddComponent<SphereCollider>();
		sc.radius = 1.4f;
		sc.isTrigger = true;

		// player bump collider
		sc = gameObject.AddComponent<SphereCollider>();
		sc.radius = 1.0f;

		_parent = new GameObject ("RobotParent");
		_parent.transform.position = transform.position;
		transform.parent = _parent.transform;
		//transform.eulerAngles = new Vector3( 0, 0, 0 );

		_doorObject = IndicatorProManager.FindDeepChild ( transform, "Door").gameObject;
		_audioDoor =_doorObject.GetComponent<AudioSource>();

		_shootPoint1 = IndicatorProManager.FindDeepChild ( transform, "ShootPoint1").gameObject;
		_shootPoint2 = IndicatorProManager.FindDeepChild ( transform, "ShootPoint2").gameObject;

		_audioGun1 = _shootPoint1.GetComponent<AudioSource>();
		_audioGun2 = _shootPoint2.GetComponent<AudioSource>();

		_flash1 = IndicatorProManager.FindDeepChild ( transform, "Flash1").gameObject;
		_flash2 = IndicatorProManager.FindDeepChild ( transform, "Flash2").gameObject;

		_flash1.SetActive(false);
		_flash2.SetActive(false);

		_audioSiren = IndicatorProManager.FindDeepChild ( transform, "Eye").gameObject.GetComponent<AudioSource>();

		_anim = gameObject.GetComponent<Animation>();

//		Debug.Log("AnimCount = " + _anim.GetClipCount());

	  //  Vector3 direction = ( _player.transform.position - _parent.transform.position );

	//	_parent.transform.rotation	= Quaternion.LookRotation ( direction );

		_curTarget = nodes[0];

		_eyeColor1 = matEye.GetColor("_Color0");
		_eyeColor2 = matEye.GetColor("_Color1");

		mat.SetColor ( "_Color0", _white );
		mat.SetColor ( "_Color1", _white );
	}

	//===========================================================================
	void TurnOnGuns()
	{
	}

	Vector3 ShotAccuracy ( Vector3 i_forward, float i_accuracy )
	{
		Vector3 dir = i_forward;
		float spread = Mathf.Lerp ( 0.03f, 0.0f, i_accuracy );

		dir.x += Random.Range ( -spread, spread );
		dir.y += Random.Range ( -spread, spread );
		dir.z += Random.Range ( -spread, spread );

		return ( dir );
	}

	//===========================================================================
	void FireGuns()
	{
		int count;
		int loop;

		if ( !_shootMode )
		{
			_audioGun1.Stop();
			_audioGun2.Stop();
   			_flash1.SetActive(false);
   			_flash2.SetActive(false);

			_shootPause+= Time.deltaTime;

			if ( _shootPause > shootPauseTime )
			{
				_shootPause = 0;
				_shootMode = true;
				_audioGun1.Play();
				_audioGun2.Play();
			}

			return;
		}

		_shootTime += Time.deltaTime;

		if ( _shootTime > shootBurstTime )
		{
			_shootTime = 0;
			_shootMode = false;
			_audioGun1.Stop();
			_audioGun2.Stop();
		}

		if ( _tick%2 == 0 )
		{
			_flash1.SetActive(true);
			_flash2.SetActive(true);
		}
		else
		{
			_flash1.SetActive(false);
			_flash2.SetActive(false);
		}

		_tick++;

		if ( _tick%2 > 0 )
			return;

		// gun 1
		count = Physics.RaycastNonAlloc( _shootPoint1.transform.position, _shootPoint1.transform.forward, _results, 64.0f, ~0 );
   		if ( count > 0 )
		{
			for ( loop = 0; loop < count; loop++ )
			{
				if ( _results[loop].transform.name == "Player")
				{
					float strength = Random.Range ( 0.0f, 1.0f );

					IndicatorProManager.Activate ( "Shot", _shootPoint1.transform.position, strength );
					break;
				}
			}
		}


		// gun 2
		count = Physics.RaycastNonAlloc( _shootPoint2.transform.position, _shootPoint2.transform.forward, _results, 64.0f, ~0 );
   		if ( count > 0 )
		{
			for ( loop = 0; loop < count; loop++ )
			{

				if ( _results[loop].transform.name == "Player")
				{
					float strength = Random.Range ( 0.0f, 1.0f );

					IndicatorProManager.Activate ( "Shot", _shootPoint2.transform.position, strength );
					break;
				}
			}
		}
	}

	//===========================================================================
	void Update ()
	{
		Quaternion qRotation;
		float curSpeed = 0;

		if ( _coolDown > 0.0f )
		{
			_coolDown -= Time.deltaTime;
		}

		if ( _mode != ROBOT_MODE.ATTACK && _coolDown < 0.1f && Vector3.Distance ( _player.transform.position , transform.position ) < 8.5f )
		{
			_attack 	= true;
			_attackTime = Random.Range ( 0.5f, 2.0f );
			mat.SetColor ( "_Color0", _blue );
			mat.SetColor ( "_Color1", _blue );
			matEye.SetColor ( "_Color0", _blue );
			matEye.SetColor ( "_Color1", _blue );
		}

		if ( _attack )
		{
			_attackTime -= Time.deltaTime;

			if ( _attackTime <= 0.0f )
			{
				_attack = false;
				_mode 	= ROBOT_MODE.ATTACK;
				_audioSiren.Play();
			}
		}


		if ( !_attack && _mode == ROBOT_MODE.TRAVEL_NODE && Vector3.Distance ( _player.transform.position , transform.position ) < 16.5f )
		{
			_mode = ROBOT_MODE.TRAVEL_NODE_SHOOT;

			if ( _doorOpen == false )
			{
				_doorOpen = true;
				_anim.Play("DoorOpen");
				_audioDoor.Play();
			}
		}

		switch ( _mode )
		{
		case ROBOT_MODE.TRAVEL_NODE:
			_curTarget = nodes[_curNode];

			if ( Vector3.Distance ( _curTarget.transform.position , _parent.transform.position ) < 0.1f )
			{
				_mode 	= ROBOT_MODE.PAUSE;
				_pauseTime 	= 2.0f;
				curSpeed = 0;
			}
			else
			{
				curSpeed = speed;
			}
			break;

		case ROBOT_MODE.TRAVEL_NODE_SHOOT:
			FireGuns();

			_curTarget = nodes[_curNode];

			if ( Vector3.Distance ( _curTarget.transform.position , _parent.transform.position ) < 0.1f )
			{
				_mode 	= ROBOT_MODE.PAUSE;
				_pauseTime 	= 2.0f;
				curSpeed = 0;

				if ( _doorOpen )
	   			{
					_flash1.SetActive(false);
					_flash2.SetActive(false);
					_audioGun1.Stop();
					_audioGun2.Stop();
	   				_anim.Play("DoorClose");
					_audioDoor.Play();
	   				_doorOpen = false;
	   			}

			}
			else
			{
				curSpeed = speed;
			}

			// look at player
			qRotation = Quaternion.LookRotation ( _player.transform.position - _parent.transform.position );
			transform.rotation = Quaternion.Slerp ( transform.rotation, qRotation, Time.deltaTime * 8 );
			break;

		case ROBOT_MODE.PAUSE:
			_pauseTime -= Time.deltaTime;

			if ( _pauseTime <= 0.0f )
			{
				_curNode = ( _curNode + 1 ) % nodes.Length;
				_mode = ROBOT_MODE.TRAVEL_NODE;
			}

			// look ahead
			qRotation = Quaternion.LookRotation ( _parent.transform.forward );
			transform.rotation = Quaternion.Slerp ( transform.rotation, qRotation, Time.deltaTime * 8 );
			break;

		case ROBOT_MODE.ATTACK:
			mat.SetColor ( "_Color0", _red );
			mat.SetColor ( "_Color1", _red );
			matEye.SetColor ( "_Color0", _red );
			matEye.SetColor ( "_Color1", _red );

			if ( _doorOpen )
			{
				_flash1.SetActive(false);
				_flash2.SetActive(false);
				_audioGun1.Stop();
				_audioGun2.Stop();
				_anim.Play("DoorClose");
				_audioDoor.Play();
				_doorOpen = false;
			}

			_curTarget 	= _player;
			curSpeed 	= speed * 3;

			// look ahead
			qRotation = Quaternion.LookRotation ( _parent.transform.forward );
			transform.rotation = Quaternion.Slerp ( transform.rotation, qRotation, Time.deltaTime * 8 );
			break;
		}

		// move and face parent
		_parent.transform.Translate ( Vector3.forward * Time.deltaTime * curSpeed );
		qRotation = Quaternion.LookRotation ( _curTarget.transform.position - _parent.transform.position, Vector3.up );
		_parent.transform.rotation = Quaternion.Slerp ( _parent.transform.rotation, qRotation, Time.deltaTime * 3.0f );
	}
}
