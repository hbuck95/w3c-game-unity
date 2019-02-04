using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class turretManager : MonoBehaviour {

	public int sTurretAmount;
    public int bTurretAmount;
	public static turretManager Instance;
	public Text standardText, bombText;

	public bool canBuild;

	// Use this for initialization
	void Start () {

		canBuild = false;

		sTurretAmount = 4;
		bTurretAmount = 2;

		Instance = this;
		
	}
	
	// Update is called once per frame
	void Update () {
		
		standardText.text = "Standard Turret Amount: " + sTurretAmount;
        bombText.text = "Bomb Turret Amount: " + bTurretAmount;

	}
}
