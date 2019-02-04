using UnityEngine;
using UnityEngine.UI;

public class UIOnAndOff : MonoBehaviour {
	public GameObject[] activate;
	public GameObject deactivate;
	public bool isActive;
	public Button bttn;

	private void Start () {
		isActive = false;
		bttn.onClick.AddListener(OnClickActive);	
	}
	
	private void Update () {
	    switch (isActive) {
	        case true:
	            activate[0].SetActive(true);
	            activate[1].SetActive(true);
	            break;
	        case false:
	            foreach (var t in activate) {
	                t.SetActive(false);
	            }
	            Debug.Log("boop");
	            break;
	    }
	}

	private void OnClickActive(){
		Debug.Log("Button is pressed");
		isActive = !isActive;
	}
}
