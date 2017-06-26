using UnityEngine;
using System.Collections;

public interface EntityConfig
{
	/*
	void Init(EntityConfig Config);
	string GetIDPoolObject();
	float GetSpeedZ();
	*/
}

public interface EntityData
{
	string GetIDPoolObject();
	float GetSpeedZ();
	void Init(EntityData Data);
	//void Init(T Config, int IndexConfig);
}