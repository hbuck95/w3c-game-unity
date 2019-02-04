using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TextAlternativeGameplay : MonoBehaviour {
    public List<GameObject> interactionObjects = new List<GameObject>();
    public GameObject mysteryBox;
    private Ray _ray;
    private RaycastHit _hit;
    public Transform enterBoxRoomPos;
    public Doors doors;

    private void Start() {
        _IntroductionDialogue();
    }
	
	private void Update () {
		InteractionCheck();
	}

    private void InteractionCheck() {
        _ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Physics.Raycast(_ray, out _hit, 5f);

        //If the raycast doesn't hit or the object it hits is not an event object then cancel the active event.
        if (_hit.transform == null || interactionObjects.Count(x => x.GetHashCode() == _hit.transform.root.gameObject.GetHashCode()) == 0) {
            //Hash value of 0 is used for anonymous events (e.g. tutorial prompts)
            //We dont want to cancel these events through this method as this is intended for objects which the player will find in the world.
            if (InteractionEvent.GetEventHash() != 0)
                InteractionEvent.CancelActiveEvent();
            return;
        }

        //Debug.Log(_hit.transform.tag);

        switch (_hit.transform.tag) {
            case "MysteryBox":
                if (MysteryBox.Instance.IsOpen())
                    return;
                InteractionEvent.CreateEvent(new Event('E', "to open the mystery box", _hit.transform.gameObject.GetHashCode(), MysteryBox.Instance.OpenBox));
                return;

        }
    }

    private void _IntroductionDialogue() {
        RobotDialogue.Instance.ClearQueue();
        RobotDialogue.Instance.QueueDialogue("Welcome to the first level: Text Alternatives and Alt Tags!");
        RobotDialogue.Instance.QueueDialogue("In this lesson, we'll be looking at the importance of having alternatives to text for people with sight issues");
        RobotDialogue.Instance.QueueDialogue("One of these methods is with Alt tags!");
        RobotDialogue.Instance.QueueDialogue("Alt tags are a string variable that can be assigned to an image on a website");
        RobotDialogue.Instance.QueueDialogue("These are used so that ther user's software assisstant will read the alt tag out for them whilst also not being visible for people that don't need assistance");
        RobotDialogue.Instance.QueueDialogue("Up ahead in the next room will be a box containing a mystery item");
        RobotDialogue.Instance.QueueDialogue("I will give you clues as to what the item is but you won't be able to clearly see it");
        RobotDialogue.Instance.QueueDialogue("Based on my description and what you can see, you will have to write the alt tag");
        RobotDialogue.Instance.QueueDialogue("Good luck!");
        RobotDialogue.Instance.StartDialogue();
    }

    private void OnTriggerEnter(Collider c) {
        switch (c.name) {
            case "EnterBoxRoom_Trigger":
                doors.Close();
                doors.SetState(DoorState.Locked);
                StartCoroutine(_ObscureItem());
                Destroy(c.gameObject);
                return;               
        }    
    }

    private IEnumerator _ObscureItem() {
        Robot.Instance.Move(enterBoxRoomPos.position, true);
        yield return new WaitWhile(() => Robot.Instance.Moving);
        Robot.Instance.StareAt(mysteryBox.transform);
    }

}
