using UnityEngine;
using System.Collections;

public class EnnemyBehaviour : EntityBehaviour<EnnemyData>
{ 
	protected override void OnCollision(Collision collision)
	{
		var entity = collision.transform.GetComponent<BaseEntity>();
		if (entity != null && entity.IsPlayer)
		{
			PlayerManager.Instance.InstanceP.HitByEnnemy(this);
			CurrentBody.TransformIntoProjecticle(collision.transform.position);
		}
	}

	public int GetValueDmg()
	{
		return _data.CarData.Damage;
	}
}
