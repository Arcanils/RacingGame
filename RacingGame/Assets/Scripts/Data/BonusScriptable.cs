﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "BonusData", menuName = "Scriptable/BonusData", order = 1)]
public class BonusScriptable : ScriptableObject
{

	public BonusConfig Config;

}

[System.Serializable]
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

[System.Serializable]
public struct BonusData : EntityData
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

	public void Init(BonusConfig Config, int IndexConfig)
	{
		try
		{
			var val = Config.ListBonus[IndexConfig];
			this.CurrentBonus = val.CurrentBonus;
			this.Value = val.Value;
			this.SpeedZ = val.SpeedZ;
		}
		catch(Exception e)
		{
			Debug.LogError("FAIL INIT " + e.Message);
		}
	}

	public void Init(EntityData Data)
	{
		var newData = (BonusData)Data;
		this.CurrentBonus = newData.CurrentBonus;
		this.Value = newData.Value;
		this.SpeedZ = newData.SpeedZ;
	}
}
