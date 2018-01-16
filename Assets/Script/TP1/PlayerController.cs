using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    public int life;
    public GameObject imageHit;
    public GameObject textLose;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("FireBall")) {
            imageHit.SetActive(true);
            life -= 1;
            Destroy(other.gameObject);
            if (life <= 0) {
                textLose.SetActive(true);
                gameObject.GetComponent<FighterBehaviour>().enabled = false;
                gameObject.GetComponent<InterfaceTP2>().enabled = false;
                Destroy(GameObject.Find("ModelVegeta"));
            }
        }
    }
}
