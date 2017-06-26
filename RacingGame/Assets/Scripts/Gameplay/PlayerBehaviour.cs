using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class PlayerBehaviour : EntityBehaviour<PlayerConfig>{

	public static PlayerBehaviour Instance { get; private set; }
	

	public Transform TransPlayer
	{
		get
		{
			return _currentBody != null ? _currentBody.transform : null;
		}
	}

	public Action<float> EventChangeScore;
	public Action<int> EventChangeHP;
	public Action<float> EventChangePowerCharge;
	

	private CarPlayerData _data;

	internal void AddBonus(BonusData data)
	{
		throw new NotImplementedException();
	}

	private bool _modeSwitchOn = false;

	public float CurrentPowerCharge
	{
		get
		{
			return _currentPowerCharge;
		}
		private set
		{
			_currentPowerCharge = Mathf.Clamp(value, 0, _config.PowerChargeMax);
			if (EventChangePowerCharge != null)
				EventChangePowerCharge(_currentPowerCharge);
		}
	}

	private float _currentPowerCharge;
	public void Awake()
	{
		Instance = this;
	}
	/*
	public void Init(PlayerConfig PConfig)
	{
		_config = PConfig;

		this._data.Init(PConfig.ListPlayerData.Find(element => element.CurrentCarType == PConfig.StartPlayer));
		GameObject instancePool;
		if (!PoolManager.Instance.GetObject(_data.CurrentCarType.ToString() + "Part", out instancePool))
		{
			throw new System.Exception();
		}

		_currentBody = instancePool.GetComponent<BaseEntity>();

		if (_currentBody == null)
			throw new System.Exception();

		_currentBody.Init(transform, true);
	}*/

	private void InitPower()
	{
		CurrentPowerCharge = _config.BegPowerCharge * _config.PowerChargeMax;
		StartCoroutine(GainPowerUpThroughTime());
	}

	public IEnumerator GainPowerUpThroughTime()
	{
		while (true)
		{
			CurrentPowerCharge += Time.deltaTime * _config.PowerChargeBySec;
			yield return null;
		}
	}

	public void StartLogic()
	{
		StartCoroutine(BehaviourEnum());
	}
	
#if UNITY_STANDALONE || UNITY_EDITOR
	public new Vector3 GetVecSpeed()
	{
		return new Vector3(Input.GetAxis("Horizontal") * _data.SpeedSides, 0f, _data.Speed);
	}
#else
	public override Vector3 GetVecSpeed()
	{
		return new Vector3(Input.acceleration.x * _data.SpeedSides, 0f, _data.Speed);
	}
#endif


	public bool DamageLogic(int Damage)
	{
		_data.CurrentHP -= Damage;
		if (EventChangeHP != null)
			EventChangeHP.Invoke(_data.CurrentHP);
		Debug.LogError("Damage / End :" + _data.CurrentHP);
		//TextEndurance.text = "Endurance : " + _currentHP.ToString() + "%";
		return _data.CurrentHP > 0;
	}

	public void HitByEnnemy(EnnemyBehaviour Ennemy)
	{
		if (!DamageLogic(Ennemy.GetValueDmg()))
		{
			//EndGame
			SceneManager.LoadScene(0);
		}
	}

	public void ResetHP()
	{
		_data.CurrentHP = _data.HP;
		if (EventChangeHP != null)
			EventChangeHP.Invoke(_data.CurrentHP);
	}

	public int GetHP()
	{
		return _data.CurrentHP;
	}

	public void CallPowerUp()
	{
		if (!_modeSwitchOn)
			StartCoroutine(SwitchCarEnum());
	}

	private IEnumerator SwitchCarEnum()
	{
		_modeSwitchOn = true;
		Time.timeScale = 0.01f;
		for (float t = 0f; t < 5f; t += Time.unscaledDeltaTime)
		{
			if (Input.GetMouseButton(0))
			{
				var Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit Info;
				if (Physics.Raycast(Ray, out Info))
				{
					if (Info.transform != null && Info.transform.tag == "Ennemy")
					{
						var script = Info.transform.GetComponent<BaseEntity>();
						if (!script)
							throw new Exception();
						CameraBehaviour.Instance.SwitchTargetCam(Info.transform, 2f, () => SwitchBody(script));
						yield break;
					}
				}
			}
			yield return null;
		}
		Time.timeScale = 1.0f;
	}

	public void SwitchBody(BaseEntity NextBody)
	{
		_currentBody = NextBody;
		this._data.Init(_config.ListPlayerData.Find(element => element.CurrentCarType == _currentBody.Type));
		_currentBody.Init(transform, true);
		_modeSwitchOn = false;
		ResetHP();
		Time.timeScale = 1.0f;
	}

	protected override void OnCollision(Collision collision)
	{
		Debug.LogError("Collision from Player");
	}
}
