using UnityEngine;
using System.Collections;
using System;

public class GameplayManager : MonoBehaviour {

	public static GameplayManager Instance { get; private set; }


	public void Awake()
	{
		Instance = this;
	}


	public void Init(PlayerConfig PConfig, EnnemyConfig EConfig, CameraConfig CConfig)
	{
		InitPlayer(PConfig);
		InitEnnemy(EConfig);
		InitCam(CConfig);
	}

	public void InitPlayer(PlayerConfig PConfig)
	{
		PlayerBehaviour.Instance.Init(PConfig);
	}

	public void InitEnnemy(EnnemyConfig EConfig)
	{
		EnnemyManager.Instance.Init(EConfig);
	}

	public void InitCam(CameraConfig CConfig)
	{
		CameraBehaviour.Instance.Init(CConfig);
	}

	public void StartLogic()
	{
		PlayerBehaviour.Instance.StartLogic();
		EnnemyManager.Instance.StartLogic();
	}
}
