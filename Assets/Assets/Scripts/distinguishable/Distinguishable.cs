using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Distinguishable : MonoBehaviour, IStateTracking {
    public Dictionary<string, bool> Status { get; private set; }
    private FPController _con;
    private Npc _target;
    public Robot robot;
    public GameObject endPoint, key;
    public Doors doors;
    private RaycastHit _hit;
    private Ray _ray;

    /// <summary>
    /// Assign variables.
    /// </summary>
    private void Awake() {
        Status = new Dictionary<string, bool> {
            {"RoboVisionActive", false },
            {"TargetFound", false },
            {"HasKey", false },
            {"UnlockedDoor", false }
        };

        _con = GetComponent<FPController>();
        Npc.destinations = GameObject.FindGameObjectsWithTag("NPCDestination").Select(x => x.transform).ToList();
    }

    /// <summary>
    /// Setup the game.
    /// Queue inital dialogue sequence.
    /// </summary>
	private void Start () {
        SelectTarget();
        StartCoroutine(_InitialDialogue());
	}
	
	private void Update () {
        RoboVisionRaycast();
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            _ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(_ray, out _hit, Mathf.Infinity)) {
                //Debug.Log(hit.transform.name);
                if (_hit.transform.CompareTag("NPC") && CheckState("TargetFound")) {//Only let players assassinate when they have located their target through the helper robots robovision.
                    var npc = _hit.transform.GetComponent<Npc>();
                    if (!_target.Dead) {
                        Assassinate(npc);
                        return; //dont jump down the chain and get the key in the same instance.
                    }
                    if(npc == _target) {
                        if (CheckState("HasKey")) {
                            RobotDialogue.Instance.QueueDialogue("You've already got the key, let's go!");
                            RobotDialogue.Instance.StartDialogue();
                            return;
                        }
                        UpdateState("HasKey", true);
                        RobotDialogue.Instance.QueueDialogue("Okay, now get to the exit so we can get out of here!");
                        RobotDialogue.Instance.StartDialogue();
                    }
                }


                if (_hit.transform.CompareTag("Door")) {
                    if (!CheckState("HasKey") && _target.Dead) {
                        RobotDialogue.Instance.QueueDialogue("You forgot the key! Go back to your target and search them, they should have it.");
                    } else if(!CheckState("HasKey") && !_target.Dead) { 
                        RobotDialogue.Instance.QueueDialogue("You need to kill your target first, they have the key that we need to leave.");
                    } else if (CheckState("HasKey") && !CheckState("UnlockedDoor")) {
                        RobotDialogue.Instance.QueueDialogue("That's it, you've unlocked the door. Let's leave!");
                        UpdateState("UnlockedDoor", true);
                        key.SetActive(true);
                        doors.SetState(DoorState.Unlocked);
                        doors.Open();
                    } else if (CheckState("HasKey") && CheckState("UnlockedDoor")) {
                        RobotDialogue.Instance.QueueDialogue("You've already unlocked the door!");
                    }
                    RobotDialogue.Instance.StartDialogue();
                }

            }
        }	
	}

    /// <summary>
    /// The initial opening dialogue for the level, also handles the disabling and re-enabling of the player controller.
    /// </summary>
    /// <returns></returns>
    private IEnumerator _InitialDialogue() {
        _con.enabled = false;
        RobotDialogue.Instance.ClearQueue(); //Clear any dialogue which might have been left over from the previous level as we're using a static instance.
        RobotDialogue.Instance.QueueDialogue("This level focuses on demonstrating distinguishability, this guideline is all about making content easily separable from the background.");
        RobotDialogue.Instance.QueueDialogue("When you go through that door you will be assigned a target to assassinate. Just locate your target and kill them, it sounds pretty simple right?");
        RobotDialogue.Instance.StartDialogue();
        yield return new WaitForSecondsRealtime(1f);
        yield return  new WaitUntil(() => RobotDialogue.Instance.IsUserReady());
        RobotDialogue.Instance.QueueDialogue("WRONG!");
        RobotDialogue.Instance.QueueDialogue("It won’t be as straight forward as you think, after all, this is about distinguishability. Lucky you have me here to help you!");
        RobotDialogue.Instance.StartDialogue();
        yield return new WaitUntil(() => RobotDialogue.Instance.IsUserReady());
        _con.enabled = true;
    }

    /// <summary>
    /// Handles the dialogue sequence for when the helper robot helps the player to distinguish between the colours.
    /// Swaps the material on the target npc gameobject.
    /// </summary>
    /// <returns>null</returns>
    private IEnumerator _DistinguishColours() {
        RobotDialogue.Instance.QueueDialogue("Okay, your target is wearing a dark blue outfit. Make sure you don't kill anybody but your target!");
        RobotDialogue.Instance.StartDialogue();
        yield return new WaitForSeconds(10f); //Wait 10 seconds after initially entering the level before starting the dialogue to allow the player to explore.
        yield return new WaitUntil(() => RobotDialogue.Instance.IsUserReady());
        RobotDialogue.Instance.QueueDialogue("Hmm, I think this might be an impossible task. How are you supposed to able to distinguish between these people?");
        RobotDialogue.Instance.QueueDialogue("You can't get this wrong, after all you don't want to kill the wrong target!");
        RobotDialogue.Instance.StartDialogue();
        yield return  new WaitUntil(() => RobotDialogue.Instance.IsUserReady()); //Check if the user has finished their previous dialogue before starting a new instance. This is to allow the time to pass before WaitForSeconds(t) runs.
        yield return new WaitForSeconds(2f);
        RobotDialogue.Instance.QueueDialogue("Wait! I know what I can do! I can use my robo-vision to increase your perception of colour.");
        //RobotDialogue.Instance.StartDialogue();
        //GameObject.FindGameObjectsWithTag("NPC").Select(npc => npc.GetComponent<Npc>()).ToList().ForEach(x => x.Distinguish());
        RobotDialogue.Instance.QueueDialogue("Point my light over each person, it will help you to distinguish between their colours.");
        //RobotDialogue.Instance.QueueDialogue("Your target is wearing a dark blue outfit, so now all you need to do it find the person with the darkest outfit.");
        RobotDialogue.Instance.QueueDialogue("But be careful, don't get ahead of yourself. With the your increased perception some targets may get darker in colour, wait for me to verify them first.");
        RobotDialogue.Instance.StartDialogue();
        RobotDialogue.Instance.QueueDialogue("....");
        yield return new WaitUntil(() => RobotDialogue.Instance.IsUserReady());
        yield return new WaitForSeconds(2f);
        RobotDialogue.Instance.QueueDialogue("There, that should do the trick.");
        RobotDialogue.Instance.StartDialogue();
        yield return new WaitUntil(() => RobotDialogue.Instance.IsUserReady());
        UpdateState("RoboVisionActive", true);
        robot.centerLight = true;
        robot.spotLight.enabled = true;
    }

    private void EndOfLevel() {
        RobotDialogue.Instance.ClearQueue();
        RobotDialogue.Instance.QueueDialogue("Well done, you managed to get the job done - but I bet you couldn't have done it without me!");
        RobotDialogue.Instance.QueueDialogue("Congradulations, The room you have just completed, has been based around the guideline Distinguishable at level A.");
        RobotDialogue.Instance.QueueDialogue("If you would like to read this guideline specification then please click <color='blue'>here</color> to view it on W3C website.", true);
        RobotDialogue.Instance.QueueDialogue("Congradulations, you have a new Achievement! You have compeleted Perceivable guidelines at level A.");
        RobotDialogue.Instance.StartDialogue();
    }

    /// <summary>
    /// The robots spotlight direction is based upon where the main camera direction is, the camera has been tied to the players viewpoint  therefore the robots 
    /// spotlight is also tied to the players viewpoint.
    /// Use the physics systems to create a raycast and simulate it coming from the robots spotlight. Use this raycast to "distinguish" (reveal) the NPCs.
    /// </summary>
    public void RoboVisionRaycast() {
        if (!CheckState("RoboVisionActive")) return;
        RaycastHit hit;
        var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out hit, 18f)){
            if(!hit.transform.CompareTag("NPC")) return;
            var n = hit.transform.GetComponent<Npc>();
            if (n.Revealed) return;
            n.Distinguish();
            RobotDialogue.Instance.ClearQueue();

            if(n.Role == CrowdRole.CrowdMember) {
               // RobotDialogue.Instance.ClearQueue(); //So we dont run into instances where multiple "innocent crowd members" are queued up to play.
                RobotDialogue.Instance.QueueDialogue("That's just an innocent robot.");
                RobotDialogue.Instance.StartDialogue();
            } else {
                UpdateState("RoboVisionActive", false);
                robot.spotLight.enabled = false;
                RobotDialogue.Instance.ClearQueue();
                RobotDialogue.Instance.QueueDialogue("That's your target! Kill them quickly and lets get out of here!");
                RobotDialogue.Instance.StartDialogue();
                _target.Indicate();
                UpdateState("TargetFound", true);
            }
        }   
    }

    /// <summary>
    /// Select a random target from all the instantiated games tagged with 'Npc' which an Npc script component attached.
    /// </summary>
    private void SelectTarget() {
        var targets = GameObject.FindGameObjectsWithTag("NPC").Select(npc => npc.GetComponent<Npc>()).ToList();
        _target = targets[new System.Random().Next(0, targets.Count)];
        _target.SetRole(CrowdRole.Target);
        Debug.Log(string.Format("{0} is now the target!", _target.gameObject.name));
    }

    /// <summary>
    /// Handles target assassination and is called from the update loop.
    /// </summary>
    /// <param name="npc"></param>
    private void Assassinate(Npc npc){
        if(npc.Role == CrowdRole.Target){
            npc.Die();
            endPoint.SetActive(true);
            RobotDialogue.Instance.QueueDialogue("You did it! You killed your target, now search it for the key.");
            RobotDialogue.Instance.StartDialogue();
            return;
        }

        RobotDialogue.Instance.QueueDialogue("Oh no! That wasn't your target at all, they were just an innocent robot!");
        RobotDialogue.Instance.StartDialogue();
    }

    private void OnTriggerEnter(Collider c) {
        if(c.name == "EnterRoom") {
            doors.SetState(DoorState.Locked);
            doors.Close();
            StartCoroutine(_DistinguishColours());
            Destroy(c.gameObject);
        }
    }
    
    private void OnTriggerStay(Collider c) {
        if(c.name == "EndPoint") {
            EndOfLevel();
            Destroy(c.gameObject);
        }
    }

    public void UpdateState(string state, bool value) {
        Status[state] = value;
    }

    public bool CheckState(string state) {
        return Status[state];
    }
}
