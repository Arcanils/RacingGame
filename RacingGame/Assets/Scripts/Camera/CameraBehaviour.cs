using UnityEngine;
using System.Collections;

public class CameraBehaviour : MonoBehaviour {


	public static CameraBehaviour Instance { get; private set; }

	public Vector3 OffsetFromPlayer;

	public System.Action EventEndSwitchCar;


	private Camera _cam;
	private Transform _trans;
	private IEnumerator _fctSwitch;
	public void Awake()
	{
		Instance = this;
		_cam = GetComponent<Camera>();
		_trans = transform;

	}

	public void Init(CameraConfig CConfig)
	{
		StartCoroutine(Behaviour());
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
		while (PlayerManager.Instance == null || PlayerManager.Instance.TransPlayer == null)
			yield return null;

		while (true)
		{
			_trans.position = new Vector3(_trans.position.x + OffsetFromPlayer.x, OffsetFromPlayer.y, PlayerManager.Instance.TransPlayer.position.z + OffsetFromPlayer.z);
			yield return new WaitForEndOfFrame();
		}
	}
	private IEnumerator SwitchCarCam(Transform NextCar, float TimeSwitch, System.Action FuncAtEnd)
	{
		Vector3 BegPos = new Vector3(_trans.position.x + OffsetFromPlayer.x, OffsetFromPlayer.y, PlayerManager.Instance.TransPlayer.position.z + OffsetFromPlayer.z);
		Vector3 EndPos;

		Debug.LogError(TimeSwitch);

		for (float t = 0f; t < TimeSwitch; t += Time.unscaledDeltaTime)
		{
			Debug.LogError(t);
			EndPos = new Vector3(_trans.position.x + OffsetFromPlayer.x, OffsetFromPlayer.y, NextCar.position.z + OffsetFromPlayer.z);
			_trans.position = Vector3.Lerp(BegPos, EndPos, Mathf.Clamp01(t / TimeSwitch));
			yield return null;
		}

		if (EventEndSwitchCar != null)
			EventEndSwitchCar.Invoke();
		if (FuncAtEnd != null)
			FuncAtEnd.Invoke();
	}
}
