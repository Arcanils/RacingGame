using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
///		Data for the ennemy's car
/// </summary>
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

/// <summary>
///		Data for player's car
/// </summary>
[System.Serializable]
public struct CarPlayerData
{
	public CarType CurrentCarType;
	public int HP;
	[System.NonSerialized]
	public int CurrentHP;
	public float Speed;
	public float SpeedSides;

	/// <summary>
	///		Curve of control apply on the speed left / right
	/// </summary>
	public List<AnimationCurve> CurveControl;


	public void Init(CarPlayerData Data)
	{
		this.CurrentCarType = Data.CurrentCarType;
		this.HP = Data.HP;
		this.CurrentHP = this.HP;
		this.Speed = Data.Speed;
		this.SpeedSides = Data.SpeedSides;
		this.CurveControl = Data.CurveControl;
	}
}

public enum CarType
{
	Car,
	Truck,
	Moto,
}
