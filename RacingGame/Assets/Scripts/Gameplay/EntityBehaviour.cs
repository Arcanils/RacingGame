using UnityEngine;
using System.Collections;

public abstract class EntityBehaviour<T, S> : MonoBehaviour 
	where T : EntityConfig
	where S : EntityData
{
	protected T _config;
	protected S _data;
	protected BaseEntity _currentBody;

	public void Init(T Config, Vector3 Position)
	{
		InitConfig(Config);
		InitData();
		GameObject instancePool;
		if (!PoolManager.Instance.GetObject(_config.GetIDPoolObject(), out instancePool))
		{
			throw new System.Exception();
		}

		_currentBody = instancePool.GetComponent<BaseEntity>();

		if (_currentBody == null)
			throw new System.Exception();

		_currentBody.SetPosition(Position);
		_currentBody.Init(transform);
		_currentBody.EventCollision += OnCollision;

		StartCoroutine(BehaviourEnum());
	}


	protected void InitConfig(T Config)
	{
		_config.Init(Config);
	}
	protected void InitData(T Config)
	{
		_data.Init(Config);
	}

	public IEnumerator BehaviourEnum()
	{
		while (true)
		{
			_currentBody.Move(GetVecSpeed());
			yield return null;
		}
	}

	protected abstract void OnCollision(Collision collision);

	protected Vector3 GetVecSpeed()
	{
		return new Vector3(0f, 0f, _config.GetSpeedZ());
	}
}
