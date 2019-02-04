using UnityEngine;

public class doorOpen : MonoBehaviour {

	public static bool _playerNear, _doorIsOpen;
	public Transform _doorOpen;
	
	private void Update () {
		if(_doorIsOpen)
		transform.rotation = Quaternion.Lerp(transform.rotation, _doorOpen.rotation, 0.2f);
		if (!_doorIsOpen){
		if (KeyboardOnOffFPController.Instance._doorLook && _playerNear){

			InteractionEvent.CreateEvent(new Event('e', "to open the door.", this.transform.gameObject.GetHashCode(), openDoor));

		}
		else{
			InteractionEvent.CancelActiveEvent();
		}
		}
	}

	void openDoor(){
		print("Door open");
		_doorIsOpen = true;
		keyboardDialogueSystem._doorOpen();
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
}
