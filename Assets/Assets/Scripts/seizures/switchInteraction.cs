using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class switchInteraction : MonoBehaviour {

	public GameObject fLight1, switchLeft, switchRight, lFLight2, rFLight2, shield;
	public Doors firstDoors, secondDoors;
	private bool _switch1Activated, _leftSwitch2Activated, _rightSwitch2Activated;
	public static bool _robotShield, _onePartPickedUp;
	public static int _bothPartsPickedUp;

	// Use this for initialization
	void Start () {

		_bothPartsPickedUp = 0;

		_robotShield = false;

		secondDoors.SetState(DoorState.Locked);
		
	}
	
	// Update is called once per frame
	void Update () {

        if (_bothPartsPickedUp == 1 && _onePartPickedUp)
        {
            RobotDialogue.Instance.QueueDialogue("You just picked up a piece of a shield!");
            RobotDialogue.Instance.QueueDialogue("If you can find the other piece, I should get a shield that can help me withstand the flashing lights for a little bit!");
            RobotDialogue.Instance.StartDialogue();
            _onePartPickedUp = false;
        }

		if (_bothPartsPickedUp == 2 && !_robotShield){
            RobotDialogue.Instance.QueueDialogue("Cool! I have a shield now!");
            RobotDialogue.Instance.QueueDialogue("I should be able to move through the lights for a little bit now before powering down");
            RobotDialogue.Instance.QueueDialogue("If that does happen, find a green button to revive me");
            RobotDialogue.Instance.StartDialogue();
			_robotShield = true;
		}

		if (_robotShield){
			secondDoors.SetState(DoorState.Unlocked);
		}

		if (_leftSwitch2Activated && _rightSwitch2Activated)
			secondDoors.SetState(DoorState.Unlocked);

		RaycastHit hit;
        var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
		Debug.DrawRay(transform.position, Vector3.forward, Color.green, 10);
        if (Physics.Raycast(ray, out hit, 2))
        {
            if (hit.transform.tag == "Switch"){
				if (!_switch1Activated)
				InteractionEvent.CreateEvent(new Event('e', "to press the switch.", hit.transform.gameObject.GetHashCode(), firstSwitch));
				print ("I can see the switch");
			}

			if (hit.transform.tag == "Health"){
				InteractionEvent.CreateEvent(new Event('e', "to revive the robot.", hit.transform.gameObject.GetHashCode(), reCharge));
			}
				/*else {
					if (InteractionEvent.Instance.GetEventHash() != hit.transform.gameObject.GetHashCode())
                		InteractionEvent.Instance.CancelActiveEvent();  
				}*/

				if (hit.transform.gameObject.name == "rightSwitch"){
					if (!_rightSwitch2Activated)
						InteractionEvent.CreateEvent(new Event('e', "to press the switch.", hit.transform.gameObject.GetHashCode(), rightSwitch));

				}

				if (hit.transform.gameObject.name == "leftSwitch"){
					if (!_leftSwitch2Activated)
						InteractionEvent.CreateEvent(new Event('e', "to press the switch.", hit.transform.gameObject.GetHashCode(), leftSwitch));

				}
				else { 
					if (InteractionEvent.GetEventHash() != hit.transform.gameObject.GetHashCode())
                		InteractionEvent.CancelActiveEvent();  
				}
        }
		
	}

	void reCharge() {
        _robotShield = true;
        roboDed._robotDown = false;
		Robot.Instance.Call();
        shield.SetActive(true);
	}

	void rightSwitch() {
		switchActivate(rFLight2, firstDoors);
		_rightSwitch2Activated = true;
	}

	void leftSwitch() {
		switchActivate(lFLight2, firstDoors);
		_leftSwitch2Activated = true;
	}
	void firstSwitch(){
		switchActivate(fLight1, firstDoors);
		_switch1Activated = true;
	}

	void switchActivate(GameObject flashingLights, Doors doors) {
        RobotDialogue.Instance.QueueDialogue("Thanks for switching off that light");
        RobotDialogue.Instance.StartDialogue();
		flashingLights.SetActive(false);
		doors.SetState(DoorState.Open);
		Robot.Instance.Call();
		roboDed._robotDown = false;
	}
}
