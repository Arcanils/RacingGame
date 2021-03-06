﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class GameplayManager : MonoBehaviour{

	public static GameplayManager Instance { get; private set; }

	private List<Action> _listEntityApplyBehaviour = new List<Action>();

	public void Awake()
	{
		Instance = this;
	}


	public void Init(PlayerConfig PConfig, EnnemyConfig EConfig, CameraConfig CConfig)
	{
		InitCam(CConfig);
		InitPlayer(PConfig);
		InitEnnemy(EConfig);
	}

	public void InitPlayer(PlayerConfig PConfig)
	{
		PlayerManager.Instance.Init(PConfig);
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
		PlayerManager.Instance.StartLogic();
		EnnemyManager.Instance.StartLogic();
		StartCoroutine(GameplayLogicEnum());
	}

	/// <summary>
	///		Pipeline logic.
	///		Process entities first, enemymanagerData second and camera third
	/// </summary>
	/// <returns></returns>
	private IEnumerator GameplayLogicEnum()
	{
		while(true)
		{
			for (int i = 0, iLength = _listEntityApplyBehaviour.Count; i < iLength; ++i)
			{
				_listEntityApplyBehaviour[i].Invoke();
			}
			EnnemyManager.Instance.RefreshData();
			CameraBehaviour.Instance.ApplyBehaviour();
			yield return null;
		}
	}

	public void AddBehaviourEntity(Action Behaviour)
	{
		_listEntityApplyBehaviour.Add(Behaviour);
	}

	public void RemoveBehaviourEntity(Action Behaviour)
	{
		_listEntityApplyBehaviour.Remove(Behaviour);
	}
}
