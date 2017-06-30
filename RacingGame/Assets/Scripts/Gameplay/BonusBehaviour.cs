using UnityEngine;
using System.Collections;
using System;

/// <summary>
///		Controller of bonus
/// </summary>
public class BonusBehaviour : EntityBehaviour<BonusConfig, BonusData>
{
	protected override void OnCollision(Collision collision)
	{
		var entity = collision.transform.GetComponent<BaseEntity>();
		if (entity != null && entity.IsPlayer)
		{
			PlayerManager.Instance.InstanceP.AddBonus(_data);
		}
	}
}
