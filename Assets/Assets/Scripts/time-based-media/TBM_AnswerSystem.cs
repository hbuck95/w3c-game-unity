using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Linq;

public class TBM_AnswerSystem : MonoBehaviour {
    public CodeEditorInteraction codeEditorInteraction;
    public PlayVideo videoPlayer;
    public Robot robot;
    private string _playerDescription;
    private const string _videoTag = "<color='#{0}'><video <color='#{1}'>src</color>=\"important_video.mp4\"> <color='#{1}'>alt</color>=\"{2}\"</color>";
    public string[] videoDescription = new string[3];
    private bool _waitingToStartVideo, levelComplete;
    public GameObject airConditioningUnit;
    private FPController _con;
    private Ray _ray;
    private RaycastHit _hit;
    public List<GameObject> interactionObjects = new List<GameObject>();
    public SceneLoader SceneLoader;

    /// <summary>
    /// Prompts the user to enter a valid description for the mystery object identified through the JS function 'window.prompt' and returns a UTF-8 formatted string.
    /// </summary>
    /// <returns>UTF-8 String of input.</returns>
    [DllImport("__Internal")]
    private static extern string SubmitDescription();

    //testing this functionality to see if the loop performs better when executed in the browser using js or in-game using c#
    [DllImport("__Internal")]
    private static extern string SubmitDescriptionB();

    private void Awake() {
        _con = GetComponent<FPController>();
    }

    private void Start() {
        StartCoroutine(_InitialDialogue());
    }

