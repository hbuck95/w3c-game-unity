using System;
using System.Collections;
using UnityEngine;

public class Robot : MonoBehaviour {
    public static Robot Instance;
    public bool instructed; //determines if the robot has been giving an instruction through Move().
    private bool _eventThisCycle; //determines if the interaction event has been activated during the current facingPlayer cycle.
    public bool centerLight, facingPlayer, canInteract, stare;
    public bool Moving { get; private set; }
    public Transform defaultPosition, player;
    public Light spotLight;
    public Event robotEvent;

    private void Awake() {
        Instance = this;
        robotEvent = new Event('E', "to interact with the robot.", gameObject.GetHashCode());
        canInteract = true;
    }

	// Update is called once per frame
	private void FixedUpdate () {
        if(centerLight)
        spotLight.transform.LookAt(Camera.main.ViewportToWorldPoint(new Vector3(0.5f,0.5f,50f))); //Keep the robots spotlight pointing as close to the screen center as we can.

        //If the robot has been instructed to move/wait somewhere or is doing something we can return early to avoid the later calls disrupting this.
	    if (instructed) return;

        if (facingPlayer) {
            transform.rotation = Quaternion.Lerp(transform.rotation, new Quaternion { eulerAngles = Camera.main.transform.forward }, Time.deltaTime);
            transform.LookAt(player);

            if (robotEvent.Method == null)
                return; //If the robot hasn't got an interaction method then there is no need to interact. Although this could be replaced with some dialogue.

            if (!InteractionEvent.EventInProgress() && !_eventThisCycle) {
                InteractionEvent.CreateEvent(robotEvent);
                _eventThisCycle = true;
            }

        } else {
            transform.position = Vector3.Lerp(transform.position, defaultPosition.position, Time.deltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, player.rotation, 0.1f);
            _eventThisCycle = false;

            //If the active event is the robot event we cancel it because the player is no longer facing the robot.        
            if (InteractionEvent.GetEventHash() == gameObject.GetHashCode())
                InteractionEvent.CancelActiveEvent();

        }
    }

    /// <summary>
    /// Sets the action which takes place after interacting with the robot.
    /// </summary>
    /// <param name="a">The action you wish to assign to the robot. (e.g. OpenMap)</param>
    public void SetAction(Action a = null) {
        robotEvent.Method = a;
    }

    /// <summary>
    /// Makes the robot stare at the given target until 'stare' is set to false.
    /// </summary>
    /// <param name="target"></param>
    public void StareAt(Transform target) {
        stare = true;
        StartCoroutine(_StareAt(target));
    }

    /// <summary>
    /// Moves the robot to the specified location. Ignores walls and colliders and travels in a straight line, point-to-point. Will cancel an active stare.
    /// </summary>
    /// <param name="location">The Vector3 location where the robot should move to.</param>
    /// <param name="delay">The length of time the robot should wait at the location before returning to the player. (OPTIONAL: 0 as default)</param>
    /// <param name="speed">The speed at which the robot should move to its destination (OPTIONAL: 0.01f as default)</param>
    /// <param name="waitToBeCalled">Should the robot wait at its destination indefinitely until it is called via Call()? (OPTIONAL: false by default)</param>
    public void Move(Vector3 location, float delay = 0, float speed = 0.01f, bool waitToBeCalled = false) {
        stare = false;
        Moving = true;
        instructed = true;
        StartCoroutine(_Move(location, delay, speed, waitToBeCalled));
    }

    /// <summary>
    /// Moves the robot to the specified location. Ignores walls and colliders and travels in a straight line, point-to-point. Will cancel an active stare.
    /// </summary>
    /// <param name="location">The Vector3 location where the robot should move to.</param>
    /// <param name="waitToBeCalled">Should the robot wait at its destination indefinitely until it is called via Call()? (OPTIONAL: false by default)</param>
    public void Move(Vector3 location, bool waitToBeCalled = false)
    {
        stare = false;
        Moving = true;
        instructed = true;
        StartCoroutine(_Move(location, 0, 0.01f, waitToBeCalled));
    }

    /// <summary>
    /// Rotate the robot to the givern quaternion.
    /// </summary>
    /// <param name="rotation">The quaternion rotation which the robot should face.</param>
    public void Rotate(Quaternion rotation) {
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime);
    }

    /// <summary>
    /// Return the robot back to the player if it has been instructed to wait. Will cancel an active stare routine.
    /// </summary>
    public void Call() {
        StartCoroutine(ReturnToPlayer());
    }

    //See: Call()
    public IEnumerator ReturnToPlayer() {
        stare = false;
        var start = Time.time;
        while (Vector3.Distance(transform.position, player.position) > 2f) {
            Debug.Log("Going back");
            transform.position = Vector3.Lerp(transform.position, player.position, (Time.time - start) * 0.01f);
            transform.LookAt(player.position);
            yield return new WaitForEndOfFrame();
        }
        instructed = false;

    }

    //See: Move()
    private IEnumerator _Move(Vector3 location, float delay, float speed, bool waitToBeCalled) {
        var start = Time.time;
        while (Vector3.Distance(transform.position, location) >= 1f) {
            transform.position = Vector3.Lerp(transform.position, location, (Time.time - start) * speed);
            transform.LookAt(location);
            yield return new WaitForEndOfFrame();
        }

        Moving = false;
        Debug.Log("Finished moving.");
        if (waitToBeCalled) yield break;

        yield return new WaitForSecondsRealtime(delay);
        StartCoroutine(ReturnToPlayer());
    }

    //See: StareAt()
    private IEnumerator _StareAt(Transform target) {
        while (stare) {
            transform.LookAt(target);
            yield return null;
        }
    }

}
