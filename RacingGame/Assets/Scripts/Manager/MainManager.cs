using UnityEngine;
using System.Collections;
using System;

public class MainManager : MonoBehaviour {

	public static MainManager Instance { get; private set; }

	private const string _gameConfigPath = "Scriptable/GameConfig";

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
		_config = Resources.Load<DataConfig>(_gameConfigPath);

		if (_config == null)
		{
			throw new Exception();
		}

		PoolManager.Instance.Init();
		RoadManager.Instance.Init(_config.AreaData);
		GameplayManager.Instance.Init(_config.PlayerData, _config.EnnemyData, _config.CameraData);
		UIGameplayManager.Instance.Init();

		//FadeOut
		//Decompte
		//Player play
		//SpawnEnnemy Play
		GameplayManager.Instance.StartLogic();
		RoadManager.Instance.StartLogic();

		yield break;
	}
}
