using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
///		Holder of the player's config
/// </summary>
[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable/PlayerData", order = 1)]
public class PlayerScriptable : ScriptableObject {

	public PlayerConfig Config;

	public void UpdateData(SaveUpgradeProgress UpgradeProgress)
	{
		Config.UpdateData(UpgradeProgress);
	}
}

/// <summary>
///		Player's config
/// </summary>
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

	/// <summary>
	///		Update Index of the player's upgrades
	/// </summary>
	/// <param name="UpgradeProgress"></param>
	public void UpdateData(SaveUpgradeProgress UpgradeProgress)
	{
		for (int i = 0, iLength = ListTypeUpgrade.Count; i < iLength; i++)
		{
			ListTypeUpgrade[i].UpdateData(UpgradeProgress.UpgradeLvl[i]);
		}
	}
}

/// <summary>
///		Player's Data
/// </summary>
public struct PlayerData : EntityData<PlayerConfig>
{
	public CarPlayerData CarData;
	public float PowerChargeBySec;
	public int PowerChargeMax;
	[Range(0f, 1f)]
	public float BegPowerCharge;
	/// <summary>
	///		Ulimate enable (Not Implemented)
	/// </summary>
	public bool UltimeEnable;
	/// <summary>
	///		Exlosion on switch body enable (Not Implemented)
	/// </summary>
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
	
	/// <summary>
	///		Upgrade Player's Data
	/// </summary>
	/// <param name="Config"></param>
	/// <param name="Type"></param>
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
				--index;
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
					CurveControl = CarData.CurveControl[index + 1];
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
				--index;
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
		this.CarData.CurrentHP = this.CarData.HP;
	}
}

/// <summary>
///		Data of the player's upgrade
/// </summary>
[Serializable]
public class UpgradePlayer
{
	public string NameAbility;
	public CouplePriceValue[] Values;
	
	public int CurrentIndexValue
	{
		get
		{
			return _currentIndexValue;
		}
		private set
		{
			if (value != _currentIndexValue)
			{
				_currentIndexValue = value;
				SaveManager.Instance.UpdateDataAndSave();
			}
		}
	}
	private int _currentIndexValue;

	public UpgradePlayer()
	{

	}

	public void SetIndex(int Index)
	{
		this._currentIndexValue = Index;
	}
	

	public bool CanUpgrade()
	{
		return CurrentIndexValue < Values.Length;
	}

	public void Upgrade()
	{
		if (CanUpgrade())
		{
			++CurrentIndexValue;
		}
	}

	public float GetValueNextUpgrade()
	{
		return CanUpgrade() ? Values[CurrentIndexValue].Price : 0f;
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


