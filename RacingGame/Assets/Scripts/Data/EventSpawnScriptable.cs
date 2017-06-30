using UnityEngine;
using System.Collections;

/// <summary>
///		Holder LD spawn (not implemented / used)
/// </summary>
[CreateAssetMenu(fileName = "EventSpawn", menuName = "Scriptable/EventSpawn", order = 1)]
public class EventSpawnScriptable : ScriptableObject {

	public EventSpawnData Data;

	public EventSpawnScriptable()
	{

	}
}

public enum TypeSpawn
{
	Empty,
	ObstacleA,
	ObstacleB,
	ObstacleC,
	Car,
	Truck,
	Moto,
	Coin,
	Bonus,
}

[System.Serializable]
public struct LineSpawnData
{
	public TypeSpawn[] DataToSpawn;
	public float TimeAfterSpawn;
}

[System.Serializable]
public struct EventSpawnData
{
	public string IDEvent;
	public LineSpawnData[] LinesSpawnData;
	public int ScoreBonus;
}

public struct PhaseSpawnData
{
	public EventSpawnScriptable Event;
	public float DelayBeforeNextEvent;
}