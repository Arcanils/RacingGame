using UnityEngine;
using System.Collections;

/// <summary>
///		Base controller
/// </summary>
/// <typeparam name="T"></typeparam>
/// <typeparam name="S"></typeparam>
public abstract class EntityBehaviour<T,S> : MonoBehaviour
	where T : EntityConfig
	where S : EntityData<T>
{
	/// <summary>
	///		Body
	/// </summary>
	public BaseEntity CurrentBody { get; protected set; }

	/// <summary>
	///		Data for the entity
	/// </summary>
	protected S _data;
	/// <summary>
	///		behaviour of the entity
	/// </summary>
	protected IEnumerator _behaviourMove;
	protected PoolComponent _poolComponent;
	public void Awake()
	{
		_poolComponent = GetComponent<PoolComponent>();
		_behaviourMove = BehaviourMoveEnum();
	}

	public void Init(T Config, int IndexStruct, Vector3 Position)
	{
		if (_poolComponent != null)
			_poolComponent.OnResetToPool += OnResetToPool;

		InitData(Config, IndexStruct);
		GameObject instancePool;
		if (!PoolManager.Instance.GetObject(_data.GetIDPoolObject(), out instancePool))
		{
			throw new System.Exception();
		}
		CurrentBody = instancePool.GetComponent<BaseEntity>();

		if (CurrentBody == null)
			throw new System.Exception();

		CurrentBody.SetPosition(Position);
		CurrentBody.Init(transform, GetType().ToString() == "PlayerBehaviour");
		CurrentBody.EventCollision += OnCollision;
		GameplayManager.Instance.AddBehaviourEntity(ApplyBehaviour);
	}

	protected void InitData(T Data, int IndexStruct)
	{
		_data.Init(Data, IndexStruct);
	}


	public virtual void StartLogic()
	{

	}
	/// <summary>
	///		Behaviour which iterate the behaviour catched
	/// </summary>
	public virtual void ApplyBehaviour()
	{
		_behaviourMove.MoveNext();
	}

	/// <summary>
	///		Behaviour for moving entity
	/// </summary>
	/// <returns></returns>
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

	public void SelfDestroy(bool DestroyBody)
	{
		if (DestroyBody && CurrentBody)
		{
			CurrentBody.SelfDestroy();
		}
		DestroyLinkToBody();

		if (_poolComponent != null)
			_poolComponent.BackToPool();
		else
		{
			GameplayManager.Instance.RemoveBehaviourEntity(ApplyBehaviour);
			Destroy(gameObject);
		}
	}

	protected virtual void DestroyLinkToBody()
	{
		if (CurrentBody != null)
		{
			CurrentBody.EventCollision -= OnCollision;
			CurrentBody = null;
		}
	}

	protected virtual void OnResetToPool()
	{
		DestroyLinkToBody();
		GameplayManager.Instance.RemoveBehaviourEntity(ApplyBehaviour);
		if (_poolComponent != null)
			_poolComponent.OnResetToPool -= OnResetToPool;
	}


}
