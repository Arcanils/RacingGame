using UnityEngine;
using System.Collections;


public interface EntityConfig
{

}

public interface EntityData<T> where T : EntityConfig
{
	string GetIDPoolObject();
	float GetSpeedZ();
	void Init(T Data, int IndexStruct);
	//void Init(T Config, int IndexConfig);
}