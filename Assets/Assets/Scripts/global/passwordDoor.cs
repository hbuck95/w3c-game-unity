using UnityEngine;
using UnityEngine.UI;

public class passwordDoor : MonoBehaviour {

	private bool _playerNear, _doorIsOpen;
	public static bool paused, _passwordScreenHasBeenOpened;
	public Transform _doorOpen;
	public GameObject _passwordEntry, _endGame;
	public Button backButton, enterButton;
	public InputField passwordInput;
	public string Password;

	void Start(){
		backButton.onClick.AddListener(goBack);
		enterButton.onClick.AddListener(enterPassword);
	}

	private void Update () {
		if(_doorIsOpen)
		transform.rotation = Quaternion.Lerp(transform.rotation, _doorOpen.rotation, 0.2f);
		if (doorOpen._doorIsOpen){
		if (KeyboardOnOffFPController.Instance._doorLook && _playerNear && !paused){
			InteractionEvent.CreateEvent(new Event('e', "to open the door.", this.transform.gameObject.GetHashCode(), openDoor));

		}
		else{
			InteractionEvent.CancelActiveEvent();
		}
		}
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

	void openDoor(){
		print("Door open");
		Cursor.visible = true;
		_passwordEntry.SetActive(true);
		paused = true;
		if (_passwordScreenHasBeenOpened == false){
			StartCoroutine(keyboardDialogueSystem._password());
			_passwordScreenHasBeenOpened = true;
		}
		else
		{
			keyboardDialogueSystem._password2();
		}
	}

	void goBack(){
		_passwordEntry.SetActive(false);
		Cursor.visible = false;
		paused = false;
	}

	void enterPassword(){
		if (passwordInput.text == Password){
			_doorIsOpen = true;
			_passwordEntry.SetActive(false);
			paused = false;
			_endGame.SetActive(true);
		}

	}
}
