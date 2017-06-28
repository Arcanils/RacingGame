using UnityEngine;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System;

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

	private const string _namefile = "/Save.txt";
	private static string _path
	{
		get
		{
			return Application.persistentDataPath + _namefile;
		}
	}
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

		if (File.Exists(_path))
		{
			_instance.Data = HolderSave.Load(_path);
			Debug.LogError("LoadSuccess");
		}
		else
		{
			_instance.Data = new HolderSave();
			_instance.Data.Save(_path);
			Debug.LogError("LoadFail");
		}

		_instance.Data.ApplyData();
	}

	public void UpdateDataAndSave()
	{
		Data.UpgradeProgress.UpdateData(ConfigManager.Instance.Config.PlayerData.Config.ListTypeUpgrade);
		Data.Save(_path);
	}
}

[System.Serializable]
public class HolderSave
{
	//Score
	//Currency
	public SaveUpgradeProgress UpgradeProgress;

	public HolderSave()
	{
		UpgradeProgress = new SaveUpgradeProgress();
		UpgradeProgress.FirstInit();
	}

	public void ApplyData()
	{
		ConfigManager.Instance.UpdateData(UpgradeProgress);
	}


	public void Save(string path)
	{
		var serializer = new XmlSerializer(typeof(HolderSave));
		using (var stream = new FileStream(path, FileMode.Create))
		{
			serializer.Serialize(stream, this);
		}
	}

	public static HolderSave Load(string path)
	{
		var serializer = new XmlSerializer(typeof(HolderSave));
		using (var stream = new FileStream(path, FileMode.Open))
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
