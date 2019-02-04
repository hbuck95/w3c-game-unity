using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class roboDed : MonoBehaviour {

	private Transform downPos;
	public Doors firstDoors;
	public GameObject shield, endGameOBJ;
	public static int _shieldHealth;
	private int shieldHealthReturn;
	public static bool _robotDown;
	public Slider shieldHealthSlider;

	// Use this for initialization
	void Start () {

		_shieldHealth = 300;

		firstDoors.SetState(DoorState.Unlocked);
		shield.SetActive(false);
		shieldHealthReturn = _shieldHealth;
		_robotDown = false;

		RobotDialogue.Instance.QueueDialogue("Hello and welcome to guideline 2.3 (Seizures)");
		RobotDialogue.Instance.QueueDialogue("As you may know, it is extremely important to design content to be safe for people with epilepsy");
		RobotDialogue.Instance.QueueDialogue("In this level there are several flashing lights which I cannot go near");
		RobotDialogue.Instance.QueueDialogue("We will need to clear these out so that I can make it to the ending");
		RobotDialogue.Instance.StartDialogue();
		
	}
	
	// Update is called once per frame
	void Update () {

		shieldHealthSlider.value = _shieldHealth;

		if (_shieldHealth <= 0){
			roboDown();
		}
		
	}

	void OnTriggerEnter (Collider other) {
		if (!switchInteraction._robotShield){
			if (other.tag == "Flash"){
				roboDown();
			}
		}

		if (other.tag == "endGame")
			endGameOBJ.SetActive(true);

	}

	void OnTriggerStay (Collider other) {
		if (!_robotDown){
			if (switchInteraction._robotShield) {
				if (other.tag == "Flash"){
					_shieldHealth--;
				}
			}
		}
	}

	void roboDown () {
		_shieldHealth = shieldHealthReturn;
		firstDoors.SetState(DoorState.Locked);
        if (!switchInteraction._robotShield)
        {
            RobotDialogue.Instance.QueueDialogue("I can't go on any further");
            RobotDialogue.Instance.QueueDialogue("Try and find a switch, there should be one here that can turn off the flashing light");
            RobotDialogue.Instance.StartDialogue();
        }
        else
        {
            RobotDialogue.Instance.QueueDialogue("I can't go any further, my shield is down");
            RobotDialogue.Instance.QueueDialogue("Try and find a green switch in the maze");
            RobotDialogue.Instance.QueueDialogue("It should recharge my shield");
            RobotDialogue.Instance.StartDialogue();
        }
        shield.SetActive(false);
		print("roboDown");
		RaycastHit hit;
		_robotDown = true;
		if (Physics.Raycast(transform.position, Vector3.down, out hit))
            Robot.Instance.Move(hit.point, 0, 2, true);
    }
}
