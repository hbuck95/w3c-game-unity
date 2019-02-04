using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour {

    public float speed;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        transform.position += transform.forward * Time.deltaTime * speed;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "NPC" || other.tag == "Floor")
        {
            Destroy(gameObject);
        }
    }
}
