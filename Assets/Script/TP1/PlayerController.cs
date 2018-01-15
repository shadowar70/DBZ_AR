using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public int life;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(life <= 0) {
            Debug.Log("GAME OVER");
        }
	}

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("FireBall")) {
            life -= 1;
            Destroy(other.gameObject);
        }
    }
}
