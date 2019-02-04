using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NavigableGameplay : MonoBehaviour, IStateTracking {
    public Dictionary<string, bool> Status { get; private set; }
    public GameObject serverInterface, keypadObject;
    public Map map;
    private bool _onPath, _checkPathRunning;
    public Doors liftDoors;
    public Transform robotIntro, keypadIntro;
    public List<GameObject> interactionObjects = new List<GameObject>(); //Objects which will have an interaction event

    //Caching raycast items for micro-optimisation
    private RaycastHit _hit;
    private Ray _ray;

    private void Awake() {
        Status = new Dictionary<string, bool>{
            {"Found_Server", false},
            {"Downloaded_Key", false},
            {"Downloaded_Map", false},
            {"Maze_IntroA", false},
            {"Maze_IntroB", false},
            {"ReturnedToStart", false},
            {"Doors_Unlocked", false}
        };
    }

    private void Update() {
        //if (Input.GetKeyDown(KeyCode.Z)) {
        //    InteractionEvent.CreateEvent(new InteractionEvent.Event('a', "to talk to the world!", 0, () => print("Hello world!")));
        //    StartCoroutine(_OpenDoorDialogue());
        //}
       
        _ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Physics.Raycast(_ray, out _hit, 5f);

        if (_hit.transform == null || interactionObjects.Count(x => x.name == _hit.transform.name) == 0) {
            interactionObjects.ForEach(x => {
                if (x.GetHashCode() == InteractionEvent.GetEventHash()) {
                    InteractionEvent.CancelActiveEvent();
                }
            });
            return;
        }

        //No default case used as the above if block will cover the checking for both a null transform if the raycast didn't hit anything
        //and the possibility of the object hit not being an event object.
        switch (_hit.transform.name) {
            case "Server":
                if (CheckState("Downloaded_Key") && CheckState("Downloaded_Map") || CheckState("Found_Server")) return;
                InteractionEvent.CreateEvent(new Event('E', "to connect the Helper Robot to the server!", _hit.transform.gameObject.GetHashCode(), () => serverInterface.SetActive(true)));
                return;
            case "KeyPad":
                if (!CheckState("Doors_Unlocked") && CheckState("ReturnedToStart"))
                    InteractionEvent.CreateEvent(new Event('E', "to connect the Helper Robot to the keypad!", _hit.transform.gameObject.GetHashCode(), () => UpdateState("Doors_Unlocked", true)));
                return;
        }

    }

    public void UpdateState(string state, bool value) {
        Status[state] = value;
    }

    public bool CheckState(string state){
        //Debug.Log(string.Format("{0}: {1}", state, Status[state]));
        return Status[state];
    }

    private void OnTriggerEnter(Collider c) {

        switch (c.tag) {           
            case "Server":
                Debug.Log("The player found the server!");
                Destroy(c.gameObject);
                RobotDialogue.Instance.QueueDialogue("You found the server! Now connect me up to it so we can get the files we need!");
                RobotDialogue.Instance.StartDialogue();
                return;
            case "MapPathPane":

                if (CheckState("Downloaded_Key") && CheckState("Downloaded_Map") && !CheckState("ReturnedToStart")) {
                    _onPath = true;
                    map.HighlightCrumb(c.name);
                }


                if (c.name != "StartingRoom_Trigger") return;//Beyond this line the code within this switch is used for this object only.

                if (!CheckState("Maze_IntroA")) {
                    liftDoors.Close();
                    liftDoors.SetState(DoorState.Locked);
                    StartCoroutine(_StartingDialogue());
                    Status["Maze_IntroA"] = true;
                }

                if (CheckState("Downloaded_Key") && CheckState("Downloaded_Map")) {
                    if (!CheckState("ReturnedToStart"))
                        StartCoroutine(_OpenDoorDialogue());
                }
                return;
            case "Finish":
                if (!CheckState("Doors_Unlocked")) return;
                Destroy(c.gameObject);
                Robot.Instance.Call();
                liftDoors.SetState(DoorState.Locked);
                return;
            default:
                return;
        }

    }


    private void OnTriggerExit(Collider c) {
        if (c.CompareTag("MapPathPane") && CheckState("Downloaded_Key") && CheckState("Downloaded_Map")) {
            if (CheckState("ReturnedToStart")) return;
            _onPath = false;
            StartCoroutine(_CheckPath());
            return;
        }

        if (c.name == "StartingRoom_Trigger") {
            if (CheckState("Maze_IntroB")) return;
            RobotDialogue.Instance.QueueDialogue("Hmm, this entire floor seems to be a maze of rooms...");
            RobotDialogue.Instance.QueueDialogue("I hope we'll be able to find our way out, still, lets worry about that later!");
            RobotDialogue.Instance.StartDialogue();
            Status["Maze_IntroB"] = true;
        }
    }

    /// <summary>
    /// Check if the player is on the maps path. If they're not give them a set amount of time to get back on the path before prompting them.
    /// </summary>
    /// <returns></returns>
    private IEnumerator _CheckPath(int t = 3) {
        if (_checkPathRunning) yield break;
        _checkPathRunning = true;
        var ticks = t; //The number of ticks to wait before determining that the player is off of the correct path.
        while (!_onPath) {
            yield return new WaitForSeconds(1);
            ticks--;
            if (ticks == 0) {
                RobotDialogue.Instance.QueueDialogue(map.IsActive()
                    ? "You seem to be going off the path, don't forget to look at the map. We don't want to get lost."
                    : "I think you're going off of the path, don't forget to use my terminal to check the breadcrumbs on the map.");
                RobotDialogue.Instance.StartDialogue();
                map.ClearCrumb();
            }
        }
        _checkPathRunning = false;
    }

    private void OnTriggerStay(Collider c){
        if (c.CompareTag("MapPathPane"))
            _onPath = true;
    }

    /// <summary>
    /// Contains and handles the starting dialogue triggered when the player initially steps out of the lift into the starting room.
    /// </summary>
    /// <returns></returns>
    private IEnumerator _StartingDialogue() {
        Robot.Instance.Move(robotIntro.position, 0, 0.01f, true);
        yield return new WaitUntil(() => !Robot.Instance.Moving);
        Robot.Instance.StareAt(transform);
        RobotDialogue.Instance.ClearQueue();
        RobotDialogue.Instance.QueueDialogue("Hmm, I don't think we're going to be able to back. What should we do?");
        RobotDialogue.Instance.StartDialogue();
        yield return new WaitForSeconds(3f);
        RobotDialogue.Instance.ClearQueue();
        RobotDialogue.Instance.QueueDialogue("Wait wait! I know what to do!");
        RobotDialogue.Instance.QueueDialogue("This floor is where the servers are stored, find the server room and connect me up to one.");
        RobotDialogue.Instance.QueueDialogue("Hopefully there will be some kind of access code that I can use to open the lift.");
        RobotDialogue.Instance.QueueDialogue("Let's get going!");
        RobotDialogue.Instance.StartDialogue();
        yield return new WaitUntil(() => !RobotDialogue.Instance.IsDialoguePlaying());
        Robot.Instance.Call();
    }

    /// <summary>
    /// Handles the dialogue, robot movements, and interactions for the scene which takes place when the player returns to starting room after downloading the files.
    /// </summary>
    /// <returns></returns>
    private IEnumerator _OpenDoorDialogue() {
        UpdateState("ReturnedToStart", true);
        Robot.Instance.SetAction();
        map.GetComponent<Canvas>().enabled = false;
        Robot.Instance.Move(keypadIntro.position, 0, 0.075f, true);
        yield return new WaitUntil(() => !Robot.Instance.Moving);
        RobotDialogue.Instance.QueueDialogue("There seems to be a port on this key pad, it might just fit me.");
        RobotDialogue.Instance.StartDialogue();
        yield return new WaitUntil(() => RobotDialogue.Instance.IsUserReady());
        Robot.Instance.StareAt(transform);
        RobotDialogue.Instance.QueueDialogue("Hey, come over here and try connecting me to it.");
        RobotDialogue.Instance.StartDialogue();
        yield return new WaitUntil(() => CheckState("Doors_Unlocked"));
        Robot.Instance.Move(keypadObject.transform.position, 0f, 0.005f, true);
        Robot.Instance.transform.LookAt(keypadObject.transform);
        RobotDialogue.Instance.ClearQueue();
        yield return new WaitUntil(() => !Robot.Instance.Moving);
        RobotDialogue.Instance.QueueDialogue("I'm in! Give me a few seconds to get the doors open.");
        RobotDialogue.Instance.StartDialogue();
        yield return new WaitForSeconds(4f);
        RobotDialogue.Instance.ClearQueue();
        RobotDialogue.Instance.QueueDialogue("I've almost got it, just a few more seconds..");
        RobotDialogue.Instance.StartDialogue();
        yield return new WaitForSeconds(2f);
        RobotDialogue.Instance.ClearQueue();
        RobotDialogue.Instance.QueueDialogue("...");
        RobotDialogue.Instance.QueueDialogue("There! Got it!");
        RobotDialogue.Instance.StartDialogue();
        yield return new WaitWhile(() => RobotDialogue.Instance.IsDialoguePlaying());
        liftDoors.SetState(DoorState.Unlocked);
        liftDoors.Open();
        yield return new WaitForSeconds(1);
        Robot.Instance.StareAt(transform);
        RobotDialogue.Instance.QueueDialogue("Lucky for us that the server also had a map on it.");
        RobotDialogue.Instance.QueueDialogue("Without the breadcrumb trail to follow back it might've taken us a lot longer to get back to where we started.");
        RobotDialogue.Instance.QueueDialogue("If you would like to look at the definitions for navigable in greater detail then click <color='blue'>here.</color>", true);
        RobotDialogue.Instance.StartDialogue();
        liftDoors.SetState(DoorState.Locked);
    }

}
