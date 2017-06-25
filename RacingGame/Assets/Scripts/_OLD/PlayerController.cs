using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	public Vector3 VecSpeed;
	public float UpSpeed = 0.05f;
	public float Endurance = 100f;
	public float EnduranceMax = 100f;
	public Text TextEndurance;

	private Transform _trans;


	public void Awake()
	{
		_trans = transform;
	}

	public void Start()
	{
		StartCoroutine(MoveControlEnum());
		Endurance = EnduranceMax;
	}

	public void Update()
	{
		if (Input.GetKeyDown(KeyCode.P) && !_modeSwitchOn)
		{
			StartCoroutine(SwitchCarEnum());
		}
	}

	public void CallPowerUp()
	{
		if (!_modeSwitchOn)
			StartCoroutine(SwitchCarEnum());
	}
	public float TimeChooseSwitch = 5.0f;

	private bool _modeSwitchOn = false;
	private IEnumerator SwitchCarEnum()
	{
		_modeSwitchOn = true;
		Time.timeScale = 0.1f;
		for (float t = 0f; t < TimeChooseSwitch; t += Time.unscaledDeltaTime)
		{
			if (Input.GetMouseButton(0))
			{
				var Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit Info;
				if (Physics.Raycast(Ray, out Info))
				{
					if (Info.transform != null && Info.transform.tag == "Ennemy")
					{
						yield return CameraController.Instance.SwitchCarCam(Info.transform);
						Info.transform.gameObject.SetActive(false);
						transform.position = Info.transform.position;
						Endurance = EnduranceMax;
						TextEndurance.text = "Endurance : " + Endurance.ToString() + "%";
						Time.timeScale = 1.0f;
						yield break;
					}
				}
			}
			yield return null;
		}
		Time.timeScale = 1.0f;
	}
	

	private IEnumerator MoveControlEnum()
	{
		float speed = 1.0f;
		Vector3 nextPosition;
		while (true)
		{
			nextPosition = Time.deltaTime * GetDirection() * speed + _trans.position;
			nextPosition = new Vector3(Mathf.Clamp(nextPosition.x, -8.75f, 8.75f), nextPosition.y, nextPosition.z);
			_trans.position = nextPosition;
			speed += UpSpeed * Time.fixedDeltaTime;
			yield return new WaitForFixedUpdate();
		}
	}

	public void Move()
	{

	}
#if UNITY_STANDALONE || UNITY_EDITOR
	public Vector3 GetDirection()
	{
		return new Vector3(Input.GetAxis("Horizontal") * VecSpeed.x, 0f,  VecSpeed.z);
	}
#else
	public Vector3 GetDirection()
	{
		return new Vector3(GyroModifyCamera() * VecSpeed.x, 0f, VecSpeed.z);
	}
#endif
	private float GyroModifyCamera()
	{/*
		var gyroValue = GyroToUnity(Input.gyro.attitude);
		var euler = gyroValue.eulerAngles;
		var value = (((euler.z + 360f) % 360f)) / 180f;
		Debug.LogError(gyroValue.ToString() + "|" + euler.ToString() + "|" + value);*/
		//return value;
		return Input.acceleration.x;
	}

	private static Quaternion GyroToUnity(Quaternion q)
	{
		return new Quaternion(q.x, q.y, -q.z, -q.w);
	}
//#endif
	public void OnCollisionEnter(Collision collision)
	{
		var tagObjCollided = collision.collider.tag;

		if (tagObjCollided == "Ennemy")
		{
			if (!DamageLogic())
				SceneManager.LoadScene(0);
			else
			{
				var rigid = collision.rigidbody;

				Vector3 direction = rigid.transform.position - transform.position;
				rigid.AddForceAtPosition(direction.normalized * 10, transform.position);
				collision.collider.enabled = false;
			}
		}
	}

	private float ImpactDmg = 20f;

	public bool DamageLogic()
	{
		Debug.LogError("Damage / End :" + Endurance);
		Endurance -= ImpactDmg;
		TextEndurance.text = "Endurance : " + Endurance.ToString() + "%";
		return Endurance > 0f;
	}
}
