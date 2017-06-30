using UnityEngine;
using System.Collections;
using System;

/// <summary>
///		Behaviour of the camera following the player
/// </summary>
public class CameraBehaviour : MonoBehaviour {


	public static CameraBehaviour Instance { get; private set; }

	public System.Action EventEndSwitchCar;



	public Vector3 OffsetFromPlayer;
	public Vector3 PositionPowerTransition
	{
		get
		{
			return OffsetFromPlayer + _trans.position;
		}
	}

	
	private Transform _trans;
	private IEnumerator _fctSwitch;
	private IEnumerator _fctBehaviour;

	public void Awake()
	{
		Instance = this;
		_trans = transform;

	}

	public void Init(CameraConfig CConfig)
	{
		_fctBehaviour = Behaviour();
	}

	public IEnumerator Behaviour()
	{
		var fctFollow = FollowPlayer();

		while (true)
		{
			while (_fctSwitch == null)
			{
				fctFollow.MoveNext();
				yield return null;
			}
			while (_fctSwitch.MoveNext())
			{
				yield return null;
			}
			_fctSwitch = null;
		}
	}

	public void SwitchTargetCam(Transform NextCar, float TimeSwitch, System.Action FuncAtEnd)
	{
		_fctSwitch = SwitchCarCam(NextCar, TimeSwitch, FuncAtEnd);
	}

	private IEnumerator FollowPlayer()
	{
		while (PlayerManager.Instance == null)
			yield return null;

		while (true)
		{
			_trans.position = 
				new Vector3(_trans.position.x + OffsetFromPlayer.x,
				OffsetFromPlayer.y,
				PlayerManager.Instance.PositionPlayer.z + OffsetFromPlayer.z);
			yield return new WaitForEndOfFrame();
		}
	}

	public void ApplyBehaviour()
	{
		_fctBehaviour.MoveNext();
	}

	/// <summary>
	///		Transition beetween current body's Player and the next
	/// </summary>
	/// <param name="NextCar"></param>
	/// <param name="TimeSwitch"></param>
	/// <param name="FuncAtEnd"></param>
	/// <returns></returns>
	private IEnumerator SwitchCarCam(Transform NextCar, float TimeSwitch, System.Action FuncAtEnd)
	{
		PlayerManager.Instance.PowerTransition = true;
		Vector3 BegPos = 
			new Vector3(_trans.position.x + OffsetFromPlayer.x,
			OffsetFromPlayer.y,
			PlayerManager.Instance.PositionPlayer.z + OffsetFromPlayer.z);
		Vector3 EndPos;


		for (float t = 0f; t < TimeSwitch; t += Time.unscaledDeltaTime)
		{
			//Debug.LogError(t);
			EndPos = new Vector3(_trans.position.x + OffsetFromPlayer.x, OffsetFromPlayer.y, NextCar.position.z + OffsetFromPlayer.z);
			_trans.position = Vector3.Lerp(BegPos, EndPos, Mathf.Clamp01(t / TimeSwitch));
			do
			{
				yield return null;
			}while (UIGameplayManager.Instance.IsPaused);
		}

		if (EventEndSwitchCar != null)
			EventEndSwitchCar.Invoke();
		if (FuncAtEnd != null)
			FuncAtEnd.Invoke();
	}
}
