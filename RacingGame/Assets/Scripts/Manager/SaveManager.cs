using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System;

/// <summary>
///  Manager of save of the game
/// </summary>
[System.Serializable]
public class SaveManager : MonoBehaviour
{

	public static SaveManager Instance
	{
		get
		{
			if (_instance == null)
				CreateAndInit();
			return _instance;
		}
	}
	private static SaveManager _instance;

	public HolderSave Data;

	public static void Init()
	{
		if (_instance == null)
			CreateAndInit();
	}

	private static void CreateAndInit()
	{
		GameObject go = new GameObject("__SaveManager__", typeof(SaveManager)) as GameObject;
		DontDestroyOnLoad(go);
		_instance = go.GetComponent<SaveManager>();

		if (HolderSave.FilesaveExist())
		{
			_instance.Data = HolderSave.Load();
			Debug.LogError("LoadSuccess");
		}
		else
		{
			_instance.Data = new HolderSave();
			_instance.Data.Save();
			Debug.LogError("LoadFail");
		}

		_instance.Data.ApplyData();
	}

	public void UpdateDataAndSave()
	{
		Data.UpgradeProgress.UpdateData(ConfigManager.Instance.Config.PlayerData.Config.ListTypeUpgrade);
		Data.Save();
	}
}

[System.Serializable]
public class HolderSave
{
	//Score
	public SaveUpgradeProgress UpgradeProgress;
	//Currency
	public float Currency
	{
		get
		{
			return _currency;
		}
		private set
		{
			_currency = value;
		}
	}
	[SerializeField]
	private float _currency;
	private const string _namefile = "/Save.txt";


	public HolderSave()
	{
		UpgradeProgress = new SaveUpgradeProgress();
		UpgradeProgress.FirstInit();
	}

	public void ApplyData()
	{
		ConfigManager.Instance.UpdateData(this);
	}

	public void AddCurrency(int Value)
	{
		_currency += Value;

		Save();
	}

	public bool RemoveCurrency(int Value)
	{
		if (Currency < Value)
			return false;

		_currency -= Value;
		Save();
		return true;
	}

	private static string _path
	{
		get
		{
			return Application.persistentDataPath + _namefile;
		}
	}

	public static bool FilesaveExist()
	{
		return File.Exists(_path);
	}

	public void Save()
	{
		var serializer = new XmlSerializer(typeof(HolderSave));
		using (var stream = new FileStream(_path, FileMode.Create))
		{
			serializer.Serialize(stream, this);
		}
		Debug.LogError("Save Success");
	}

	public static HolderSave Load()
	{
		var serializer = new XmlSerializer(typeof(HolderSave));
		using (var stream = new FileStream(_path, FileMode.Open))
		{
			return serializer.Deserialize(stream) as HolderSave;
		}
	}

	public static HolderSave LoadFromText(string text)
	{
		var serializer = new XmlSerializer(typeof(HolderSave));
		return serializer.Deserialize(new StringReader(text)) as HolderSave;
	}
}

public class SaveUpgradeProgress
{
	public List<int>[] UpgradeLvl;
	public SaveUpgradeProgress()
	{

	}

	public void FirstInit()
	{
		PlayerConfig config = ConfigManager.Instance.Config.PlayerData.Config;

		var data = config.ListTypeUpgrade;
		UpgradeLvl = new List<int>[4];
		for (int i = 0; i < 4; i++)
		{
			UpgradeLvl[i] = new List<int>();
			for (int j = 0, jLength = data[i].ListUpgrade.Count; j < jLength; j++)
			{
				UpgradeLvl[i].Add(0);
			}
		}
	}

	public void UpdateData(List<ContainerTypeUpgrade> ListUpgrade)
	{
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0, jLength = ListUpgrade[i].ListUpgrade.Count; j < jLength; j++)
			{
				UpgradeLvl[i][j] = ListUpgrade[i].ListUpgrade[j].CurrentIndexValue;
			}
		}
	}
}
