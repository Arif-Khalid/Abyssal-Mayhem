using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotSniper : MonoBehaviour
{
	private RaycastHit[] _results = new RaycastHit[6];

	private GameObject 		_player;
	private GameObject 		_robot;
	private float           _time2Shoot;
	private GameObject      _flash;
	private GameObject      _shootPoint;
	private float           _flashOn;
	private AudioSource     _audioGun1;
	private float           _shootModeTime;
	private float           _shootPauseTime;
	private Quaternion  	_rotation;
	public float 			shotInterval = 1;
	public float            shootModeTime = 2;
	public float            shootPauseTime = 3;

	void Start ()
	{
		_player = GameObject.Find("Camera");
		_robot =  GameObject.Find("Robot");

		_flash = IndicatorProManager.FindDeepChild( transform, "Flash1").gameObject;
		_shootPoint = IndicatorProManager.FindDeepChild( transform, "ShootPoint1").gameObject;

		_audioGun1 =_shootPoint.GetComponent<AudioSource>();


		_flash.SetActive(false);

		_time2Shoot = 0.0f;
		_flashOn = 0;
		_shootPauseTime = shootPauseTime;
		_shootModeTime = 0;

	}

	void TurnTo ( Transform i_target )
	{
		_rotation = Quaternion.LookRotation ( i_target.position - transform.position );
		transform.rotation = Quaternion.Slerp ( transform.rotation, _rotation, Time.deltaTime * 8 );
	}

	void Update ()
	{
		int loop;
		int count;

		if ( _shootModeTime > 0.0f )
		{
			TurnTo( _player.transform );

			_shootModeTime-=Time.deltaTime;

			if (_shootModeTime < 0.0 )
			{
				_shootPauseTime = shootPauseTime;
			}

			_time2Shoot-= Time.deltaTime;

			if ( _time2Shoot <= 0.0f )
			{
				_flashOn = 0.1f;
				_flash.SetActive(true);
				_audioGun1.Play();

				_time2Shoot = shotInterval;

				count = Physics.RaycastNonAlloc( _shootPoint.transform.position, _shootPoint.transform.forward, _results, 64.0f, ~0 );

				for ( loop = 0; loop < count; loop++ )
				{
					if ( _results[loop].transform.name == "Player")
					{
						float strength = Random.Range ( 0.25f, 1.0f );

						IndicatorProManager.Activate ( "SniperShot", _shootPoint.transform.position, strength );
						break;
					}
				}
			}
		}
		else
		{
			TurnTo ( _robot.transform );

			_shootPauseTime -= Time.deltaTime;

			if ( _shootPauseTime < 0.0f )
			{
				_shootModeTime = shootModeTime;
			}
		}

		if ( _flashOn > 0.0f )
		{
			_flashOn -= Time.deltaTime;

			if ( _flashOn < 0.0f )
			{
				_flash.SetActive(false);
			}
		}
	}
}
