using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable/PlayerData", order = 1)]
public class PlayerScriptable : ScriptableObject {

	public PlayerConfig Config;
}

[System.Serializable]
public struct PlayerConfig : EntityConfig
{
	public List<CarPlayerData> ListPlayerData;
	public float PowerChargeBySec;
	public int PowerChargeMax;
	[Range(0f, 1f)]
	public float BegPowerCharge;
	public CarType StartPlayer;

	public const string IDPool = "Player";

	public string GetIDPoolObject()
	{
		return IDPool;
	}

	public float GetSpeedZ()
	{
		throw new NotImplementedException();
	}

	public void Init(EntityConfig Config)
	{
		throw new NotImplementedException();
	}
}

public struct PlayerData : EntityData
{
	public CarPlayerData CarData;
	public float PowerChargeBySec;
	public int PowerChargeMax;
	[Range(0f, 1f)]
	public float BegPowerCharge;

	public PlayerData(PlayerConfig Config, CarType Type)
	{
		this.CarData = Config.ListPlayerData.Find(element => element.CurrentCarType == Type);
		this.PowerChargeBySec = Config.PowerChargeBySec;
		this.PowerChargeMax = Config.PowerChargeMax;
		this.BegPowerCharge = Config.BegPowerCharge;
	}

	public string GetIDPoolObject()
	{
		return CarData.CurrentCarType.ToString() + "Part";
	}

	public float GetSpeedZ()
	{
		return CarData.Speed;
	}

	public void Init(EntityData Data)
	{
		var newData = (PlayerData)Data;
		this.CarData = newData.CarData;
		this.PowerChargeBySec = newData.PowerChargeBySec;
		this.PowerChargeMax = newData.PowerChargeMax;
	}
}
