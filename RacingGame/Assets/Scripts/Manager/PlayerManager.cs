﻿using UnityEngine;
using System.Collections;
using System;

public class PlayerManager : MonoBehaviour {

	public static PlayerManager Instance;
	public Vector3 PositionPlayer
	{
		get
		{/*
			if (!PowerTransition)
				return InstanceP != null && InstanceP.CurrentBody != null ? InstanceP.CurrentBody.transform.position : Vector3.zero;
			else
				return CameraBehaviour.Instance.PositionPowerTransition;
				*/

			return InstanceP != null && InstanceP.CurrentBody != null ? InstanceP.CurrentBody.transform.position : Vector3.zero;
		}
	}

	public Transform Container;
	
	public PlayerBehaviour InstanceP { get; private set; }


	private PlayerConfig _config;
	public bool PowerTransition;

	public void Awake()
	{
		Instance = this;
	}

	public void Init(PlayerConfig Config)
	{
		_config = Config;
		GameObject instancePool;
		if (!PoolManager.Instance.GetObject("Player", out instancePool))
		{
			throw new System.Exception();
		}

		instancePool.transform.parent = Container;
		instancePool.transform.localPosition = Vector3.zero;
		InstanceP = instancePool.GetComponent<PlayerBehaviour>();
		//var DataP = new PlayerData();
		//DataP.Init(Config, (int)Config.StartPlayer); 
		InstanceP.Init(Config, (int)Config.StartPlayer, Container.position);
	}

	public void StartLogic()
	{
		InstanceP.StartLogic();
	}


	public void SwitchBodyForPlayer(BaseEntity BodyEnnemy)
	{
		PowerTransition = false;
		BodyEnnemy.transform.parent.GetComponent<EnnemyBehaviour>().SelfDestroy(false);
		BodyEnnemy.Init(InstanceP.transform, true);
		InstanceP.SwitchBody(BodyEnnemy, _config, (int) BodyEnnemy.Type);
		
	}

	
}