	private void Update () {
        InteractionCheck(); //Method containing the interaction event functionality

        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            if (codeEditorInteraction.Hovering) {
                StartCoroutine(WriteDescription());             
                FormatTag(true);
            } else {
                FormatTag();
            }
        }

    }

    private IEnumerator _InitialDialogue() {
        Robot.Instance.enabled = false;
        RobotDialogue.Instance.ClearQueue(); //Clear any dialogue which might have been left over from the previous level as we're using a static instance.
        RobotDialogue.Instance.QueueDialogue("Welcome to lesson 2 where we'll look at time-based media.");
        RobotDialogue.Instance.QueueDialogue("In this room you'll watch a video, unfortunately the air conditioning in the room can get a little bit noisy so it might be hard to hear it.");
        RobotDialogue.Instance.QueueDialogue("You'll have to watch the video and then edit the videos alt tag with a good desciption, your description should be the phrase said in the video.");
        RobotDialogue.Instance.StartDialogue();
        _con.enabled = false;
        yield return new WaitUntil(() => RobotDialogue.Instance.IsUserReady());
        _con.enabled = true;
        videoPlayer.PlayNoise();
        Robot.Instance.enabled = true;
    }

    /// <summary>
    /// The dialogue which is used after the video has played. Includes instructing the robot to turn off the noise. Makes use of yield statements to pause thread execution.
    /// </summary>
    /// <returns></returns>
    private IEnumerator PostVideoDialogue() {
        yield return new WaitUntil(() => videoPlayer.videoFinished);
        yield return new WaitForSeconds(0.5f);
        RobotDialogue.Instance.ClearQueue(); //Clear any dialogue which might have been left over from the previous level as we're using a static instance.
        RobotDialogue.Instance.QueueDialogue("It's too noisy in here! Now that the video is over let me see if I can do something about it.");
        RobotDialogue.Instance.StartDialogue();
        robot.Move(airConditioningUnit.transform.position, 2f, 0.01f);
        yield return new WaitForSeconds(4f);
        videoPlayer.StopNoise();
        yield return new WaitForSeconds(1f);
        RobotDialogue.Instance.QueueDialogue("Hmmm, I think that should do it, it's getting quieter already.");
        RobotDialogue.Instance.StartDialogue();
        yield return new WaitWhile(() => videoPlayer.IsNoisy());
        RobotDialogue.Instance.QueueDialogue("Few! I'm glad thats stopped!");
        RobotDialogue.Instance.QueueDialogue("Okay, now that the video has finished it's time to get to work. Open your code editor when you're ready!");
        RobotDialogue.Instance.StartDialogue();
        yield return new WaitWhile(() => RobotDialogue.Instance.IsDialoguePlaying());
        InteractionEvent.CreateEvent(new Event('E', "to open the code editor"));
        yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.E));
        codeEditorInteraction.Show();
        FormatTag();
    }

    /// <summary>
    /// MouseOver()/PointerEnter() functionality used to interact with Unitys event system.
    /// </summary>
    public void Enter() {
        codeEditorInteraction.Enter();
        FormatTag(true);
    }

    /// <summary>
    /// MouseExit()/PointerExit() functionality used to interact with Unitys event system.
    /// </summary>
    public void Exit() {
        codeEditorInteraction.Exit();
        if (codeEditorInteraction.Selected) return;
        FormatTag();
    }
    
     private void FormatTag(bool invert = false) {
        var descToDisplay = string.IsNullOrEmpty(_playerDescription) ? string.Format("Video-{0}.mp4", videoPlayer.videoId) : _playerDescription;
        codeEditorInteraction.htmlTag.text = codeEditorInteraction.ColourAltTag(_videoTag, descToDisplay, invert);
    }

    public IEnumerator WriteDescription() {

        while (true) {
            var videoDesc = videoDescription[videoPlayer.videoId].ToLower();
            _playerDescription = SubmitDescriptionB().ToLower();
            FormatTag(true);

            //If the player entered descriptions matches the defined description keywords
            if (videoDesc.Contains(_playerDescription) || _playerDescription.Contains(videoDesc))
                break;

            RobotDialogue.Instance.ClearQueue();
            RobotDialogue.Instance.QueueDialogue("Hmm...I don't think that's quite accurate enough. Try again.");
            RobotDialogue.Instance.StartDialogue();
            yield return new WaitWhile(() => RobotDialogue.Instance.IsDialoguePlaying());

        }

        Destroy(codeEditorInteraction.transform.root.gameObject);
        RobotDialogue.Instance.ClearQueue();
        RobotDialogue.Instance.QueueDialogue("Hmm, yes, that seems like a good description for it.");
        RobotDialogue.Instance.QueueDialogue("The scenario that you have just completed is based on guideline 1.2 time-based Media of the W3C accessibility guidelines.");
        RobotDialogue.Instance.QueueDialogue("If you would like to read the guideline specifications please click <color='blue'>here</color> to view it.", true);
        RobotDialogue.Instance.StartDialogue();

        levelComplete = true;
        //Level is complete.
        //Put DB calls and achievements after this.
    }

    private void OnTriggerEnter(Collider c) {
        if (c.name == "Pedastal") {
            //Don't interact with it if the video has played is playing.
            if (videoPlayer.videoFinished || videoPlayer.IsPlaying())
                return;
            RobotDialogue.Instance.ClearQueue(); //Clear any dialogue which might have been left over from the previous level as we're using a static instance.
            RobotDialogue.Instance.QueueDialogue("Okie doke! When you're ready for the video to play just press that big red button there.");
            RobotDialogue.Instance.StartDialogue();
            _waitingToStartVideo = true;
            c.enabled = false;
        }
    }

    /// <summary>
    /// Handles all the functionality for Interaction Events. This should be run continuously either each frame within Update() or through an Invoke.
    /// </summary>
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

        Debug.Log(_hit.transform.name);

        switch (_hit.transform.name) {
            case "BigRedButton":
                if (_waitingToStartVideo)
                    InteractionEvent.CreateEvent(new Event('E', "to start the video", _hit.transform.gameObject.GetHashCode(), StartVideo));
                return;
            case "SceneDoor":
                if(levelComplete)
                    InteractionEvent.CreateEvent(new Event('E', "to go back to the hallway.", _hit.transform.gameObject.GetHashCode(), SceneLoader.LoadHallway));
                return;
        }

    }

    private void StartVideo() {
        _waitingToStartVideo = false;
        videoPlayer.LoadVideo();
        StartCoroutine(PostVideoDialogue());
    }

}
