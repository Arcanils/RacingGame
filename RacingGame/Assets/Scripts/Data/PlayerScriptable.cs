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

public struct PlayerData : EntityData
{
	public CarPlayerData CarData;
	public float PowerChargeBySec;
	public int PowerChargeMax;
	[Range(0f, 1f)]
	public float BegPowerCharge;
	public bool UltimeEnable;
	public bool ExploEnable;

	public PlayerData(PlayerConfig Config, CarType Type)
	{
		this.CarData = Config.ListPlayerData.Find(element => element.CurrentCarType == Type);
		this.PowerChargeBySec = Config.PowerChargeBySec;
		this.PowerChargeMax = Config.PowerChargeMax;
		this.BegPowerCharge = Config.BegPowerCharge;
		this.UltimeEnable = false;
		this.ExploEnable = false;
		ApplyUpgrade(Config, Type);
	}

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
					CarData.SpeedSides += (int)(upgrades[i].Values[index].Value);
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

	public void Init(EntityData Data)
	{
		var newData = (PlayerData)Data;
		this.CarData = newData.CarData;
		this.PowerChargeBySec = newData.PowerChargeBySec;
		this.PowerChargeMax = newData.PowerChargeMax;
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


