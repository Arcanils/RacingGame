using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
///		Ennemy's Manager 
/// </summary>
public class EnnemyManager : MonoBehaviour {

	public static EnnemyManager Instance { get; private set; }
	

	public Transform Container;

	private EnnemyConfig _config;
	private bool[] _previousSpawn;
	private bool[] _nextSpawn;
	private Vector2[] _posSpawn;
	
	private EntityPlacementManager _entityPlacementManager;

	public void Awake()
	{
		Instance = this;
	}

	public void Init(EnnemyConfig EConfig)
	{
		_config = EConfig;
		_entityPlacementManager = new EntityPlacementManager(Mathf.CeilToInt(_config.NumberOfColumns / 2f));
		_posSpawn = new Vector2[_config.NumberOfColumns];
		_previousSpawn = new bool[_config.NumberOfColumns];
		_nextSpawn = new bool[_config.NumberOfColumns];

		for (int i = 0, j = - _config.NumberOfColumns / 2; i < _config.NumberOfColumns; i++)
		{
			_posSpawn[i] = new Vector2((RoadManager.SizeRoadPart * (j++)) / (float)(_config.NumberOfColumns + 1), Container.position.y);
		}
		//StartCoroutine(ModeAleatoire ? SpawnAleatoire() : SpawnPattern());
	}

	public void StartLogic()
	{
		StartCoroutine(SpawnAleatoire());
	}

	public bool IsNearCar(EnnemyBehaviour ennemyBehaviour)
	{
		if (ennemyBehaviour == null || ennemyBehaviour.CurrentBody == null)
			return false;
		else
			return (_entityPlacementManager.IsNearCar(ennemyBehaviour));
	}

	public float GetCarFrontSpeed(EnnemyBehaviour ennemyBehaviour)
	{
		return (_entityPlacementManager.GetCarFrontSpeed(ennemyBehaviour));
	}
	public EnnemyBehaviour GetFrontCar(EnnemyBehaviour ennemyBehaviour)
	{
		return (_entityPlacementManager.GetFrontCar(ennemyBehaviour));
	}

	/*
private IEnumerator SpawnPattern()
{
	int indexInternPattern = 0;
	int internPatternLength = Pattern.GetLength(0);
	Debug.LogError(internPatternLength);
	while (true)
	{
		SpawnInternPattern(indexInternPattern++);
		if (indexInternPattern >= internPatternLength)
		{
			indexInternPattern = 0;
			yield return new WaitForSeconds(TimeRepeatPattern);
		}
		else
			yield return new WaitForSeconds(TimeInternPattern);
	}
}

private void SpawnInternPattern(int IndexInternPattern)
{
	for (int i = 0; i < 4; i++)
	{
		if (Pattern[IndexInternPattern, i] == 1)
		{
			var go = GameObject.Instantiate(PrefabEnnemy, ListSpawnerContainer[i], false) as GameObject;
			var transGo = go.transform;
			transGo.localPosition = Vector3.zero;
		}
	}
}
*/


	private IEnumerator SpawnAleatoire()
	{
		float currentCoefSpawnDouble = 0f;
		float randValue = 0.0f;
		float LastPosZ;

		while (true)
		{
			randValue = UnityEngine.Random.Range(0f, 1f);
			//Debug.LogError(randValue + " " + currentCoefSpawnDouble);
			if (randValue < currentCoefSpawnDouble)
			{
				Spawn(2);
				currentCoefSpawnDouble = 0f;
			}
			else
			{
				Spawn(1);
				currentCoefSpawnDouble += _config.LuckSpawnDouble;
			}

			LastPosZ = PlayerManager.Instance.PositionPlayer.z;

			yield return new WaitWhile(() => PlayerManager.Instance.PositionPlayer.z - LastPosZ <= _config.DistanceBeetweenSpawn);
		}
	}

	private void Spawn(int Spawn)
	{
		for (int i = 0, nSpawn = 0, indexSpawn = Mathf.RoundToInt(UnityEngine.Random.Range(0, _config.NumberOfColumns) / 2) * 2; i < _config.NumberOfColumns; i += 2)
		{
			indexSpawn = Mathf.RoundToInt(((indexSpawn + i) % _config.NumberOfColumns) / 2) * 2;
			if (_previousSpawn[indexSpawn] || nSpawn >= Spawn)
			{
				_nextSpawn[indexSpawn] = false;
				continue;
			}
			else
			{
				_nextSpawn[indexSpawn] = true;
				CreateEnnemy(indexSpawn);
				++nSpawn;
			}
		}
		_previousSpawn = _nextSpawn;
		_nextSpawn = new bool[_config.NumberOfColumns];
	}

