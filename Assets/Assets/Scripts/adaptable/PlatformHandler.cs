using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 649

public enum Platform {
    None,
    Computer,
    Mobile,
    Television
}

public class PlatformHandler : MonoBehaviour {
    public static PlatformHandler Instance { get; private set; }
    public PlatformContent ActiveContent { get; private set; }
    private AdaptableGameplay _gameplay;
    private FPController _con;
    private readonly PlatformContent _default = new PlatformContent { contentPlatform = Platform.None };
    private Event _event;
    [SerializeField] private PlatformContent _pcPlatform, _mobilePlatform, _tvPlatform;


    // Use this for initialization
    private void Awake() {
        Instance = this;
        _gameplay = GetComponent<AdaptableGameplay>();
        _con = GameObject.FindGameObjectWithTag("Player").GetComponent<FPController>();
        ActiveContent = _default; //Set the active content to the default (none).
        _event = new Event('E', "To access this platform.");//Keep this event stored in memory and just update the hash and action.

        {
            //unsafe {
            //    _event = new Event {
            //        Key = 'E',
            //        Message = "To access the device.",
            //        //Hash = _hit.transform.gameObject.GetHashCode(),
            //        Method = delegate {
            //            fixed (Platform* p = &ActiveContent.contentPlatform) {
            //                ActivatePlatform(*p);
            //            }
            //        }
            //    };
            //}
        }
    }

    // Update is called once per frame
    private void Update() {

        if (ActiveContent.contentPlatform == Platform.None)
            return;

        if (!_gameplay.CheckState("PlatformIntroduction"))
            return;

        if (Input.GetKeyDown(KeyCode.Backspace))
            CloseActivePlatform();

        if (Input.GetKeyDown(KeyCode.R))
            ActiveContent.Toolbox.ResetUI();

        if(Input.GetKeyDown(KeyCode.T) && !ActiveContent.Toolbox.IsEmpty())
            ActiveContent.Toolbox.Toggle(!ActiveContent.Toolbox.gameObject.activeSelf);
    }


    public void ActivateEvent(Platform p, int hash) {
        _event.Hash = hash;
        _event.Method = () => ActivatePlatform(p);
        InteractionEvent.CreateEvent(_event);
    }

    public void CloseActivePlatform() {
        if (ActiveContent == _default || ActiveContent.contentPlatform == Platform.None) {
            Debug.Log("The active platform is either set to default or has not had a platform set.");
            return;
        }

        ActiveContent.Toolbox.Toggle(false);
        ActiveContent.gameObject.SetActive(false);
        _con.enabled = true;
        ActiveContent = _default;
        InteractionEvent.CancelActiveEvent();
        RobotDialogue.Instance.ClearQueue();
        Robot.Instance.Call();
    }

    public void ActivatePlatform(Platform platform) {
 
        switch (platform) {
            case Platform.Computer:
                ActiveContent = _pcPlatform;
                break;
            case Platform.Mobile:
                ActiveContent = _mobilePlatform;
                break;
            case Platform.Television:
                ActiveContent = _tvPlatform;
                break;
            case Platform.None:
                ActiveContent = _default;
                return;
        }

        if (!_gameplay.CheckState("PlatformIntroduction") && platform == Platform.Computer) {
            StartCoroutine(_PlatformIntroduction());
        }

        ActiveContent.gameObject.SetActive(true);
        ActiveContent.Toolbox.Populate(); //Generates the list of the UI elements in the toolbox if it is empty.
        _con.enabled = false;

    }

