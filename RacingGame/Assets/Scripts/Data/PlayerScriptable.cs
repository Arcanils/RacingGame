using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable/PlayerData", order = 1)]
public class PlayerScriptable : ScriptableObject {

	public PlayerConfig Config;

	public void UpdateData(SaveUpgradeProgress UpgradeProgress)
	{
		Config.UpdateData(UpgradeProgress);
	}
}

[System.Serializable]
public class PlayerConfig : EntityConfig
{
	public List<CarPlayerData> ListPlayerData;
	public List<ContainerTypeUpgrade> ListTypeUpgrade;
	public float PowerChargeBySec;
	public int PowerChargeMax;
	[Range(0f, 1f)]
	public float BegPowerCharge;
	public CarType StartPlayer;

	public const string IDPool = "Player";

	public PlayerConfig()
	{

	}

	public string GetIDPoolObject()
	{
		return IDPool;
	}

	public void UpdateData(SaveUpgradeProgress UpgradeProgress)
	{
		for (int i = 0, iLength = ListTypeUpgrade.Count; i < iLength; i++)
		{
			ListTypeUpgrade[i].UpdateData(UpgradeProgress.UpgradeLvl[i]);
		}
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

public struct PlayerData : EntityData<PlayerConfig>
{
	public CarPlayerData CarData;
	public float PowerChargeBySec;
	public int PowerChargeMax;
	[Range(0f, 1f)]
	public float BegPowerCharge;
	public bool UltimeEnable;
	public bool ExploEnable;
	public AnimationCurve CurveControl
	{
		get
		{
			return _curveControl;
		}
		set
		{
			_curveControl = value;
		}
	}
	private AnimationCurve _curveControl;
	

	public void ApplyUpgrade(PlayerConfig Config, CarType Type)
	{
		var data = Config.ListTypeUpgrade.Find(element => element.MyType.ToString() == Type.ToString());
		if (data != null)
		{
			var upgrades = data.ListUpgrade;
			int index;
			for (int i = 0; i < upgrades.Count; i++)
			{
				index = upgrades[i].CurrentIndexValue;
				if (index == 0)
					continue;
				if (upgrades[i].NameAbility == "HP")
				{
					CarData.HP += (int)(upgrades[i].Values[index].Value);
				}
				else if (upgrades[i].NameAbility == "Speed")
				{
					CarData.Speed += (int)(upgrades[i].Values[index].Value);
				}
				else if (upgrades[i].NameAbility == "Control")
				{
					CurveControl = CarData.CurveControl[index];
				}
				else if (upgrades[i].NameAbility == "Ultime")
				{
					UltimeEnable = true;
				}
			}
		}
		data = Config.ListTypeUpgrade.Find(element => element.MyType.ToString() == "Ghost");
		if (data != null)
		{
			var upgrades = data.ListUpgrade;
			int index;
			for (int i = 0; i < upgrades.Count; i++)
			{
				index = upgrades[i].CurrentIndexValue;
				if (index == 0)
					continue;
				if (upgrades[i].NameAbility == "BegPowerCharge")
				{
					BegPowerCharge += (upgrades[i].Values[index].Value);
				}
				else if (upgrades[i].NameAbility == "Explo")
				{
					ExploEnable = true;
				}
			}
		}
	}

	public string GetIDPoolObject()
	{
		return CarData.CurrentCarType.ToString() + "Part";
	}

	public float GetSpeedZ()
	{
		return CarData.Speed;
	}
	/*
	public void Init(EntityData Data)
	{
		Debug.LogError("NE DOIT PAS PASSER ICI");
		var newData = (PlayerData)Data;
		this.CarData = newData.CarData;
		this.PowerChargeBySec = newData.PowerChargeBySec;
		this.PowerChargeMax = newData.PowerChargeMax;
	}*/

	public void Init(PlayerConfig Data, int IndexStruct)
	{
		this.CarData = Data.ListPlayerData.Find(element => (int)element.CurrentCarType == IndexStruct);
		this.PowerChargeBySec = Data.PowerChargeBySec;
		this.PowerChargeMax = Data.PowerChargeMax;
		this.BegPowerCharge = Data.BegPowerCharge;
		this.UltimeEnable = false;
		this.ExploEnable = false;
		this._curveControl = this.CarData.CurveControl[0];
		ApplyUpgrade(Data, (CarType)IndexStruct);
		Debug.LogError(CarData.Speed);
		this.CarData.CurrentHP = this.CarData.HP;
	}
}

[Serializable]
public class UpgradePlayer
{
	public string NameAbility;
	public CouplePriceValue[] Values;

	[System.NonSerialized]
	public int CurrentIndexValue;

	public UpgradePlayer()
	{

	}

	public void SetIndex(int Index)
	{
		this.CurrentIndexValue = Index;
	}

	public bool UpgradeIfPossible(float value)
	{

		if (CurrentIndexValue < Values.Length && Values[CurrentIndexValue].Price <= value)
		{ 
			ConfigManager.Instance.Config.Currency -= Values[CurrentIndexValue].Price;
			CurrentIndexValue++;
			return true;
		}
		else
			return false;
	}
}

[Serializable]
public struct CouplePriceValue
{
	public float Price;
	public float Value;
}

[Serializable]
public class ContainerTypeUpgrade
{
	public enum TypeUpgrade
	{
		Car,
		Truck,
		Moto,
		Ghost,
	}
	public TypeUpgrade MyType;

	public List<UpgradePlayer> ListUpgrade;

	public ContainerTypeUpgrade()
	{

	}

	public void UpdateData(List<int> ListIndex)
	{
		for (int i = 0, iLength = ListUpgrade.Count; i < iLength; i++)
		{
			ListUpgrade[i].SetIndex(ListIndex[i]);
		}
	}
}


