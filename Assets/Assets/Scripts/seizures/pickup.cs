using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickup : MonoBehaviour {

	public int spinSpeed;

    public GameObject shield;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		transform.Rotate(Vector3.down * spinSpeed);
		
	}

	void OnTriggerEnter(Collider other) {

        switchInteraction._bothPartsPickedUp++;

        if (switchInteraction._bothPartsPickedUp == 2)
            shield.SetActive(true);

        switchInteraction._onePartPickedUp = true;

		if (other.tag == "Player"){
			Destroy(gameObject);
		}

	}
}
