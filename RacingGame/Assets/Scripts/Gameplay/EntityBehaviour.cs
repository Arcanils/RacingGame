using UnityEngine;
using System.Collections;

public abstract class EntityBehaviour<T,S> : MonoBehaviour
	where T : EntityConfig
	where S : EntityData<T>
{
	public BaseEntity CurrentBody { get; protected set; }

	protected S _data;
	protected IEnumerator _behaviourMove;
	protected PoolComponent _poolComponent;
	public void Awake()
	{
		_behaviourMove = BehaviourMoveEnum();
	}

	public void Init(T Config, int IndexStruct, Vector3 Position)
	{
		InitData(Config, IndexStruct);
		GameObject instancePool;
		if (!PoolManager.Instance.GetObject(_data.GetIDPoolObject(), out instancePool))
		{
			throw new System.Exception();
		}
		_poolComponent = GetComponent<PoolComponent>();
		if (_poolComponent != null)
			_poolComponent.OnResetToPool += OnResetToPool;
		CurrentBody = instancePool.GetComponent<BaseEntity>();

		if (CurrentBody == null)
			throw new System.Exception();

		CurrentBody.SetPosition(Position);
		CurrentBody.Init(transform, GetType().ToString() == "PlayerBehaviour");
		CurrentBody.EventCollision += OnCollision;
		GameplayManager.Instance.AddBehaviourEntity(ApplyBehaviour);
	}
	/*

	protected void InitConfig(T Config)
	{
		_config.Init(Config);
	}
	*/
	protected void InitData(T Data, int IndexStruct)
	{
		_data.Init(Data, IndexStruct);
	}


	public virtual void StartLogic()
	{
		//StartCoroutine(BehaviourEnum());
	}

	public virtual void ApplyBehaviour()
	{
		_behaviourMove.MoveNext();
	}

	public IEnumerator BehaviourMoveEnum()
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

	public void SelfDestroy()
	{
		if (_poolComponent != null)
			_poolComponent.BackToPool();
		else
		{
			GameplayManager.Instance.RemoveBehaviourEntity(ApplyBehaviour);
			Destroy(gameObject);
		}
	}

	protected virtual void OnResetToPool()
	{
		GameplayManager.Instance.RemoveBehaviourEntity(ApplyBehaviour);
		if (_poolComponent != null)
			_poolComponent.OnResetToPool -= OnResetToPool;
	}


}
