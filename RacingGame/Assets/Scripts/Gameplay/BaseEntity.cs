﻿using UnityEngine;
using System.Collections;
using System;

public class BaseEntity : MonoBehaviour
{

	public Action<Collision> EventCollision;

	public CarType Type;
	public bool IsPlayer { get; private set; }

	private Rigidbody _rigid;
	private BoxCollider _col;
	private Transform _trans;

	private const float _demiLengthXRoad = 10f;

	public void SetPosition(Vector3 position)
	{
		_trans.position = position;
	}

	private float _limitX;

	public void Awake()
	{
		_trans = transform;
		_rigid = GetComponent<Rigidbody>();
		_col = GetComponent<BoxCollider>();
		_limitX = _demiLengthXRoad - _col.size.x / 2f;
	}

	public void Init(Transform Parent, bool IsPlayer = false)
	{
		this.IsPlayer = IsPlayer;
		_rigid.constraints = RigidbodyConstraints.FreezeAll;
		_trans.parent = Parent;
		tag = IsPlayer ? "Player" : "Ennemy";
	}

	public void Move(Vector3 VecSpeed)
	{
		var nextPosition = Time.deltaTime * VecSpeed + _trans.position;
		nextPosition = new Vector3(Mathf.Clamp(nextPosition.x, -_limitX, _limitX), nextPosition.y, nextPosition.z);
		_trans.position = nextPosition;
	}

	public void TransformIntoProjecticle(Vector3 PositionOther)
	{
		Debug.LogError("!!");
		_rigid.freezeRotation = false;
		_rigid.constraints = RigidbodyConstraints.None;

		Vector3 direction = _trans.position - PositionOther;
		_rigid.AddForceAtPosition((direction + Vector3.up).normalized * 1000, direction / 2f + _trans.position);
		_col.enabled = false;
	}

	/*
	private IEnumerator MoveControlEnum()
	{
		while (true)
		{
			yield return new WaitForFixedUpdate();
		}
	}

	/*
	public void OnCollisionEnter(Collision collision)
	{
		var tagObjCollided = collision.transform.tag;

		if (tagObjCollided == "Player")
		{
			if (!collision.transform.GetComponent<PlayerController>().DamageLogic())
				SceneManager.LoadScene(0);
			else
			{
				var rigid = GetComponent<Rigidbody>();

				Vector3 direction = transform.position - collision.transform.position;
				rigid.AddForceAtPosition((direction + Vector3.up).normalized * 1000, direction / 2f + transform.position);
				GetComponentInChildren<Collider>().enabled = false;
			}
		}
	}
	*/

	public void OnCollisionEnter(Collision collision)
	{
		if (EventCollision != null)
			EventCollision(collision);
	}
}
