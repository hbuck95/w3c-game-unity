using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class towerDefDialogue : MonoBehaviour {

	// Use this for initialization
	void Start () {

        RobotDialogue.Instance.ClearQueue();
        RobotDialogue.Instance.QueueDialogue("Hello and welcome to Guideline 2.2 - 'Enough Time'");
        RobotDialogue.Instance.QueueDialogue("In this lesson, you'll be tasked with destroying 100 enemies in this tower defense game!");
        RobotDialogue.Instance.QueueDialogue("Although, there's a twist. You only have 15 seconds to build the turrets before the wave starts!");
        RobotDialogue.Instance.QueueDialogue("To start building, just click on the 'Start Game' button!");
        RobotDialogue.Instance.QueueDialogue("And then just click on the cube that you want to place the turret on and choose your turret type!");
        RobotDialogue.Instance.QueueDialogue("Blue = Regular Turret, Green = Bomb Turret, Red = Demolish Turret (Delete this line when the UI art is done");
        RobotDialogue.Instance.QueueDialogue("Then, after time is up, the wave should start automatically! Good luck!");
        RobotDialogue.Instance.StartDialogue();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
