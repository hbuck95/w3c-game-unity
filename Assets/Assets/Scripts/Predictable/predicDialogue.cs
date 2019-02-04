using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class predicDialogue : MonoBehaviour {

	// Use this for initialization
	void Start () {

		RobotDialogue.Instance.QueueDialogue("Welcome to guideline 3.2, Predictable!");
		RobotDialogue.Instance.QueueDialogue("In the room up ahead, there will be a password-protected door that will behave unpredictably");
		RobotDialogue.Instance.QueueDialogue("To fix this, we will need to fix said door and then open it");
		RobotDialogue.Instance.StartDialogue();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
