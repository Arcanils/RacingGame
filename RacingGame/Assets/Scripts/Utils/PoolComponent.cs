using UnityEngine;
using System.Collections;

public class PoolComponent : MonoBehaviour
{
	private Pool _creator;

	public System.Action OnResetToPool;

	private Coroutine _routineDelayed;

	public void InitFromPool(Pool Creator)
	{
		_creator = Creator;
	}

	public void BackToPool()
	{
		if (_routineDelayed != null)
			StopCoroutine(_routineDelayed);
		_routineDelayed = null;

		if (_creator != null)
			_creator.ReturnToPool(this);
	}

	public void BackToPool(float Time)
	{
		if (_routineDelayed != null)
		{
			Debug.LogError("Spam Delayed call to back pool");
			return;
		}

		_routineDelayed = StartCoroutine(WaitBeforeBackToPool(Time));
	}

	private IEnumerator WaitBeforeBackToPool(float Duration)
	{
		yield return new WaitForSeconds(Duration);
		BackToPool();
		_routineDelayed = null;
	}

	public void Reset()
	{
		if (OnResetToPool != null)
			OnResetToPool.Invoke();

		_routineDelayed = null;
		OnResetToPool = null;

	}
}
