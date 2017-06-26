using UnityEngine;
using System.Collections;

public abstract class EntityBehaviour<S> : MonoBehaviour 
	where S : EntityData
{
	protected S _data;
	public BaseEntity CurrentBody { get; protected set; }

	public void Init(S Data, Vector3 Position)
	{
		InitData(Data);
		GameObject instancePool;
		if (!PoolManager.Instance.GetObject(Data.GetIDPoolObject(), out instancePool))
		{
			throw new System.Exception();
		}

		CurrentBody = instancePool.GetComponent<BaseEntity>();

		if (CurrentBody == null)
			throw new System.Exception();

		CurrentBody.SetPosition(Position);
		CurrentBody.Init(transform, GetType().ToString() == "PlayerBehaviour");
		CurrentBody.EventCollision += OnCollision;

	}
	/*

	protected void InitConfig(T Config)
	{
		_config.Init(Config);
	}
	*/
	protected void InitData(S Data)
	{
		_data.Init(Data);
	}


	public virtual void StartLogic()
	{
		StartCoroutine(BehaviourEnum());
	}
	public IEnumerator BehaviourEnum()
	{
		while (true)
		{
			CurrentBody.Move(GetVecSpeed());
			yield return null;
		}
	}

	protected abstract void OnCollision(Collision collision);

	protected virtual Vector3 GetVecSpeed()
	{
		return new Vector3(0f, 0f, _data.GetSpeedZ());
	}
}
