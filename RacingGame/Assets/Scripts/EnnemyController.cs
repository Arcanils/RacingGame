using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class EnnemyController : MonoBehaviour
{

    public Vector3 VecSpeed;



    private Transform _trans;

    public void Awake()
    {
        _trans = transform;
    }

    public void Start()
    {
        StartCoroutine(MoveControlEnum());
    }


    private IEnumerator MoveControlEnum()
    {
        while (true)
        {
            _trans.position += Time.fixedDeltaTime * VecSpeed;
            yield return new WaitForFixedUpdate();
        }
    }

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
}
