using UnityEngine;
using System.Collections;
using System;

public class MainManager : MonoBehaviour {

	public static MainManager Instance { get; private set; }


	private DataConfig _config;

	public void Awake()
	{
		Instance = this;
	}

	public void Start()
	{
		StartCoroutine(Init());
	}

	public IEnumerator Init()
	{
		_config = ConfigManager.Instance.Config;

		PoolManager.Instance.Init();
		RoadManager.Instance.Init(_config.AreaData);
		GameplayManager.Instance.Init(_config.PlayerData.Config, _config.EnnemyData.Config, _config.CameraData);
		UIGameplayManager.Instance.Init();
		yield return UIGameplayManager.Instance.StartGame();
		GameplayManager.Instance.StartLogic();
		RoadManager.Instance.StartLogic();

		yield break;
	}
}
