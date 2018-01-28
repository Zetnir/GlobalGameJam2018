using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZonePlayer : MonoBehaviour {

    public int zoneIndex;
	// Use this for initialization
	void Start () {

    }

    // Update is called once per frame
    void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Ball")
        {
            other.GetComponent<BallScript>().zonePosition = zoneIndex;

        }
    }
}
