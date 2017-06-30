using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
///		Pool manager. Use it for evade GC
/// </summary>
public class PoolManager : MonoBehaviour
{
	public static PoolManager Instance { get; private set; }

	public List<Pool> PoolObjects = new List<Pool>();

	private Dictionary<string, Pool> _dicoPools = new Dictionary<string, Pool>();

	public void Awake()
	{
		Instance = this;
	}

	public void Init()
	{
		string ID;
		for (int i = 0, iLength = PoolObjects.Count; i < iLength; i++)
		{
			if (PoolObjects[i] == null || string.IsNullOrEmpty(PoolObjects[i].IDPool))
				continue;

			ID = PoolObjects[i].IDPool;

			PoolObjects[i].InitPool(transform);
			_dicoPools[ID] = PoolObjects[i];
		}
	}

	public bool GetObject(string IDPool, out GameObject InstanceFromPool)
	{
		if (string.IsNullOrEmpty(IDPool) || !_dicoPools.ContainsKey(IDPool))
		{
			InstanceFromPool = null;
			return false;
		}

		InstanceFromPool = _dicoPools[IDPool].GetInstance();

		return InstanceFromPool != null;
	}
}

/// <summary>
///		Pool of prefab object set
/// </summary>
[System.Serializable]
public class Pool
{
	public string IDPool;
	public GameObject Prefab;
	public int NObject;


	private Queue<GameObject> _listObjs = new Queue<GameObject>();
	private Transform _container;
	private bool _canBeReturnedToPool;

	public void InitPool(Transform ContainerPool)
	{
		CreateContainerPool(ContainerPool);
		_canBeReturnedToPool = Prefab != null && Prefab.GetComponent<PoolComponent>() != null;
		for (int i = 0; i < NObject; i++)
		{
			_listObjs.Enqueue(CreateObject(false));
		}
	}

	private void CreateContainerPool(Transform Parent)
	{
		GameObject go = new GameObject("__Pool_" + IDPool + "__");
		_container = go.transform;
		_container.parent = Parent;
	}

	private GameObject CreateObject(bool InitObject)
	{
		var instance = GameObject.Instantiate(Prefab, _container, false) as GameObject;
		var instanceTrans = instance.transform;
		instanceTrans.position = Vector3.zero;

		instance.SetActive(InitObject);

		if (_canBeReturnedToPool)
		{
			var script = instance.GetComponent<PoolComponent>();
			script.InitFromPool(this);
		}
		//_listObjs.Enqueue(instance);

		return instance;
	}

	public GameObject GetInstance()
	{
		GameObject instance = null;

		if (_listObjs.Count != 0)
		{
			instance = _listObjs.Dequeue();
			instance.SetActive(true);
		}
		else
			instance = CreateObject(true);

		return instance;
	}

	public void ReturnToPool(PoolComponent ObjectPooled)
	{
		ObjectPooled.Reset();
		ObjectPooled.gameObject.SetActive(false);
		ObjectPooled.transform.SetParent(_container);
		_listObjs.Enqueue(ObjectPooled.gameObject);
	}
}
