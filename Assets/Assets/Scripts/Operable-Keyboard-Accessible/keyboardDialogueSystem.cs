using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class keyboardDialogueSystem : MonoBehaviour {

	public KeyboardOnOffFPController _con;
	public Robot _robot;
	public GameObject _mouseViewobj, _doorTriggerobj, _lavaTriggerobj, _congratsobj, _keyboardDialogueobj, _mouseDialogueobj;
	public Transform _robotWarn;

	public bool _passwordSeen;

	// Use this for initialization
	void Start () {
		RobotDialogue.Instance.ClearQueue();
		RobotDialogue.Instance.QueueDialogue("Welcome to Operable Keyboard! In this lesson we'll be.... hang on... it looks like the keyboard is broken... you can't jump");
		RobotDialogue.Instance.QueueDialogue("Oh well, that's not really important... wait...");
		RobotDialogue.Instance.QueueDialogue("Oh dear... the mouse seems to be completely broken too");
		RobotDialogue.Instance.QueueDialogue("I think I see the mouse input up ahead. Go and pick it up.");
		RobotDialogue.Instance.QueueDialogue("Also, don't forget you can use Left Shift to run.");
		RobotDialogue.Instance.StartDialogue();
	}
	
	private void _pickupA() {
		RobotDialogue.Instance.QueueDialogue("You've picked up the A key! Congrats! Now we can move left. Looks like the S key is up head, go and pick it up.");
		RobotDialogue.Instance.StartDialogue();
	}

	private void _pickupS(){
		RobotDialogue.Instance.QueueDialogue("Okay... I have good news and bad news...");
		RobotDialogue.Instance.QueueDialogue("The good news is we can move backwards now!");
		RobotDialogue.Instance.QueueDialogue("The bad news is we're now at a dead end...");
		RobotDialogue.Instance.QueueDialogue("Let's see if we can backtrack and find some more input methods.");
		RobotDialogue.Instance.StartDialogue();
	}

	private void _mouseView(){
		RobotDialogue.Instance.QueueDialogue("Ah! There's the mouse input! Let's pick it up!");
		RobotDialogue.Instance.StartDialogue();
	}

	private void _pickupMouse(){
		RobotDialogue.Instance.QueueDialogue("Sweet! We can look around with the mouse now! Now we can properly explore the environment!");
		RobotDialogue.Instance.StartDialogue();
	}

	private void _lavaTrigger(){
		_robot.Move(_robotWarn.position, 2f, 2);
		RobotDialogue.Instance.QueueDialogue("Woah woah woah!");
		RobotDialogue.Instance.QueueDialogue("We haven't found the jump button yet! It's probably not a good idea to go here yet...");
		RobotDialogue.Instance.StartDialogue();
	}

	private void _doorTrigger(){
		RobotDialogue.Instance.QueueDialogue("Ah! There's a door over there!");
		RobotDialogue.Instance.QueueDialogue("If you go over there and press 'E', the door should open");
		RobotDialogue.Instance.QueueDialogue("The keyword there being should");
		RobotDialogue.Instance.QueueDialogue("I'm not even sure if the E key still works but give it a try anyway");
		RobotDialogue.Instance.StartDialogue();
	}

	public static void _doorOpen(){
		RobotDialogue.Instance.QueueDialogue("Oh! I guess the E key does still work then");
		RobotDialogue.Instance.QueueDialogue("Go ahead and pick up the space bar");
		RobotDialogue.Instance.StartDialogue();
	}

	private void _pickupSpace(){
		RobotDialogue.Instance.QueueDialogue("Ok cool! We can now jump! You can now traverse that lava!");
		RobotDialogue.Instance.StartDialogue();
	}

	public static void _deadDialogue(){
		RobotDialogue.Instance.ClearQueue();
		if (KeyboardOnOffFPController.Deaths < 10 && KeyboardOnOffFPController.space == true){
			RobotDialogue.Instance.QueueDialogue("Woah! Be careful! I can only respawn you so many times!");
		}
		if (KeyboardOnOffFPController.space == false){
			RobotDialogue.Instance.QueueDialogue("I told you not to go there yet!");
			RobotDialogue.Instance.QueueDialogue("Anyway whatever... There's a door behind you... Go ahead and open it with 'E'");
		}
		if (KeyboardOnOffFPController.Deaths >= 10 && KeyboardOnOffFPController.space == true){
			RobotDialogue.Instance.QueueDialogue("Okay... Remember what I said about only being able to respawn you so many times?");
			RobotDialogue.Instance.QueueDialogue("Well... I lied... just complete the level already... Please?");
		}
		RobotDialogue.Instance.StartDialogue();
	}

	private void _congrats(){
		RobotDialogue.Instance.QueueDialogue("Nice jumping!");
		RobotDialogue.Instance.QueueDialogue("Ok, now let's try and open that door up ahead with the E key");
		RobotDialogue.Instance.StartDialogue();
	}

	public static IEnumerator _password(){
			RobotDialogue.Instance.QueueDialogue("Hmmm.... This door has a password system");
			RobotDialogue.Instance.QueueDialogue("Hang on a second... I think I can figure out what the password is. Give me a second.");
			RobotDialogue.Instance.StartDialogue();
			yield return new WaitUntil(() => RobotDialogue.Instance.IsUserReady());
			yield return new WaitForSeconds(2f);
			RobotDialogue.Instance.QueueDialogue("Okay, the password is 'Accessibility' give that a try");
			RobotDialogue.Instance.StartDialogue();
			yield return new WaitForSeconds(2f);
			RobotDialogue.Instance.ClearQueue();
			RobotDialogue.Instance.QueueDialogue("Oh right! They keyboard is still broken! We should look around for the missing keys.");
			RobotDialogue.Instance.QueueDialogue("It seems as though A, E, and S are already working. We just need to find the rest of the keys");
			RobotDialogue.Instance.StartDialogue();
	}

	public static void _password2(){
		RobotDialogue.Instance.QueueDialogue("Remember, the password is 'Accessibility'");
		RobotDialogue.Instance.StartDialogue();
	}

	private IEnumerator _keyboardDialogue() {
		RobotDialogue.Instance.QueueDialogue("Oh no.. it seems as though this area is blocking out the use of the mouse completely!");
		RobotDialogue.Instance.QueueDialogue("Hang on, I think I can alter the keyboard to help you traverse this area");
		RobotDialogue.Instance.StartDialogue();
		yield return new WaitUntil(() => RobotDialogue.Instance.IsUserReady());
		yield return new WaitForSeconds(2f);
		RobotDialogue.Instance.QueueDialogue("Okay... try pressing 'Q' and 'E'");
		RobotDialogue.Instance.QueueDialogue("That should rotate you right and left. Use that to traverse this area");
		RobotDialogue.Instance.StartDialogue();
	}

	private IEnumerator _mouseDialogue() {
		RobotDialogue.Instance.QueueDialogue("Uh oh... looks like the keyboard input has been completely disabled");
		RobotDialogue.Instance.QueueDialogue("Let me see if I can do anything about this");
		RobotDialogue.Instance.StartDialogue();
		yield return new WaitUntil(() => RobotDialogue.Instance.IsUserReady());
		yield return new WaitForSeconds(2f);
		RobotDialogue.Instance.QueueDialogue("Okay... you can now move forward by left clicking");
		RobotDialogue.Instance.QueueDialogue("Just face whatever direction you want to move in and CLICK");
		RobotDialogue.Instance.QueueDialogue("Also if you can to jump, just right click");
		RobotDialogue.Instance.QueueDialogue("There's no lava in this section, I promise");
		RobotDialogue.Instance.QueueDialogue("Just kidding! There totally is!");
		RobotDialogue.Instance.StartDialogue();
	}

	private void OnTriggerEnter(Collider other){

        if (other.tag == "a"){
            _pickupA();
        }

		if (other.tag == "s"){
			_pickupS();
		}

		if (other.tag == "mouseView"){
			_mouseView();
			Destroy(_mouseViewobj);
		}

		if (other.tag == "mouse"){
			_pickupMouse();
		}

		if (other.tag == "lavaTrigger" && KeyboardOnOffFPController.space == false){
			_lavaTrigger();
			Destroy(_lavaTriggerobj);
		}

		if (other.tag == "doorTrigger" && KeyboardOnOffFPController.mouse == true && KeyboardOnOffFPController.space == false){
			_doorTrigger();
			Destroy(_doorTriggerobj);
		}

		if (other.tag == "space"){
			_pickupSpace();
		}

		if (other.tag == "congrats"){
			_congrats();
			Destroy(_congratsobj);
		}

		if (other.tag == "keyboardDialogue"){
			StartCoroutine(_keyboardDialogue());
			Destroy(_keyboardDialogueobj);
		}

		if (other.tag == "mouseDialogue"){
			StartCoroutine(_mouseDialogue());
			Destroy(_mouseDialogueobj);
		}
	}
}
