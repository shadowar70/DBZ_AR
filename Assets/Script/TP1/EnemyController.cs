using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    public GameObject prefabHit;
    public GameObject prefabFireBall;
    public GameObject spawnerFireBall;
    private Animator animBody;
    private bool change = false;
    private GameObject target;
    public int life;
	// Use this for initialization
	void Start () {
        animBody = gameObject.GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player");
        InvokeRepeating("AttackFireBall", 8, 5);
    }
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(new Vector3(target.transform.position.x, 0 , target.transform.position.z));
        if(life <= 0) {
            Debug.Log("WIN");
            Destroy(gameObject);
        }

	}

    void AttackFireBall() {
        animBody.SetBool("isAttacking", true);
        Invoke("CleanState", 1);
        GameObject fireBall = Instantiate(prefabFireBall, spawnerFireBall.transform.position, Quaternion.identity);
        fireBall.transform.LookAt(target.transform);
        Destroy(fireBall, 10);
        
    }

    void CleanState() {
        animBody.SetBool("isAttacking", false);
        animBody.SetBool("isHitBody", false);
        animBody.SetBool("isHitHead", false);

    }


    //private void OnCollisionEnter(Collision collision) {

    //    if (collision.gameObject.CompareTag("FireBall")) {
    //        animBody.SetBool("isHitBody", true);
    //        life -= 1;
    //        Invoke("CleanState", 1);
    //        Destroy(collision.gameObject.transform.parent.gameObject);
    //        Destroy(Instantiate(prefabHit, collision.gameObject.transform.position, Quaternion.identity),1);

    //    }

    //}

    private void OnTriggerEnter(Collider other) {

        if (other.CompareTag("FireBall")) {
            animBody.SetBool("isHitBody" , true);
            life -= 1;
            Invoke("CleanState" , 1);
            Destroy(other.transform.parent.gameObject);
            Destroy(Instantiate(prefabHit , other.transform.position , Quaternion.identity) , 1);

        }
    }
}
