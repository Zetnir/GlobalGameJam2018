using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunchCollider : MonoBehaviour {

    public PlayerController Player;
    public Vector3 attackDirection;
    public float attackForce = 30;
    public float attackTime = 0f;
    public float attackDuration = 0.5f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Player = GetComponentInParent<PlayerController>();
        if (Time.time - attackTime > attackDuration)
        {
            Player.gameObject.GetComponent<PlayerController>().enabled = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Ball")
        {
            attackDirection = (other.transform.position - Player.transform.position).normalized;
            attackDirection.y = 0;
            attackDirection *= attackForce;

            attackTime = Time.time;
            Player.gameObject.GetComponent<PlayerController>().powerJauge += 10f;
            Player.gameObject.GetComponent<PlayerController>().enabled = false;
            other.GetComponent<BallScript>().BallAttack(attackDirection, Player.numController);


            //other.GetComponent<Rigidbody>().AddForce(attackDirection * other.GetComponent<BallScript>().powerStack, ForceMode.Impulse);
        }
    }

    void OnTriggerExit(Collider other)
    {

    }

    void OnTriggerStay(Collider other)
    {

    }
}
