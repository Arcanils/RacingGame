﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public struct CarEnnemyData
{
	public CarType CurrentCarType;
	public int Damage;
	public float SpeedMin;
	public float SpeedMax;

	[System.NonSerialized]
	public float CurrentSpeedZ;

	public void Init(CarEnnemyData Data)
	{
		this.CurrentCarType = Data.CurrentCarType;
		this.Damage = Data.Damage;
		this.SpeedMin = Data.SpeedMin;
		this.SpeedMax = Data.SpeedMax;

		this.CurrentSpeedZ = Random.Range(this.SpeedMin, this.SpeedMax);
	}
}

[System.Serializable]
public struct CarPlayerData
{
	public CarType CurrentCarType;
	public int HP;
	[System.NonSerialized]
	public int CurrentHP;
	public float Speed;
	public float SpeedSides;


	public void Init(CarPlayerData Data)
	{
		this.CurrentCarType = Data.CurrentCarType;
		this.HP = Data.HP;
		this.CurrentHP = this.HP;
		this.Speed = Data.Speed;
		this.SpeedSides = Data.SpeedSides;
	}
}

public enum CarType
{
	Car,
	Truck,
	Moto,
}