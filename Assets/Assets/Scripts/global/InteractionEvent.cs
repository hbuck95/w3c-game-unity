using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public struct Event {
    public Action Method;
    public char Key;
    public string Message;
    public int Hash; //Used to identify the gameobject the event is tied to (e.g. the hash of the mystery box in the first level)

    /// <summary>
    /// Creates a new interaction event where the player has to press the specified key in order to trigger an event/continue.
    /// Example Usage:
    /// - Delegates: InteractionEvent.CreateEvent(New Event('a', "Press 'A'!!", () => print("Hello world!"))); 
    /// - Methods: InteractionEvent.CreateEvent(New Event('a', "Press 'A'!!", MyMethod));
    /// 
    /// Supported input: Currently only standard A-Z latin characters.
    /// </summary>
    /// <param name="key">The character value of the key the player will need to press to trigger the event. (e.g. 'A')</param>
    /// <param name="message">The event description/prompt shown to the player. (e.g. "Press the button!") [Supports Rich Text]</param>
    /// <param name="hash">The hash of the object creating the event (e.g. gameobject.GetHash()). Used to identify events and cancel them if no longer facing the object. [Optional: Use 0 for anonymous events]</param>
    /// <param name="method">Optional: The action method to invoke when the event is completed. See: Example Usage</param>
    public Event(char key, string message, int hash = 0, Action method = null) {
        Key = key;
        Message = message;
        Method = method;
        Hash = hash;
    }

    /// <summary>
    /// Get the keycode value of the "key" character.
    /// </summary>
    /// <returns>The the keycode of the key used for the event.</returns>
    public KeyCode EventKey() {
        return (KeyCode)Enum.Parse(typeof(KeyCode), char.ToUpper(Key).ToString());
    }

}

public class InteractionEvent : MonoBehaviour {

    private static InteractionEvent Instance;
    private Canvas _display;
    private Text _eventDescription;
    [SerializeField] private Text _prefix;
    private Image _keyToPress;
    private bool _eventInProgress;
    private static bool _cancelEvent;
    [SerializeField] private Sprite[] _keySprites = new Sprite[26];
    private Event _currentEvent;

    /* Example Implementation *\
 *
 *
 *  Step 1: - Do the raycast in update/fixed update/where needed
 *  -------------------------------------------------------------
 *
 *  RaycastHit hit;
 *  var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
 *  if (Physics.Raycast(ray, out hit, 25f)) {
 *
 *  Step 2: - Compare the hit transforms name/tag/hash etc
 *  ---------------------------------------------------------------    
 *
 *  if(hit.transform.CompareTag("MyTag") {               
 *       InteractionEvent.CreateEvent(new InteractionEvent.Event('E', "Interact with this object!", hit.transform.gameObject.GetHashCode(), DoThing));
 *  } else {  
 *       if (InteractionEvent.Instance.GetEventHash() == myGameObject.GetHashCode())
 *           InteractionEvent.Instance.CancelActiveEvent();                   
 *  }
 *
 *  In the first part of step 2 we create the event based off of the object we're looking at. In this case its the object with the tag "MyTag".
 *  In the else chain we then compare to see if the current events hashcode is equal to any of our trigger objects hashcodes, if it is then we cancel the event as this will trigger when we're not looking at it.
 *
 *  NOTE: You will need to store a reference to the object you create the event based on. (e.g. public GameObject myGameObject;)
 *
 */

    private void Awake() {
        _display = GetComponent<Canvas>();
        _eventDescription = GetComponentInChildren<Text>();
        _keyToPress = GetComponentInChildren<Image>();
        Instance = this;
        _display.sortingOrder = 10; //Draw this canvas over the other scene canvases.
    }

    private IEnumerator _StartEvent(Event e) {
        _currentEvent = e;
        _eventInProgress = true;
        _eventDescription.text = e.Message;
        _prefix.fontSize = _eventDescription.fontSize; //The event text is set to best size, this will ensure that the "Press" prefix is resized correctly.
        _keyToPress.sprite = GetCharacterSprite(e.Key);
        _display.enabled = true;
        yield return new WaitUntil(() => Input.GetKeyDown(e.EventKey()) || _cancelEvent);
        _display.enabled = false;
        _eventInProgress = false;
        _currentEvent = new Event();

        if (_cancelEvent) {
            _cancelEvent = false;
            Debug.Log(string.Format("[InteractionEvent] The '{0}' event was cancelled and has ended prematurely.", e.Key));
            yield break;
        }

        Debug.Log(string.Format("[InteractionEvent] '{0}' Pressed. Ending Event.", e.Key));

        if (e.Method != null)
            e.Method();

    }


    //Mixing design patterns for ease of access as this will instead modify the singleton instance.
    //Alternative would have been to either design this class as a static class, or continue with singleton call.
    //Making this static reduces the length of code by a reasonable amount for the sacrifice of a few ns.
    //InteractionEvent.CreateEvent(new InteractionEvent.Event('foo', "bar", baz);
    //--------------------------------vs.-----------------------------------
    //InteractionEvent.Instance.CreateEvent(new InteractionEvent.Event('foo', "bar", baz);
    /// <summary>
    /// Create a new interaction event using the event struct.
    /// </summary>
    /// <param name="e">The event to be executed.</param>
    public static void CreateEvent(Event e) {
        if (Instance._eventInProgress) {
            // Debug.Log("Event Request Rejected. Another event is currently in progress.");         
            return;
            //TODO: Implement a queue system as done in the dialogue system if needed.
        }

        Instance.StartCoroutine(Instance._StartEvent(e));
    }

    /// <summary>
    /// Cancel the current active event.
    /// </summary>
    public static void CancelActiveEvent() {
        _cancelEvent = true;
    }

    private Sprite GetCharacterSprite(char c) {
        var v = char.ToLower(c) - 97;//97 is the integer value of 'a' and so we offset this to get to index 0.

        if (v >= 0 && v <= 26)
            return _keySprites[v];

        Debug.Log(string.Format("Error: '{0}' is not a supported character.", c));
        Debug.Log("Supported characters are A-Z standard characters.");
        return new Sprite();//Return a new blank sprite (white square) so there is a visual error on screen if within a build.
    }

    /// <summary>
    /// Is there an interaction event currently in progress?
    /// </summary>
    /// <returns></returns>
    public static bool EventInProgress() {
        return Instance._eventInProgress;
    }

    /// <summary>
    /// Returns the hash code from the current event which points to the gameobject which initiated the event.
    /// </summary>
    /// <returns></returns>
    public static int GetEventHash() {
        return Instance._currentEvent.Hash;
    }
}
