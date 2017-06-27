using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "EnnemyData", menuName = "Scriptable/EnnemyData", order = 2)]
public class EnnemyScriptable : ScriptableObject {

	public EnnemyConfig Config;

}

[System.Serializable]
public struct EnnemyConfig : EntityConfig
{
	public List<EnnemyData> ListEnnemyData;
	public List<PhaseSpawnData> ListPhaseSpawnData;
	public int NumberOfColumns;
	public float SpawnDistanceFromPlayer;
	public bool ModeAleatoire;
	public float LuckSpawnDouble;
	public float DistanceBeetweenSpawn;
	public int LuckTruck;
	public int LuckMoto;

	public CarType GetRandType()
	{
		float rand = UnityEngine.Random.Range(0, 100);

		if (rand < LuckMoto)
		{
			return CarType.Moto;
		}
		else
			return rand < (LuckMoto + LuckTruck) ? CarType.Truck : CarType.Car;
	}
}

[System.Serializable]
public struct EnnemyData : EntityData
{
	public CarEnnemyData CarData;

	public string GetIDPoolObject()
	{
		return CarData.CurrentCarType.ToString() + "Part";
	}

	public float GetSpeedZ()
	{
		return CarData.CurrentSpeedZ;
	}

	public void Init(EntityData Data)
	{
		var newData = (EnnemyData)Data;
		this.CarData = newData.CarData;
		CarData.CurrentSpeedZ = (CarData.SpeedMin + CarData.SpeedMax) / 2f;
	}
}