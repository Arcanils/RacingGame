using UnityEngine;
using System.Collections;

public class AutoDestru : MonoBehaviour {

    public float DurationBeforeDestru = 30f;
	void Start ()
    {
        Destroy(gameObject, DurationBeforeDestru);
	}
	
}
