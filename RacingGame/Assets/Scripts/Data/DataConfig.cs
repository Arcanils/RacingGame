using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
///		Holder of the config of the game
/// </summary>
[CreateAssetMenu(fileName = "Config", menuName = "Scriptable/Config", order = 1)]
public class DataConfig : ScriptableObject
{ 
	public AreaConfig AreaData;
	public EnnemyScriptable EnnemyData;
	public PlayerScriptable PlayerData;
	public BonusScriptable BonusData;
	public CameraConfig CameraData;
	
}


/// <summary>
///		Config of the road
/// </summary>
[System.Serializable]
public struct AreaConfig
{
	public float SizeRoadPart;
	public float DistanceFromPlayerBeforeMovePart;
	public int RoadPartLength;
}



/// <summary>
///		Config of the camera
/// </summary>
[System.Serializable]
public struct CameraConfig
{

}



