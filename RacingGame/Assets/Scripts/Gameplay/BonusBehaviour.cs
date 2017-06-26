using UnityEngine;
using System.Collections;
using System;

public class BonusBehaviour : EntityBehaviour<BonusData>
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
