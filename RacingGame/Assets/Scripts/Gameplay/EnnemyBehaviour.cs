using UnityEngine;
using System.Collections;
using System;

/// <summary>
///		Ennemy's Controller
/// </summary>
public class EnnemyBehaviour : EntityBehaviour<EnnemyConfig, EnnemyData>
{
	public int ColumnCarIndex;
	/// <summary>
	///		Is reducing speed from origine for avoiding collision
	/// </summary>
	public bool SlowState { get; private set; }

	private IEnumerator _slowBehaviour;

	protected override void OnCollision(Collision collision)
	{
		var entity = collision.transform.GetComponent<BaseEntity>();
		if (entity != null && entity.IsPlayer)
		{
			PlayerManager.Instance.InstanceP.HitByEnnemy(this);
			CurrentBody.TransformIntoProjecticle(collision.transform.position);
			CurrentBody.transform.SetParent(transform.parent);
			SelfDestroy(false);
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
			SlowState = true;
			_slowBehaviour = ApplySlowBehaviour(0.75f);
		}

		if (_slowBehaviour != null)
			_slowBehaviour.MoveNext();
		base.ApplyBehaviour();
	}

	/// <summary>
	///		Behaviour of the slow process
	/// </summary>
	/// <param name="DurationSlow"></param>
	/// <returns></returns>
	private IEnumerator ApplySlowBehaviour(float DurationSlow)
	{
		float begSpeed = _data.CarData.CurrentSpeedZ;
		EnnemyBehaviour target = EnnemyManager.Instance.GetFrontCar(this);
		if (target == null)
			yield break;
		for (float t = 0f , perc = 0f; perc < 1f; t += Time.deltaTime)
		{
			perc = Mathf.Clamp01(t / DurationSlow);
			_data.CarData.CurrentSpeedZ = Mathf.Lerp(begSpeed, target.GetSpeed(), perc);
			yield return null;
		}
		while (target != null)
		{
			_data.CarData.CurrentSpeedZ = target.GetSpeed();
			yield return null;
		}
		_slowBehaviour = null;
	}

	public float GetSpeed()
	{
		return _data.CarData.CurrentSpeedZ;
	}

	public CarType GetCarType()
	{
		return CurrentBody.Type;
	}

	protected override void OnResetToPool()
	{
		SlowState = false;
		_slowBehaviour = null;
		base.OnResetToPool();
	}
}
