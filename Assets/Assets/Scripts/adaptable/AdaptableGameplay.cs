using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class AdaptableGameplay : MonoBehaviour, IStateTracking {
    public Dictionary<string, bool> Status { get; private set; }
    public List<GameObject> interactionObjects = new List<GameObject>(); //Objects which will have an interaction event
    public GameObject computerObject, televisionObject, tvObjRobotPos;
    private FPController _con;
    private RaycastHit _hit;
    private Ray _ray;

    /// <summary>
    /// Initialise variables and set things up.
    /// </summary>
    private void Awake () {
        _con = GetComponent<FPController>();
        Status = new Dictionary<string, bool> {
            {"Introduction", false},
            {"RoomIntroduction", false },
            {"FoundPC", false },
            {"FoundTV", false },
            {"OpenedPlatform", false},
            {"PlatformIntroduction", false },
            {"MobileComplete", false },
            {"TelevisionComplete", false },
            {"ComputerComplete", false },
        };
    }

    private void Start() {
        if (!CheckState("Introduction"))
            StartCoroutine(_IntroductionDialogue());
    }
	
	/// <summary>
    /// Update loop called once per frame.
    /// </summary>
	private void Update () {
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

        //If the PC hasn't been found yet then return early as we don't want the player to interact
        //with any platforms until it has been found and introduced.
	    if (!CheckState("FoundPC"))
	        return;

	    //No default case used as the above if block will cover the checking for both a null transform if the raycast didn't hit anything
	    //and the possibility of the object hit not being an event object.
	    switch (_hit.transform.tag) {
	        case "MobilePlatform":
	            if (PlatformHandler.Instance.ActiveContent.contentPlatform == Platform.Mobile)
	                return;
                PlatformHandler.Instance.ActivateEvent(Platform.Mobile, _hit.transform.gameObject.GetHashCode());
                return;
            case "TVPlatform":
                if (PlatformHandler.Instance.ActiveContent.contentPlatform == Platform.Television || !CheckState("FoundTV"))
                    return;
                PlatformHandler.Instance.ActivateEvent(Platform.Television, _hit.transform.gameObject.GetHashCode());
                return;
	        case "PCPlatform":
	            if (PlatformHandler.Instance.ActiveContent.contentPlatform == Platform.Computer)
	                return;
	            PlatformHandler.Instance.ActivateEvent(Platform.Computer, _hit.transform.root.gameObject.GetHashCode());
	            return;
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Update the specified state with its specified value.
    /// </summary>
    /// <param name="state">The name of the state to update.</param>
    /// <param name="value">The true or false value to assign the state.</param>
    public void UpdateState(string state, bool value) {
        Debug.Log(string.Format("{0}: {1}", state, value));
        Status[state] = value;
    }

    /// <inheritdoc />
    /// <summary>
    /// Check the value of the specified state. 
    /// </summary>
    /// <param name="state"></param>
    /// <returns>bool - True if the state has been reached, false if not.</returns>
    public bool CheckState(string state) {
        return Status[state];
    }

    private IEnumerator _IntroductionDialogue() {
        UpdateState("Introduction", true);
        _con.enabled = false;
        RobotDialogue.Instance.ClearQueue();
        RobotDialogue.Instance.QueueDialogue("This office is the Department of Adapability.");
        RobotDialogue.Instance.QueueDialogue("They work on adapating content for use on a range of platforms rather than just the platform it was designed for.");
        RobotDialogue.Instance.QueueDialogue("According to my database, they're currently working on making a website design for use on computers suitable...");
        RobotDialogue.Instance.QueueDialogue("...for use on mobile and smart tv platforms, but they've been having some trouble.");
        RobotDialogue.Instance.QueueDialogue("Let's go and take a look.");
        RobotDialogue.Instance.StartDialogue();
        yield return new WaitWhile(() => RobotDialogue.Instance.IsDialoguePlaying());
        _con.enabled = true;
    }

    private IEnumerator _RoomIntroduction() {
        Doors.Instance.Close();
        Doors.Instance.SetState(DoorState.Locked);
        RobotDialogue.Instance.QueueDialogue("Look around for a computer that you can access so we can see what content is being worked on.");
        RobotDialogue.Instance.StartDialogue();
        UpdateState("RoomIntroduction", true);
        yield return null;
    }

    private void OnTriggerEnter(Collider c) {
        switch (c.name) {
            case "EnterRoom":
                StartCoroutine(_RoomIntroduction());
                Destroy(c.gameObject);
                return;
            case "PCPlatformObj":
                StartCoroutine(_FoundComputer());
                Destroy(c);
                return;
            case "TVPlatformObj":
                if (!CheckState("FoundPC"))
                    return;
                StartCoroutine(_FoundTelevision());
                Destroy(c);
                return;
            case "EndOfLevel":
                if (!CheckState("ComputerComplete") && !CheckState("MobileComplete") && !CheckState("TelevisionComplete"))
                    return;
                StartCoroutine(_EndOfLevel());
                Destroy(c.gameObject);
                return;
            default:
                Debug.LogError(string.Format("Unable to determine trigger outcome for the gameobject '{0}'.", c.name));
                return;
        }
    }

    private IEnumerator _FoundComputer() {
        RobotDialogue.Instance.QueueDialogue("A computer!");
        RobotDialogue.Instance.StartDialogue();
        Robot.Instance.Move(computerObject.transform.position, 0, 0.01f, true);
        yield return new WaitWhile(() => Robot.Instance.Moving);
        Robot.Instance.StareAt(transform);
        RobotDialogue.Instance.QueueDialogue("Access it so we can look at the progress that's been made.");
        RobotDialogue.Instance.StartDialogue();
        yield return new WaitWhile(() => RobotDialogue.Instance.IsDialoguePlaying());
        UpdateState("FoundPC", true);
    }

    private IEnumerator _FoundTelevision() {
        Robot.Instance.Move(tvObjRobotPos.transform.position, 0, 0.01f, true);
        yield return new WaitWhile(() => Robot.Instance.Moving);
        Robot.Instance.StareAt(televisionObject.transform);
        yield return new WaitForSeconds(2f);
        RobotDialogue.Instance.QueueDialogue("Hmm, that's definitely one of the platforms but I don't know how you would edit a website through a television display.");
        RobotDialogue.Instance.StartDialogue();
        yield return new WaitWhile(() => RobotDialogue.Instance.IsDialoguePlaying());
        yield return new WaitForSeconds(0.25f);
        Robot.Instance.StareAt(transform);
        RobotDialogue.Instance.QueueDialogue("...");
        RobotDialogue.Instance.QueueDialogue("What if you connected me to the screen?");
        RobotDialogue.Instance.QueueDialogue("I could display all the content and you'd be able to see what you're doing.");
        RobotDialogue.Instance.StartDialogue();
        yield return new WaitWhile(() => RobotDialogue.Instance.IsDialoguePlaying());
        InteractionEvent.CreateEvent(new Event('E', "to connect the screen to the Helper Robot."));
        yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.E));
        Robot.Instance.StareAt(televisionObject.transform);
        RobotDialogue.Instance.QueueDialogue("..");
        RobotDialogue.Instance.StartDialogue();
        yield return new WaitForSeconds(0.5f);
        RobotDialogue.Instance.ClearQueue();
        RobotDialogue.Instance.QueueDialogue("....");
        RobotDialogue.Instance.StartDialogue();
        yield return new WaitForSeconds(0.5f);
        RobotDialogue.Instance.ClearQueue();
        RobotDialogue.Instance.QueueDialogue("......");
        RobotDialogue.Instance.StartDialogue();
        yield return new WaitForSeconds(0.5f);
        RobotDialogue.Instance.ClearQueue();
        RobotDialogue.Instance.QueueDialogue("..");
        RobotDialogue.Instance.StartDialogue();
        yield return new WaitForSeconds(0.5f);
        RobotDialogue.Instance.ClearQueue();
        RobotDialogue.Instance.QueueDialogue("....");
        RobotDialogue.Instance.StartDialogue();
        yield return new WaitForSeconds(0.5f);
        RobotDialogue.Instance.ClearQueue();
        RobotDialogue.Instance.QueueDialogue("......");
        RobotDialogue.Instance.StartDialogue();
        yield return new WaitForSeconds(2f);
        Robot.Instance.StareAt(transform);
        Robot.Instance.Rotate(Quaternion.Euler(transform.position));
        RobotDialogue.Instance.ClearQueue();
        RobotDialogue.Instance.QueueDialogue("Ok, I'm connected up to it, you should be able to access it now.");
        RobotDialogue.Instance.StartDialogue();
        yield return new WaitWhile(() => RobotDialogue.Instance.IsDialoguePlaying());
        UpdateState("FoundTV", true);
    }

    private IEnumerator _EndOfLevel() {
        Robot.Instance.StareAt(transform);
        RobotDialogue.Instance.QueueDialogue("You've now completed the adaptability level! Congratulations!");
        RobotDialogue.Instance.QueueDialogue("If you want to read the W3C requirement for this in more detail then click here now!", true);
        RobotDialogue.Instance.StartDialogue();
        yield return new WaitWhile(() => RobotDialogue.Instance.IsDialoguePlaying());
    }
}
