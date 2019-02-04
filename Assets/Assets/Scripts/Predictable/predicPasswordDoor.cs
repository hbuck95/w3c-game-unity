using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class predicPasswordDoor : MonoBehaviour {

    public static predicPasswordDoor Instance;

	public bool _playerNear, _doorIsOpen, _codeIsSeen, _disabledTextDialogueRun, _submitTextDialogueRun, _onFocusTextDialogueRun;
	public static bool paused, _passwordScreenHasBeenOpened;
	public GameObject _passwordEntry, _endGame, _codeWindow;
	public Button backButton, enterButton, codeButton, closeCodeButton;
	public InputField passwordInput, disabledText, submitText, onFocusText;
	public string Password;
	public ColorBlock passwordColor;

	void Start(){
		backButton.onClick.AddListener(goBack);
		enterButton.onClick.AddListener(enterPassword);
		codeButton.onClick.AddListener(seeCode);
		closeCodeButton.onClick.AddListener(closeCode);

        Instance = this;

		_passwordScreenHasBeenOpened = false;
		_codeIsSeen = false;
		_disabledTextDialogueRun = false;
		_submitTextDialogueRun = false;
		_onFocusTextDialogueRun = false;
	}

    public  void _openDoor()
    {
		if (!_passwordScreenHasBeenOpened){
			RobotDialogue.Instance.QueueDialogue("Okay so the door has a password system, there must be a password somewhere in this room");
			RobotDialogue.Instance.StartDialogue();
		}
        print("Door open");
        _doorIsOpen = true;
        _passwordEntry.SetActive(true);
        FPController._paused = true;
        MouseBehaviour.Instance.cameraLock = true;
        Cursor.visible = true;
        _passwordScreenHasBeenOpened = true;
        if (predicDoorOpen._codeFound)
        {
            RobotDialogue.Instance.QueueDialogue("Okay, now that we have the code, you should try and enter it");
            RobotDialogue.Instance.QueueDialogue("Although... It does seem as though the input isn't working properly");
            RobotDialogue.Instance.QueueDialogue("Let's open up the code and try to figure out what's going on");
            RobotDialogue.Instance.StartDialogue();
        }
    }

	private void Update () {

		/*if (FPController.Instance._doorLook && _playerNear && Input.GetKeyDown(KeyCode.E)){
			print("Door open");
		    _doorIsOpen = true;
			_passwordEntry.SetActive(true);
			paused = true;
		}*/

		if (disabledText.text == "" && !_disabledTextDialogueRun){
			passwordInput.readOnly = false;
			RobotDialogue.Instance.QueueDialogue("Okay, the text box should be active now");
			RobotDialogue.Instance.QueueDialogue("The next problem I see is that the 'Enter Password' button has no input type");
			RobotDialogue.Instance.QueueDialogue("On the line below the last, go ahead and enter 'submit' in the blank space after 'type='");
			RobotDialogue.Instance.StartDialogue();
			_disabledTextDialogueRun = true;
		}

		if (submitText.text == "submit" && !_submitTextDialogueRun){
			enterButton.interactable = true;
			RobotDialogue.Instance.QueueDialogue("Okay, the 'Enter Password' button should work now");
			RobotDialogue.Instance.QueueDialogue("The next problem I see is the onfocus function");
			RobotDialogue.Instance.QueueDialogue("It's set to black, but we can't really see the text with a black background");
			RobotDialogue.Instance.QueueDialogue("Let's change it to grey");
			RobotDialogue.Instance.StartDialogue();
			_submitTextDialogueRun = true;
		}

		if (onFocusText.text == "\"grey\";" && !_onFocusTextDialogueRun){
			passwordColor.highlightedColor = Color.gray;
			RobotDialogue.Instance.QueueDialogue("Okay cool! Let's close this window and try to enter the code");
			RobotDialogue.Instance.StartDialogue();
			_onFocusTextDialogueRun = true;
		}

		passwordInput.colors = passwordColor;
	}

	private void OnTriggerEnter(Collider other){
		if (other.tag == "Player"){
			_playerNear = true;
		}
	}

	private void OnTriggerExit(Collider other){
		if (other.tag == "Player"){
			_playerNear = false;
		}
	}

	void goBack(){
		_passwordEntry.SetActive(false);
        MouseBehaviour.Instance.cameraLock = false;
        Cursor.visible = false;
        FPController._paused = false;
	}

	void enterPassword(){
		if (passwordInput.text == Password){
			_passwordEntry.SetActive(false);
			paused = false;
			gameObject.SetActive(false);
			_endGame.SetActive(true);
		}

	}

	void seeCode(){
		_codeWindow.SetActive(true);
		if (_codeIsSeen == false){
			RobotDialogue.Instance.QueueDialogue("Okay, I can see a few problems here");
			RobotDialogue.Instance.QueueDialogue("Let's start with the first");
			RobotDialogue.Instance.QueueDialogue("The text input is disabled!");
			RobotDialogue.Instance.QueueDialogue("On the line that starts with 'input type=text', get rid of the disabled type at the end of the line");
			RobotDialogue.Instance.StartDialogue();
		}
		_codeIsSeen = true;
	}

	void closeCode(){
		_codeWindow.SetActive(false);
	}
}
