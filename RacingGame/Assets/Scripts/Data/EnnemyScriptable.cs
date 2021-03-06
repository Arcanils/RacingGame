﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
///		Holder of the ennemy's config
/// </summary>
[CreateAssetMenu(fileName = "EnnemyData", menuName = "Scriptable/EnnemyData", order = 2)]
public class EnnemyScriptable : ScriptableObject {

	public EnnemyConfig Config;

}

/// <summary>
///		Ennemy's config
/// </summary>
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

/// <summary>
///		Ennemy's Data
/// </summary>
[System.Serializable]
public struct EnnemyData : EntityData<EnnemyConfig>
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
	/*
	public void Init(EntityData Data)
	{
		var newData = (EnnemyData)Data;
		this.CarData = newData.CarData;
		CarData.CurrentSpeedZ = (CarData.SpeedMin + CarData.SpeedMax) / 2f;
	}
	*/
	public void Init(EnnemyConfig Data, int IndexStruct)
	{
		this.CarData = Data.ListEnnemyData[IndexStruct].CarData;
		CarData.CurrentSpeedZ = (CarData.SpeedMin + CarData.SpeedMax) / 2f;
	}
}