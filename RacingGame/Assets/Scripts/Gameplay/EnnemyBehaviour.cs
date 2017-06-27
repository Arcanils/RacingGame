using UnityEngine;
using System.Collections;
using System;

public class EnnemyBehaviour : EntityBehaviour<EnnemyData>
{
	public int ColumnCarIndex;
	public bool SlowState { get; private set; }

	private IEnumerator _slowBehaviour;

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

	public override void ApplyBehaviour()
	{
		if (!SlowState && EnnemyManager.Instance.IsNearCar(this))
		{
			Debug.LogError("!!");
			Debug.Break();
			SlowState = true;
			_slowBehaviour = ApplySlowBehaviour(1f);
		}

		if (_slowBehaviour != null)
			_slowBehaviour.MoveNext();
		base.ApplyBehaviour();
	}

	private IEnumerator ApplySlowBehaviour(float DurationSlow)
	{
		float begSpeed = _data.CarData.CurrentSpeedZ;
		float endSpeed = EnnemyManager.Instance.GetCarFrontSpeed(this);

		for (float t = 0f , perc = 0f; perc < 1f; t += Time.deltaTime)
		{
			if (EnnemyManager.Instance.IsNearCar(this))
				endSpeed = EnnemyManager.Instance.GetCarFrontSpeed(this);

			_data.CarData.CurrentSpeedZ = Mathf.Lerp(begSpeed, endSpeed, perc);
			yield return null;
		}

		_slowBehaviour = null;
	}

	public float GetSpeed()
	{
		return _data.CarData.CurrentSpeedZ;
	}
}
