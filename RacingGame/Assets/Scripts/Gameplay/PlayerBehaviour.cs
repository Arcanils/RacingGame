using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class PlayerBehaviour : EntityBehaviour<PlayerData>{

	public Action<float> EventChangeScore;
	public Action<int> EventChangeHP;
	public Action<float> EventChangePowerCharge;
	

	private bool _modeSwitchOn = false;

	public float CurrentPowerCharge
	{
		get
		{
			return _currentPowerCharge;
		}
		private set
		{
			_currentPowerCharge = Mathf.Clamp(value, 0, _data.PowerChargeMax);
			if (EventChangePowerCharge != null)
				EventChangePowerCharge(_currentPowerCharge);
		}
	}

	private float _currentPowerCharge;
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
		CurrentPowerCharge = _data.BegPowerCharge * _data.PowerChargeMax;
		StartCoroutine(GainPowerUpThroughTime());
	}

	public IEnumerator GainPowerUpThroughTime()
	{
		while (true)
		{
			CurrentPowerCharge += Time.deltaTime * _data.PowerChargeBySec;
			yield return null;
		}
	}
	
#if UNITY_STANDALONE || UNITY_EDITOR
	protected override Vector3 GetVecSpeed()
	{
		return new Vector3(Input.GetAxis("Horizontal") * _data.CarData.SpeedSides, 0f, _data.CarData.Speed);
	}
#else
	protected override Vector3 GetVecSpeed()
	{
		return new Vector3(Input.acceleration.x * _data.SpeedSides, 0f, _data.Speed);
	}
#endif


	public bool DamageLogic(int Damage)
	{
		_data.CarData.CurrentHP -= Damage;
		if (EventChangeHP != null)
			EventChangeHP.Invoke(_data.CarData.CurrentHP);
		Debug.LogError("Damage / End :" + _data.CarData.CurrentHP);
		//TextEndurance.text = "Endurance : " + _currentHP.ToString() + "%";
		return _data.CarData.CurrentHP > 0;
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
		_data.CarData.CurrentHP = _data.CarData.HP;
		if (EventChangeHP != null)
			EventChangeHP.Invoke(_data.CarData.CurrentHP);
	}

	public int GetHP()
	{
		return _data.CarData.CurrentHP;
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
						CameraBehaviour.Instance.SwitchTargetCam(Info.transform, 2f, () => PlayerManager.Instance.SwitchBodyForPlayer(script));
						yield break;
					}
				}
			}
			yield return null;
		}
		Time.timeScale = 1.0f;
	}

	public void SwitchBody(BaseEntity NextBody, CarPlayerData Data)
	{
		_data.CarData = Data;
		CurrentBody = NextBody;
		CurrentBody.Init(transform, true);
		_modeSwitchOn = false;
		ResetHP();
		CurrentPowerCharge = 0f;
		Time.timeScale = 1.0f;
	}

	protected override void OnCollision(Collision collision)
	{
		Debug.LogError("Collision from Player");
	}


	public void AddBonus(BonusData data)
	{
		if (data.CurrentBonus == BonusData.TypeBonus.PowerCharge)
			CurrentPowerCharge += data.Value;
	}
}
