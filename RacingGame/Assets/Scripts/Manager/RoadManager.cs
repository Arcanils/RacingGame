using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoadManager : MonoBehaviour {

	public static RoadManager Instance { get; private set; }
	
	public Transform Container;

	public static float SizeRoadPart;
	public string IDPrefab = "RoadPart";
	
	private float _distanceFromPlayer = 20f;
	private Vector3 _offsetRoad = new Vector3(0f, 0f, 20f);
	private List<Transform> _roadAlive = new List<Transform>();
	private int _roadPartLength;

	public void Awake()
	{
		Instance = this;
		foreach (Transform child in Container)
		{
			Destroy(child.gameObject);
		}
	}

	public void Init(AreaConfig Config)
	{
		_distanceFromPlayer = Config.DistanceFromPlayerBeforeMovePart;
		SizeRoadPart = Config.SizeRoadPart;
		_offsetRoad = new Vector3(0f, 0f, Config.SizeRoadPart);
		_roadPartLength = Config.RoadPartLength;
		InitRoad();
	}

	public void StartLogic()
	{
		StartCoroutine(CircleRoadEnum());
	}

	private void InitRoad()
	{
		Vector3 LastPositionRoad = Container.position - _offsetRoad;
		for (int i = 0; i < _roadPartLength; i++)
		{
			var instance = (AddRoadPart(LastPositionRoad));
			LastPositionRoad = instance.position;
			_roadAlive.Add(instance);
		}
	}

	private IEnumerator CircleRoadEnum()
	{
		int currentFirst = 0;
		Vector3 LastPositionRoad = _roadAlive[_roadAlive.Count - 1].transform.position;
		while (true)
		{
			while (PlayerManager.Instance.PositionPlayer.z < _roadAlive[currentFirst].position.z + _distanceFromPlayer)
				yield return null;
			LastPositionRoad += _offsetRoad;
			_roadAlive[currentFirst].position = LastPositionRoad;
			currentFirst = (currentFirst + 1) % _roadPartLength;
		}
	}

	private Transform AddRoadPart(Vector3 LastPosition)
	{
		GameObject go = null;
		if (!PoolManager.Instance.GetObject("RoadPart", out go))
		{
			throw new System.Exception();
		}

		var transGo = go.transform;
		transGo.parent = Container;
		transGo.position = LastPosition + _offsetRoad;

		return transGo;
	}
}
