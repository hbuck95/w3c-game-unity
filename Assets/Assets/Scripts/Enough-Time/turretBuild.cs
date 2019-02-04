using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class turretBuild : MonoBehaviour {

	public float timeToBuild;
	private string timeToBuildString;
	public TextMesh text;
	public GameObject standardTurret, buildText;
	public Transform standardTurretPos;

	// Use this for initialization
	void Start () {

		turretManager.Instance.canBuild = false;
		
	}
	
	// Update is called once per frame
	void Update () {

		timeToBuild -= Time.deltaTime;

		timeToBuildString = Mathf.Round(timeToBuild).ToString();

		text.text = timeToBuildString;

		if (timeToBuild <= 0){
			turretManager.Instance.canBuild = true;
			standardTurret.SetActive(true);
			buildText.SetActive(false);
		}
		
	}
}