    private IEnumerator _PlatformIntroduction() {
        RobotDialogue.Instance.QueueDialogue("Oh, it's a website!");
        RobotDialogue.Instance.QueueDialogue("Hmm, but it doesn't look like they quite had time to finish adapating it..");
        RobotDialogue.Instance.QueueDialogue("Let's see if we can finish it! Don't worry, I'll walk you through it.");
        RobotDialogue.Instance.StartDialogue();
        yield return new WaitWhile(() => RobotDialogue.Instance.IsDialoguePlaying());
        yield return new WaitForSeconds(0.5f);
        RobotDialogue.Instance.QueueDialogue("It looks like it's almost done, it's just missing that video there.");
        RobotDialogue.Instance.QueueDialogue("Let's see what's in the platforms toolbox!");
        RobotDialogue.Instance.StartDialogue();
        yield return new WaitWhile(() => RobotDialogue.Instance.IsDialoguePlaying());
        RobotDialogue.Instance.QueueDialogue("To start, open your toolbox by pressing 'T'.");
        RobotDialogue.Instance.StartDialogue();
        yield return new WaitWhile(() => RobotDialogue.Instance.IsDialoguePlaying());
        yield return new WaitForSeconds(0.25f);
        InteractionEvent.CreateEvent(new Event('T', "To access the Toolbox", 0, () => ActiveContent.Toolbox.Toggle(true)));
        yield return new WaitUntil(() => ActiveContent.Toolbox.gameObject.activeInHierarchy);
        yield return new WaitForSeconds(0.25f);
        RobotDialogue.Instance.QueueDialogue("This is where the content elements are stored that you'll need to place somewhere on the web page.");
        RobotDialogue.Instance.QueueDialogue("To use what's available just drag it across onto the platform, try that now.");
        RobotDialogue.Instance.StartDialogue();
        yield return new WaitUntil(() => ActiveContent.Toolbox.transform.childCount == 0);
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
        yield return new WaitForSeconds(0.25f);
        RobotDialogue.Instance.QueueDialogue("Now all you have to do is drag it onto the space on the page where it belongs.");
        RobotDialogue.Instance.QueueDialogue("You might have to close your toolbox first.");
        RobotDialogue.Instance.StartDialogue();
        yield return new WaitWhile(() => RobotDialogue.Instance.IsDialoguePlaying());
        InteractionEvent.CreateEvent(new Event('T', "To close the Toolbox", 0, () => ActiveContent.Toolbox.Toggle(false)));
        yield return new WaitWhile(() => ActiveContent.Toolbox.gameObject.activeInHierarchy);
        yield return new WaitForSeconds(0.25f);
        yield return new WaitUntil(() => _gameplay.CheckState("ComputerComplete"));
        yield return new WaitForSeconds(0.25f);
        RobotDialogue.Instance.QueueDialogue("This platform has now been fully adapated, but there are still 2 different platforms somewhere in this room which need to be finished.");
        RobotDialogue.Instance.QueueDialogue("Find them and can complete them too, once they're all finished we can continue.");
        RobotDialogue.Instance.QueueDialogue("You can use this platform as an reference when adding the other elements if you need to.");
        RobotDialogue.Instance.QueueDialogue("You can press backspace to leave a platform at any time, do that now so you can start looking for the other platforms.");
        RobotDialogue.Instance.StartDialogue();
        yield return new WaitWhile(() => RobotDialogue.Instance.IsDialoguePlaying());
        yield return new WaitUntil(() => Input.GetKeyUp(KeyCode.Backspace));
        CloseActivePlatform();
        _gameplay.UpdateState("PlatformIntroduction", true);
        Robot.Instance.Call();

    }

    public void MarkComplete() {
        _gameplay.UpdateState(string.Format("{0}Complete", ActiveContent.contentPlatform), true);
        if (ActiveContent.contentPlatform == Platform.Computer)
            return;
        RobotDialogue.Instance.QueueDialogue(string.Format("Well done! That's the {0} platform complete!", ActiveContent.contentPlatform.ToString().ToLower()));
        RobotDialogue.Instance.StartDialogue();

        if (_gameplay.CheckState("ComputerComplete") && _gameplay.CheckState("MobileComplete") && _gameplay.CheckState("TelevisionComplete")) {
            RobotDialogue.Instance.QueueDialogue("That's it! All the platforms have been finished!");
            RobotDialogue.Instance.QueueDialogue("I think we've been in this room long enough, let's leave.");
            RobotDialogue.Instance.StartDialogue();
            Doors.Instance.SetState(DoorState.Unlocked);
            CloseActivePlatform();
            InteractionEvent.CancelActiveEvent();

        }
    }
}
