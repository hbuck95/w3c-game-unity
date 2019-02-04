using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class towerDefEnemy : MonoBehaviour {

    public NavMeshAgent agent;
    public GameObject endPoint;
    private int health;

	// Use this for initialization
	void Start () {

        endPoint = GameObject.Find("endPoint");
        agent.destination = endPoint.transform.position;
        health = 2;
		
	}
	
	// Update is called once per frame
	void Update () {

        if (health <= 0)
        {
            waveManager.enemyCount += 1;
            Destroy(gameObject);
        }

        if (transform.position.z <= 9){
            waveManager.Instance.endWave();
        }
		
	}

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bullet")
        {
            health -= 1;
        }
    }
}
