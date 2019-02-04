using UnityEngine;
using System.Text.RegularExpressions;

/// <summary>
/// A class for handling each Hallway door in the Hallway transition scene for the individual requirements. 
/// </summary>
public class HallwayDoor : MonoBehaviour {
    public Level TargetLevel = Level.None;
    public Hallway Hallway;
    public GameObject Door; //Apply motion/force/or animation if and when needed.
    public GameObject LevelLabel; //Setup the materials for the level label object. Unfortunately only materials for 4 labels were made.
    private Event _triggerEvent;
    public int DoorNumber; //used for apply levels to doors. The lower the number the closer it is to the lift.

    private void Start() {
        /* If this hallway is a requirement where there are less levels the the number of doors then destroy this door.
         * Also destroy this door if the level has already been completed.
         * (e.g. Robust for the 'A' standard which has only 1 level)
         * This works as Setup() is called during Awake() from the Hallway script and so has a higher execution priority. */
        if (TargetLevel == Level.None || StandardManager.CheckCompletion(TargetLevel))
            Destroy(this);
    }

    private void OnTriggerEnter(Collider c) {
        if (c.CompareTag("Player"))
            InteractionEvent.CreateEvent(_triggerEvent);
    }

    private void OnTriggerExit(Collider c) {
        if (c.CompareTag("Player"))
            InteractionEvent.CancelActiveEvent();
    }

    public void Setup(Level l, Hallway h) {
        TargetLevel = l;
        Hallway = h;
        _triggerEvent = new Event('E', string.Format("to go to {0}", CamelCaseToSentence(TargetLevel.ToString())), 0, () => Hallway.SceneLoader.Load(TargetLevel.ToString()));
    }

    private static string CamelCaseToSentence(string input) {
        return Regex.Replace(input, "(?<=[a-z])([A-Z])", " $1", RegexOptions.None).Trim();
    }

}
