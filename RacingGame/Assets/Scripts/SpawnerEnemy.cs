using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnerEnemy : MonoBehaviour {

    public GameObject PrefabEnnemy;
    public float TimeInternPattern = 0.5f;
    public float TimeRepeatPattern = 1.0f;

    public float OffsetFromPlayer = 100f;

    public List<Transform> ListSpawnerContainer;

    public bool ModeAleatoire = true;

    public int[,] Pattern = new int[,]
    {
        {0 , 1 , 0, 1},
        {0 , 1 , 0, 1},
        {0 , 0 , 0, 0},
        {0 , 0 , 0, 0},
        {1 , 0 , 1, 0},
        {1 , 0 , 1, 0},
        {1 , 0 , 1, 0},
    };


    public void Start()
    {
        StartCoroutine(ModeAleatoire ? SpawnAleatoire() : SpawnPattern());
    }


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

    public float LuckSpawnDouble = 0.1f;
    public float TimeBeetweenSpawnAlea = 0.2f;
    private bool[] _previousSpawn = new bool[4];
    private bool[] _nextSpawn = new bool[4];
    private Transform _transPlayer;
    

    private IEnumerator SpawnAleatoire()
    {
        yield return new WaitForSeconds(1f);
        float currentCoefSpawnDouble = 0f;
        float randValue = 0.0f;
        _transPlayer = GameObject.FindGameObjectWithTag("Player").transform;

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
                currentCoefSpawnDouble += LuckSpawnDouble;
            }

            yield return new WaitForSeconds(TimeBeetweenSpawnAlea);
        }
    }

    private void Spawn(int Spawn)
    {
        for (int i = 0, nSpawn = 0, indexSpawn = Random.Range(0, 4); i < 4; i++)
        {
            indexSpawn = (indexSpawn + i) % 4;
            if (_previousSpawn[indexSpawn] || nSpawn >= Spawn)
            {
                _nextSpawn[indexSpawn] = false;
                continue;
            }
            else
            {
                _nextSpawn[indexSpawn] = true;
                ++nSpawn;
                GameObject go = GameObject.Instantiate(PrefabEnnemy, ListSpawnerContainer[indexSpawn]) as GameObject;
                var goTrans = go.transform;
                goTrans.position = new Vector3(ListSpawnerContainer[indexSpawn].position.x, ListSpawnerContainer[indexSpawn].position.y, _transPlayer.position.z + OffsetFromPlayer);
            }
        }
        _previousSpawn = _nextSpawn;
        _nextSpawn = new bool[4];
    }
    
}
