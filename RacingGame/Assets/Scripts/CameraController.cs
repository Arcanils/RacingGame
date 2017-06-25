using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	public static CameraController Instance { get; private set; }

    public Vector3 OffsetFromPlayer;

    private Camera _cam;
    private Transform _transPlayer;
    private Transform _trans;
	private Coroutine _routine;
    public void Awake()
    {
        _cam = GetComponent<Camera>();
        _trans = transform;
        _transPlayer = GameObject.FindGameObjectWithTag("Player").transform;
		Instance = this;

	}

    public void Start()
    {
		_routine = StartCoroutine(FollowPlayer());
    }

    private IEnumerator FollowPlayer()
    {
        while (true)
        {
            _trans.position = new Vector3(_trans.position.x + OffsetFromPlayer.x, OffsetFromPlayer.y, _transPlayer.position.z + OffsetFromPlayer.z);
            yield return null;
        }
    }
	public float TimeSwitch = 2f;
	public IEnumerator SwitchCarCam(Transform NextCar)
	{
		StopCoroutine(_routine);
		Vector3 BegPos = new Vector3(_trans.position.x + OffsetFromPlayer.x, OffsetFromPlayer.y, _transPlayer.position.z + OffsetFromPlayer.z);
		Vector3 EndPos;

		for (float t = 0f; t < TimeSwitch; t += Time.unscaledDeltaTime)
		{
			EndPos = new Vector3(_trans.position.x + OffsetFromPlayer.x, OffsetFromPlayer.y, NextCar.position.z + OffsetFromPlayer.z);
			_trans.position = Vector3.Lerp(BegPos, EndPos, Mathf.Clamp01(t / TimeSwitch));
			yield return null;
		}
		_routine = StartCoroutine(FollowPlayer());
	}
}
