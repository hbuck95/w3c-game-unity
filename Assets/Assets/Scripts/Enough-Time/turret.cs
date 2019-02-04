using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turret : MonoBehaviour {

    private GameObject NPC;
    public GameObject Bullet;
    private float _enDist;
    private float nextFire;
    public float fireRate;
    public float range;

    public GameObject FindClosestEnemy()
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("NPC");
        GameObject closest = null;
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }

    // Use this for initialization
    public void Start () {

	}
	
	// Update is called once per frame
	public void Update () {


        if (FindClosestEnemy() != null)
        {
            transform.LookAt(FindClosestEnemy().transform);
        }
        else
            transform.rotation = Quaternion.identity;

        if (FindClosestEnemy() != null)
            _enDist = Vector3.Distance(transform.position, FindClosestEnemy().transform.position);
        else
            _enDist = range + 1;

        if (_enDist < range)
        {
            if (Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                Shoot();
                
            }
        }
		

    }

    public void Shoot()
    {
        Instantiate(Bullet, transform.position, transform.rotation);
    }
}
