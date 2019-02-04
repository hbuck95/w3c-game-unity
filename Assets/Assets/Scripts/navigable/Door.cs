using System.Collections;
using UnityEngine;

public class Door : MonoBehaviour {
    [Tooltip("The root parent in the door hierarchy used for pivot placement.")]
    public GameObject door;
    public State DoorState { get; private set; }
    [Tooltip("The degrees by which to rotate the door. (e.g. -110f")]
    public float rotateBy = -110f;
    private Quaternion _closedRot, _openRot;
    //private bool _paused;

    public enum State {
        Open,
        Closed,
        Locked,
        Unlocked
    }

	private void Start () {
        // _closedRot = door.transform.rotation;
        _closedRot = door.transform.rotation;
	    _openRot = new Quaternion {eulerAngles = new Vector3(0, _closedRot.eulerAngles.y + rotateBy, 0)};
	    DoorState = State.Closed;
	}
	
    public void SetState(State state) {
        DoorState = state;
    }

    public void OpenDoor() {
        DoorState = State.Open;
        StartCoroutine(_Open());
    }

    public void CloseDoor() {
        StartCoroutine(_Close());
    }

    public void OnTriggerEnter(Collider c) {
        if (!c.gameObject.CompareTag("Player")) return;
        //Debug.Log(string.Format("Player has entered {0}.", door.name));

        //if(DoorState != State.Locked)
        //    OpenDoor();

        switch (DoorState) {
            case State.Locked:
                Debug.Log(string.Format("Cannot open {0} as it is locked.", door.name));
                return;
            case State.Open:
            case State.Closed:
            case State.Unlocked:
                OpenDoor();
                return;
        }
    }

    public void OnTriggerExit(Collider c) {
        if (!c.gameObject.CompareTag("Player")) return;

        //Debug.Log(string.Format("Player has exited {0}.", door.name));
        CloseDoor();

    }

    //public void OnCollisionEnter(Collision c) {
    //    if (c.gameObject.CompareTag("Player"))
    //    {
    //        _paused = true;
    //        Debug.Log("Paused");
    //    }
    //}

    //public  void OnCollisionExit(Collision c) {
    //    if (c.gameObject.CompareTag("Player"))
    //    {
    //        _paused = false;
    //        Debug.Log("Un-Paused");
    //    }
    //}


    private IEnumerator _Open() {
        StopCoroutine(_Close());
        var rot = door.transform.rotation;

        var startTime = Time.time;
        while(Quaternion.Angle(door.transform.rotation, _openRot) > 0) { 
            //Vector3.Distance(_closedRot.eulerAngles, _openRot.eulerAngles) > 0.5f) {
            //Debug.Log(string.Format("Rot: {0}", Quaternion.Angle(door.transform.rotation, _openRot)));
          //  yield return new WaitUntil(() => !_paused);
            door.transform.rotation = Quaternion.Lerp(rot, _openRot, (Time.time - startTime) * 0.5f);
            yield return new WaitForEndOfFrame();
        }

        door.transform.rotation = _openRot;

        Debug.Log("Door opened");
    }

    private IEnumerator _Close() {
        StopCoroutine(_Open());
        var rot = door.transform.rotation;

        var startTime = Time.time;
        while (Quaternion.Angle(door.transform.rotation, _closedRot) > 0){
            //Vector3.Distance(_openRot.eulerAngles, _closedRot.eulerAngles) > 0.5f) {
            //Debug.Log(string.Format("Rot: {0}", Quaternion.Angle(door.transform.rotation, _openRot)));
           // yield return new WaitUntil(() => !_paused);
            door.transform.rotation = Quaternion.Lerp(rot, _closedRot, (Time.time - startTime) * 0.5f);
            yield return new WaitForEndOfFrame();
        }

        door.transform.rotation = _closedRot;

        if (DoorState != State.Locked) DoorState = State.Closed;
        //Debug.Log("Door closed.");
    }
}