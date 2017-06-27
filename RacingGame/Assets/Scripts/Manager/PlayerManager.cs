using UnityEngine;
using System.Collections;
using System;

public class PlayerManager : MonoBehaviour {

	public static PlayerManager Instance;
	public Vector3 PositionPlayer
	{
		get
		{
			if (!_powerTransition)
				return InstanceP != null && InstanceP.CurrentBody != null ? InstanceP.CurrentBody.transform.position : Vector3.zero;
			else
				return CameraBehaviour.Instance.PositionPowerTransition;
		}
	}

	public Transform Container;
	
	public PlayerBehaviour InstanceP { get; private set; }


	private PlayerConfig _config;
	private bool _powerTransition;

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
		InstanceP.Init(new PlayerData(Config, Config.StartPlayer), Container.position);
		InstanceP.ResetHP();
	}

	public void StartLogic()
	{
		InstanceP.StartLogic();
	}


	public void SwitchBodyForPlayer(BaseEntity BodyEnnemy)
	{
		BodyEnnemy.Init(InstanceP.transform, true);
		InstanceP.SwitchBody(BodyEnnemy, _config.ListPlayerData.Find(element => element.CurrentCarType == BodyEnnemy.Type));
		
	}

	
}
