using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KamehaController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Destroy(gameObject, 10);
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(transform.forward*30*Time.deltaTime);
    }
}
