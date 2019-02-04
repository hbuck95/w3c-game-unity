using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : bullet {

    public GameObject Explosion;

    private void OnTriggerEnter(Collider other)
    {

        if (other.tag == "NPC" || other.tag == "Floor")
        {
            Instantiate(Explosion, transform.position, transform.rotation);
            Destroy(gameObject);
        }

    }
	
}
