using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

/// <summary>
///		Player's Controller
/// </summary>
public class PlayerBehaviour : EntityBehaviour<PlayerConfig, PlayerData>{

	public Action<int> EventChangeScore;
	public Action<int> EventChangeHP;
	public Action<int> EventNewMaxHP; 
	public Action<float> EventChangePowerCharge;
	public Action<int> EventChangeDistance;

	public int CurrentDistance
	{
		get
		{
			return _currentDistance;
		}
		private set
		{
			if (value != _currentDistance)
			{
				_currentDistance = value;
				if (EventChangeDistance != null)
					EventChangeDistance.Invoke(value);
			}
		}
	}
	private int _currentDistance;

	public int CurrentScore
	{
		get
		{
			return _currentScore;
		}
		set
		{
			if (value != _currentScore)
			{
				_currentScore = value;
				if (EventChangeScore != null)
					EventChangeScore.Invoke(value);
			}
		}
	}
	private int _currentScore;

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
				EventChangePowerCharge(_currentPowerCharge  / _data.PowerChargeMax);
		}
	}

	private bool _modeSwitchOn = false;
	private float _currentPowerCharge;

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
	/// <summary>
	///		Smooth value with curve. Better control overall
	/// </summary>
	/// <returns></returns>
	protected override Vector3 GetVecSpeed()
	{
		var val = Input.GetAxis("Horizontal");
		return new Vector3(_data.CurveControl.Evaluate(Mathf.Abs(val)) * _data.CarData.SpeedSides * Mathf.Sign(val), 0f, _data.CarData.Speed);
	}
#else
	protected override Vector3 GetVecSpeed()
	{
		var val = Input.acceleration.x;
		return new Vector3(_data.CurveControl.Evaluate(Math.Abs(val)) * _data.CarData.SpeedSides * Mathf.Sign(val), 0f, _data.CarData.Speed);
	}
#endif

	/// <summary>
	///		Aplly damage
	/// </summary>
	/// <param name="Damage"></param>
	/// <returns></returns>
	public bool DamageLogic(int Damage)
	{
		_data.CarData.CurrentHP -= Damage;
		if (EventChangeHP != null)
			EventChangeHP.Invoke(_data.CarData.CurrentHP);
		return _data.CarData.CurrentHP > 0;
	}

	public void HitByEnnemy(EnnemyBehaviour Ennemy)
	{
		CurrentScore += (((int)(Ennemy.GetCarType()) + 1)) * 100;
		if (!DamageLogic(Ennemy.GetValueDmg()))
		{
			//EndGame
			UIGameplayManager.Instance.ShowEndScreen();
		}
	}

	public void ResetHP()
	{
		_data.CarData.CurrentHP = _data.CarData.HP;
		if (EventNewMaxHP != null)
			EventNewMaxHP.Invoke(_data.CarData.HP);
	}

	public int GetHP()
	{
		return _data.CarData.CurrentHP;
	}

	public void CallPowerUp()
	{
		if (!_modeSwitchOn && CurrentPowerCharge >= _data.PowerChargeMax)
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
			do
				yield return null;
			while (UIGameplayManager.Instance.IsPaused);
		}

		_modeSwitchOn = false;
		CurrentPowerCharge = 0f;

		StartCoroutine(BackSlerpToRealTimeScale());
	}

	public void SwitchBody(BaseEntity NextBody, PlayerConfig Config, int IndexStruct)
	{
		_data.Init(Config, IndexStruct);
		CurrentBody.SelfDestroy();
		CurrentBody = NextBody;
		CurrentBody.Init(transform, true);
		ResetHP();
		_modeSwitchOn = false;
		CurrentPowerCharge = 0f;
		//Time.timeScale = 1.0f;
		StartCoroutine(BackSlerpToRealTimeScale());
	}

	private IEnumerator BackSlerpToRealTimeScale()
	{
		float begTime = Time.timeScale;
		float endTime = 1f;

		for (float t = 0f, perc = 0f; perc < 1f; t += Time.unscaledDeltaTime)
		{
			perc = Mathf.Clamp01(t / 1f);
			Time.timeScale = Mathf.Lerp(begTime, endTime, Mathf.SmoothStep(0f, 1f, perc));
			yield return null;
		}
	}

	protected override void OnCollision(Collision collision)
	{
		//Debug.LogError("Collision from Player");
	}


	public void AddBonus(BonusData data)
	{
		if (data.CurrentBonus == BonusData.TypeBonus.PowerCharge)
			CurrentPowerCharge += data.Value;
	}

	public override void StartLogic()
	{
		base.StartLogic();
		InitPower();
		StartCoroutine(DistanceEnum());
	}

	public IEnumerator DistanceEnum()
	{
		float Origine = CurrentBody.transform.position.z;
		int NewValue = 0;
		while (true)
		{
			NewValue = Mathf.RoundToInt((CurrentBody.transform.position.z - Origine) / 100f);
			if (NewValue > CurrentDistance)
			{
				CurrentDistance = NewValue;

				CurrentScore += 1000;
			}
			yield return null;
		}
	}
}