	private EnnemyBehaviour CreateEnnemy(int IndexSpawnColumn)
	{
		GameObject go;

		if (!PoolManager.Instance.GetObject("Ennemy", out go))
			throw new System.Exception();

		var goTrans = go.transform;
		goTrans.SetParent(Container);

		var script = go.GetComponent<EnnemyBehaviour>();
		if (!script)
			throw new System.Exception();
		var type = _config.GetRandType();
		script.ColumnCarIndex = IndexSpawnColumn;
		script.Init(_config, (int)type,
			new Vector3(_posSpawn[IndexSpawnColumn].x,
			_posSpawn[IndexSpawnColumn].y,
			PlayerManager.Instance.PositionPlayer.z + _config.SpawnDistanceFromPlayer));
		script.StartLogic();
		_entityPlacementManager.AddEntity(script);

		return script;
	}

	public void RefreshData()
	{
		_entityPlacementManager.RefreshData();
	}
}

/// <summary>
///		Holder ennemies ref next to a ennemy ref
/// </summary>
public class EntityPlacement
{
	public enum TypeRefPlacement
	{
		FrontLeft,
		FrontCenter,
		FrontRight,
		BehindLeft,
		BehindCenter,
		BehindRight,
	}
	public EnnemyBehaviour RefEntity;
	public EnnemyBehaviour[] RefNearEntities;

	public EntityPlacement(EnnemyBehaviour RefEntity)
	{
		this.RefEntity = RefEntity;
		RefNearEntities = new EnnemyBehaviour[6];
	}

	public Vector3 GetPos()
	{
		return RefEntity.CurrentBody.transform.position;
	}
}

/// <summary>
///		Holder order beetween ennemy spawned
/// </summary>
public class EntityPlacementManager
{
	public List<EntityPlacement>[] EntitiesPlacement;

	public EntityPlacementManager(int NumberOfCarColumns)
	{
		EntitiesPlacement = new List<EntityPlacement>[NumberOfCarColumns];
		for (int i = 0; i < NumberOfCarColumns; i++)
		{
			EntitiesPlacement[i] = new List<EntityPlacement>();
		}
	}

	public void AddEntity(EnnemyBehaviour Entity)
	{
		EntitiesPlacement[Mathf.CeilToInt(Entity.ColumnCarIndex / 2f)].Add(new EntityPlacement(Entity));
	}

	

	public void RefreshData()
	{
		RemoveCarAway();

		for (int i = 0, iLength = EntitiesPlacement.Length; i < iLength; i++)
		{
			for (int j = 0, jLength = EntitiesPlacement[i].Count; j < jLength; j++)
			{
				EntitiesPlacement[i][j].RefNearEntities[1] = (j != jLength - 1) ? EntitiesPlacement[i][j + 1].RefEntity : null;
				EntitiesPlacement[i][j].RefNearEntities[4] = (j != 0) ? EntitiesPlacement[i][j - 1].RefEntity : null;
			}
		}
	}

	public void RemoveCarAway()
	{
		float PosPlayerZ = PlayerManager.Instance.PositionPlayer.z;
		for (int i = 0, iLength = EntitiesPlacement.Length; i < iLength; i++)
		{
			int j = 0;
			while (j < EntitiesPlacement[i].Count)
			{
				if (EntitiesPlacement[i][j] == null || EntitiesPlacement[i][j].RefEntity == null ||
					!EntitiesPlacement[i][j].RefEntity.gameObject.activeInHierarchy)
					EntitiesPlacement[i].RemoveAt(j);
   				else if (EntitiesPlacement[i][j].GetPos().z + 20f < PosPlayerZ)
				{
					EntitiesPlacement[i][j].RefEntity.SelfDestroy(true);
					EntitiesPlacement[i].RemoveAt(j);
				}
				else
					break;
			}
		}
	}

	public float GetCarFrontSpeed(EnnemyBehaviour ennemyBehaviour)
	{
		int indexColumn = Mathf.CeilToInt(ennemyBehaviour.ColumnCarIndex / 2f);
		var item = EntitiesPlacement[indexColumn].Find(element => element.RefEntity == ennemyBehaviour);

		if (item == null)
			return 0f;
		else
			return item.RefNearEntities[1] != null ? item.RefNearEntities[1].GetSpeed() : 0f;
	}


	public EnnemyBehaviour GetFrontCar(EnnemyBehaviour ennemyBehaviour)
	{
		int indexColumn = Mathf.CeilToInt(ennemyBehaviour.ColumnCarIndex / 2f);
		var item = EntitiesPlacement[indexColumn].Find(element => element.RefEntity == ennemyBehaviour);

		if (item == null)
			return null;
		else
			return item.RefNearEntities[1];
	}

	public bool IsNearCar(EnnemyBehaviour ennemyBehaviour)
	{
		int indexColumn = Mathf.CeilToInt(ennemyBehaviour.ColumnCarIndex / 2f);
		var item = EntitiesPlacement[indexColumn].Find(element => element.RefEntity == ennemyBehaviour);

		if (item == null)
			return false;
		else
			return item.RefNearEntities[1] != null  && item.RefNearEntities[1].CurrentBody != null ? 
				item.RefNearEntities[1].CurrentBody.transform.position.z - ennemyBehaviour.CurrentBody.transform.position.z <= 25 :
				false;
	}
}
