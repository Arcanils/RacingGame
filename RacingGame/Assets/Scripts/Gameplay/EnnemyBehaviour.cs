using UnityEngine;
using System.Collections;

public class EnnemyBehaviour : MonoBehaviour {

	private BaseEntity _currentBody;
	private CarEnnemyData _data;

	public void Init(CarEnnemyData Data, Vector3 Position)
	{
		_data.Init(Data);
		GameObject instancePool;
		if (!PoolManager.Instance.GetObject(_data.CurrentCarType.ToString() + "Part", out instancePool))
		{
			throw new System.Exception();
		}

		_currentBody = instancePool.GetComponent<BaseEntity>();

		if (_currentBody == null)
			throw new System.Exception();

		_currentBody.SetPosition(Position);
		_currentBody.Init(transform);
		_currentBody.EventCollision += OnCollision;

		StartCoroutine(BehaviourEnum());
	}

	public IEnumerator BehaviourEnum()
	{
		while (true)
		{
			_currentBody.Move(new Vector3(0f,0f, _data.CurrentSpeedZ));
			yield return null;
		}
	}

	public void OnCollision(Collision collision)
	{
		var tagObjCollided = collision.transform.tag;
		if (tagObjCollided == "Player")
		{
			PlayerBehaviour.Instance.HitByEnnemy(this);
			_currentBody.TransformIntoProjecticle(collision.transform.position);
		}
	}

	public int GetValueDmg()
	{
		return _data.Damage;
	}
}
