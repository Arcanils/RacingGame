using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnnemyManager : MonoBehaviour {

	public static EnnemyManager Instance { get; private set; }

	public Transform Container;

	private EnnemyConfig _config;
	private bool[] _previousSpawn;
	private bool[] _nextSpawn;
	private Vector2[] _posSpawn;

	public void Awake()
	{
		Debug.LogError("!!");
		Instance = this;
	}

	public void Init(EnnemyConfig EConfig)
	{
		_config = EConfig;
		_posSpawn = new Vector2[_config.NumberOfColumns];
		_previousSpawn = new bool[_config.NumberOfColumns];
		_nextSpawn = new bool[_config.NumberOfColumns];

		for (int i = 0, j = - _config.NumberOfColumns / 2; i < _config.NumberOfColumns; i++)
		{
			_posSpawn[i] = new Vector2((RoadManager.SizeRoadPart * (j++)) / (float)_config.NumberOfColumns, Container.position.y);
		}
		//StartCoroutine(ModeAleatoire ? SpawnAleatoire() : SpawnPattern());
	}

	public void StartLogic()
	{
		StartCoroutine(SpawnAleatoire());
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

		while (true)
		{
			randValue = Random.Range(0f, 1f);
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

			yield return new WaitForSeconds(_config.TimeBeetweenSpawnAlea);
		}
	}

	private void Spawn(int Spawn)
	{
		for (int i = 0, nSpawn = 0, indexSpawn = Mathf.RoundToInt(Random.Range(0, _config.NumberOfColumns) / 2) * 2; i < _config.NumberOfColumns; i += 2)
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
				++nSpawn;
				GameObject go;

				if (!PoolManager.Instance.GetObject("Ennemy", out go))
					throw new System.Exception();

				var goTrans = go.transform;

				var script = go.GetComponent<EnnemyBehaviour>();
				if (!script)
					throw new System.Exception();
				var type = _config.GetRandType();
				script.Init(_config.ListEnnemyData.Find(element => element.CurrentCarType == type), 
					new Vector3(_posSpawn[indexSpawn].x, _posSpawn[indexSpawn].y, PlayerBehaviour.Instance.TransPlayer.position.z + _config.SpawnDistanceFromPlayer));
			}
		}
		_previousSpawn = _nextSpawn;
		_nextSpawn = new bool[_config.NumberOfColumns];
	}
}
