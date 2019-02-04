using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flashingLight : MonoBehaviour {

	public Light fLight;
	public GameObject fLightGO;

	// Use this for initialization
	void Start () {

		StartCoroutine(flicker());
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator flicker () {

		while (fLight != null){
			fLight.enabled = !fLight.enabled;
			fLightGO.SetActive(fLight.enabled);
			yield return new WaitForSeconds(0.2f);
		}

	}
}
