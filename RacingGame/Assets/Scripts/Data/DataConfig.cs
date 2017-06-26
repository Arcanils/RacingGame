using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "Config", menuName = "Scriptable/Config", order = 1)]
public class DataConfig : ScriptableObject
{ 
	public AreaConfig AreaData;
	public EnnemyConfig EnnemyData;
	public PlayerConfig PlayerData;
	public CameraConfig CameraData;
}

[System.Serializable]
public struct AreaConfig
{
	public float SizeRoadPart;
	public float DistanceFromPlayerBeforeMovePart;
	public int RoadPartLength;
}

[System.Serializable]
public struct EnnemyConfig
{
	public List<CarEnnemyData> ListEnnemyData;
	public List<PhaseSpawnData> ListPhaseSpawnData;
	public int NumberOfColumns;
	public float SpawnDistanceFromPlayer;
	public bool ModeAleatoire;
	public float LuckSpawnDouble;
	public float TimeBeetweenSpawnAlea;
	public int LuckTruck;
	public int LuckMoto;

	public CarType GetRandType()
	{
		float rand = Random.Range(0, 100);

		if (rand < LuckMoto)
		{
			return CarType.Moto;
		}
		else
			return rand < (LuckMoto + LuckTruck) ? CarType.Truck : CarType.Car;
	}
}

[System.Serializable]
public struct PlayerConfig : EntityConfig
{
	public List<CarPlayerData> ListPlayerData;
	public CarType StartPlayer;
	public float PowerChargeBySec;
	public int PowerChargeMax;
	[Range(0f, 1f)]
	public float BegPowerCharge;
}

[System.Serializable]
public struct CameraConfig
{

}

public struct BonusConfig : EntityConfig
{
	public List<BonusData> ListBonus;

	public string GetIDPoolObject()
	{
		throw new NotImplementedException();
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

public struct BonusData : EntityData<BonusConfig>
{
	public enum TypeBonus
	{
		Score,
		PowerCharge,
		Multiplicateur,
		SlowDown,
	}

	public TypeBonus CurrentBonus;
	public float Value;
	public float SpeedZ;

	public string GetIDPoolObject()
	{
		return "Bonus" + CurrentBonus.ToString();
	}

	public float GetSpeedZ()
	{
		return SpeedZ;
	}

	public void Init(BonusConfig Config)
	{
		SpeedZ = Config.
	}
}

public interface EntityConfig
{
	void Init(EntityConfig Config);
	string GetIDPoolObject();
	float GetSpeedZ();
}

public interface EntityData<T> where T : EntityConfig
{
	string GetIDPoolObject();
	float GetSpeedZ();
	void Init(T Config);
}
