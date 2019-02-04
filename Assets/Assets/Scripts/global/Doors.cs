using System;
using System.Collections;
using UnityEngine;

[Obsolete("Deprecated as of 28th March as the new lift system and scripts have been implemented.")]
public enum DoorState {
    Open,
    Closed,
    Locked,
    Unlocked
}

[Obsolete("Deprecated as of 28th March as the new lift system and scripts have been implemented.")]
public class Doors : MonoBehaviour {
    public static Doors Instance;

    public GameObject door1, door2;
  //  public BoxCollider doorCol;
    public Transform door1Targ, door1Pos, door1TargL;
    public Transform door2Targ, door2Pos, door2TargL;
    public bool doorOpen;
    public DoorState State { get; private set; }

    private void Awake() {
        Instance = this;
    }

    // Use this for initialization
    private void Start() {
        door1TargL = door1Pos;
        door2TargL = door2Pos;
        doorOpen = false;
    }

    public void Open(){
        if (State == DoorState.Locked) return;
        StartCoroutine(_Open());
    }

    public void Close(){
        StartCoroutine(_Close());
    }

    public void SetState(DoorState state){
      //  doorCol.enabled = state == DoorState.Locked;
        State = state;
        Debug.Log(string.Format("The door is now {0}.", state.ToString()));
    }

    

    private IEnumerator _Open() {
        while (Vector3.Distance(door1.transform.position, door1Targ.position) >= 0.1f) {     
            door1.transform.position = Vector3.Lerp(door1.transform.position, door1Targ.position, 0.5f);
            door2.transform.position = Vector3.Lerp(door2.transform.position, door2Targ.position, 0.5f);
            yield return new WaitForEndOfFrame();
        }

        door1.transform.position = door1Targ.position;
        door2.transform.position = door2Targ.position;

        if(State != DoorState.Locked) State = DoorState.Open;
        Debug.Log("The doors have been opened");
    }

    private IEnumerator _Close() {

        while (Vector3.Distance(door1.transform.position, door1Pos.position) >= 0.1f) {
            door1.transform.position = Vector3.Lerp(door1.transform.position, door1Pos.position, 0.5f);
            door2.transform.position = Vector3.Lerp(door2.transform.position, door2Pos.position, 0.5f);
            yield return new WaitForEndOfFrame();
        }

        door1.transform.position = door1Pos.position;
        door2.transform.position = door2Pos.position;

        if (State != DoorState.Locked) State = DoorState.Closed;
        Debug.Log("The doors are now closed.");

    }

    // Update is called once per frame
    private void Update() {
        //door1.transform.position = Vector3.Lerp(door1.transform.position, door1Targ.position, 0.5f);
        //door2.transform.position = Vector3.Lerp(door2.transform.position, door2Targ.position, 0.5f);

        ////If the player is in the radius of the doors, the target for the LERPs above will change to the target for opening.
        ////Else, the target will change to that of the position of a closed door.
        //if (doorOpen) {
        //    door1TargL = door1Targ;
        //    door2TargL = door2Targ;
        //} else {
        //    door1TargL = door1Pos;
        //    door2TargL = door2Pos;
        //}
    }

    //If the player enters the trigger collider, the boolean for triggering the door opening will become true.
    private void OnTriggerEnter(Collider other) {
        if (other.tag == "Player")
           Open();
    }

    //Same as above, only this is for the door closing.
    private void OnTriggerExit(Collider other) {
        if (other.tag == "Player")
           Close();
    }

}
