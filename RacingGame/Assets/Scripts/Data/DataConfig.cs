using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "Config", menuName = "Scriptable/Config", order = 1)]
public class DataConfig : ScriptableObject
{ 
	public AreaConfig AreaData;
	public EnnemyScriptable EnnemyData;
	public PlayerScriptable PlayerData;
	public BonusScriptable BonusData;
	public CameraConfig CameraData;

	[System.NonSerialized]
	public float Currency = 100000;
}

[System.Serializable]
public struct AreaConfig
{
	public float SizeRoadPart;
	public float DistanceFromPlayerBeforeMovePart;
	public int RoadPartLength;
}




[System.Serializable]
public struct CameraConfig
{

}



