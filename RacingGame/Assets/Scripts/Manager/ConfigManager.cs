using UnityEngine;
using System.Collections;

public class ConfigManager : MonoBehaviour {


	public static ConfigManager Instance
	{
		get
		{
			if (_instance == null)
				CreateAndInit();
			return _instance;
		}
	}
	private static ConfigManager _instance;

	public DataConfig Config;

	private const string _gameConfigPath = "Scriptable/GameConfig";

	public static void Init()
	{
		if (_instance == null)
			CreateAndInit();
	}

	private static void CreateAndInit()
	{
		GameObject go = new GameObject("__ConfigManager__", typeof(ConfigManager)) as GameObject;
		DontDestroyOnLoad(go);
		_instance = go.GetComponent<ConfigManager>();

		_instance.Config = Resources.Load<DataConfig>(_gameConfigPath);
	}

	public void UpdateData(SaveUpgradeProgress UpgradeProgress)
	{
		Config.PlayerData.UpdateData(UpgradeProgress);
	}
}
