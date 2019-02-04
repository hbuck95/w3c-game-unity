using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class predicDoorOpen : MonoBehaviour {

    public static bool _codeFound;

	// Use this for initialization
	void Start () {

        _codeFound = false;
		
	}
	
	// Update is called once per frame
	void Update () {

		RaycastHit _hit;

		var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
		if (!FPController._paused){
		if (Physics.Raycast(ray, out _hit, Mathf.Infinity) && predicPasswordDoor.Instance._playerNear)
            {
                Debug.Log(_hit.transform.name + "hit");
                if (_hit.transform.tag == "Door"){
                	InteractionEvent.CreateEvent(new Event('e', "to open the door.", _hit.transform.gameObject.GetHashCode(), predicPasswordDoor.Instance._openDoor));
                }

            }
            else
            {
				if (InteractionEvent.GetEventHash() != _hit.transform.gameObject.GetHashCode())
                		InteractionEvent.CancelActiveEvent();  
            }

        }

        if (Physics.Raycast(ray, out _hit, Mathf.Infinity) && !_codeFound && _hit.transform.tag == "Trigger")
        {
            RobotDialogue.Instance.QueueDialogue("Ah! There's the code for the door!");
            RobotDialogue.Instance.QueueDialogue("6032. Let's not forget it");
            RobotDialogue.Instance.StartDialogue();
            _codeFound = true;

        }
    }
}
