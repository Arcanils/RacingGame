using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Config", menuName = "Scriptable/Config", order = 1)]
public class DataConfig : ScriptableObject
{ 
	public AreaConfig AreaData;
	public EnnemyConfig EnnemyData;
	public PlayerConfig PlayerData;
	public CameraConfig CameraData;
}

[System.Serializable]
public struct AreaConfig
{
	public float SizeRoadPart;
	public float DistanceFromPlayerBeforeMovePart;
	public int RoadPartLength;
}

[System.Serializable]
public struct EnnemyConfig
{
	public List<CarEnnemyData> ListEnnemyData;
	public List<PhaseSpawnData> ListPhaseSpawnData;
	public int NumberOfColumns;
	public float SpawnDistanceFromPlayer;
	public bool ModeAleatoire;
	public float LuckSpawnDouble;
	public float TimeBeetweenSpawnAlea;
	public int LuckTruck;
	public int LuckMoto;

	public CarType GetRandType()
	{
		float rand = Random.Range(0, 100);

		if (rand < LuckMoto)
		{
			return CarType.Moto;
		}
		else
			return rand < (LuckMoto + LuckTruck) ? CarType.Truck : CarType.Car;
	}
}

[System.Serializable]
public struct PlayerConfig
{
	public List<CarPlayerData> ListPlayerData;
	public CarType StartPlayer;
}

[System.Serializable]
public struct CameraConfig
{

}
