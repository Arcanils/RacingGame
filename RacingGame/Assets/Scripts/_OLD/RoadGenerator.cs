using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoadGenerator : MonoBehaviour {

    public GameObject PrefabRoad;
    public int PreInit = 20;
    public Transform Container;
    public Vector3 OffsetRoad = new Vector3(0f, 0f, 20f);
    public float RepeatTime = 5f;

    private List<Transform> _roadAlive = new List<Transform>();

    public void Awake()
    {
        foreach (Transform child in Container)
        {
            Destroy(child.gameObject);
        }
        InitRoad();
        StartCoroutine(CircleRoadEnum());
    }

    private void InitRoad()
    {
        Vector3 LastPositionRoad = Container.position - OffsetRoad;
        for (int i = 0; i < PreInit; i++)
        {
            var instance = (AddRoadPart(LastPositionRoad));
            LastPositionRoad = instance.position;
            _roadAlive.Add(instance);
        }
    }

    private IEnumerator CircleRoadEnum()
    {
        int currentFirst = 0;
        Transform transPlayer = GameObject.FindObjectOfType<PlayerController>().transform;
        Vector3 LastPositionRoad = _roadAlive[_roadAlive.Count - 1].transform.position;
        while (true)
        {
            while (transPlayer.position.z < _roadAlive[currentFirst].position.z + 20)
                yield return null;
            LastPositionRoad += OffsetRoad;
            _roadAlive[currentFirst].position = LastPositionRoad;
            currentFirst = (currentFirst + 1) % PreInit; 
        }
    }

    private Transform AddRoadPart(Vector3 LastPosition)
    {
        var go = GameObject.Instantiate(PrefabRoad, Container) as GameObject;
        var transGo = go.transform;
        transGo.position = LastPosition + OffsetRoad;

        return transGo;
    }
}
