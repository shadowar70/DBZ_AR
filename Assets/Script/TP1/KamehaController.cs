using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamehaController : MonoBehaviour {
    public float speed = 30;
	// Use this for initialization
	void Start () {
        Destroy(gameObject, 10);
	}
	
	// Update is called once per frame
	void Update () {
        transform.position += transform.forward * speed * Time.deltaTime;
    }
}
