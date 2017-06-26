using UnityEngine;
using System.Collections;
using System;

public class BonusBehaviour : EntityBehaviour<BonusConfig>
{

	protected BonusData _data;
	
	

	public override void Init(BonusConfig Data, Vector3 Position)
	{
		base.Init(Data, Position);
		//_data = _config.ListBonus.Find(element => element.CurrentBonus == BonusWanted);
	}

	protected override void OnCollision(Collision collision)
	{
		var entityTag = collision.transform.tag;

		if (entityTag == "Player")
		{
			PlayerBehaviour.Instance.AddBonus(_data);
		}
	}
}
