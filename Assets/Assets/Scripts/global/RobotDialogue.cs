using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

/// <summary>
/// Description: A simple dialogue system to allow the helper robot to communicate with the player.
/// Usage: Put this script into the scene and assign the dialogueBox, robotText, and clickToContinue variables. 
/// ------ Add dialogue from any other script by calling RobotDialogue.Instance.QueueDialogue("Dialogue");
/// ------ Start and lock the dialogue pool by calling RobotDialogue.Instance.StartDialogue();
/// ------ Any other attempt to add more dialogue while the pool is in use will fail and the dialogue will be queued to add to the pool at the end.
/// </summary>

public class RobotDialogue : MonoBehaviour {
    public static RobotDialogue Instance;
    public GameObject dialogueBox, clickToContinue;
    public Text robotText;
    private readonly Queue<KeyValuePair<string, bool>> _dialoguePool = new Queue<KeyValuePair<string, bool>>();
    private bool _iterating; //Determine if the pool is being iterated across.
    private bool _userIsReady; //Determine if the user is ready to go to the next dialogue.
    private bool _queuedToPlay; //Determine if the dialogue system has been queued to play again after it has finished.
    public Button clickEvent;
    
    /// <summary>
    /// Open a URL within a new tab/window instead of replacing the game window.
    /// </summary>
    /// <param name="url"></param>
    [DllImport("__Internal")]
    private static extern void OpenUrl(string url);

    #region Unity Methods

    /// <summary>
    /// Assign any variables.
    /// </summary>
    private void Awake() {
        GetComponent<Canvas>().sortingOrder = 10;
       // clickEvent = GetComponentInChildren<Button>();
        clickEvent.onClick.AddListener(OpenGuidelineUrl);
        Instance = this;
        clickEvent.enabled = false;

        //Setup texts and fonts
        var font = Resources.Load<Font>("Open_Sans/OpenSans-Regular");
        robotText.font = font;
        robotText.fontSize = 11;
        robotText.resizeTextForBestFit = true;
        robotText.resizeTextMaxSize = 14;
        robotText.resizeTextMinSize = 11;
        clickToContinue.GetComponentInChildren<Text>().font = font;
    }

    private void Update() {
    //Checks if the user is ready to advance the on-screen dialogue.
    if (Input.GetKeyDown(KeyCode.Space) && !_userIsReady) {// && _iterating) {
            _userIsReady = true;
            Debug.Log("User is ready to continue.");
            clickToContinue.SetActive(false);
        }

        //Automatically re-launch dialogue iteration next frame if more dialogue was added during an iteration and asked to play.
        if (_queuedToPlay && !_iterating){
            _queuedToPlay = false;
            StartDialogue();
        }
    }

    #endregion

    #region RobotDialogue Methods

    /// <summary>
    /// Add dialogue to the queue for playback.
    /// </summary>
    /// <param name="dialogue">The dialogue string to queue. e.g. "Hello World! or "<color='blue'>Hello World!</color>" as rich text is supported.</param>
    /// <param name="clickable">Optional bool to tell the dialogue system if its going to be a clickable link to the guidelines.</param>
    public void QueueDialogue(string dialogue, bool clickable = false) {
        StartCoroutine(_AddToPool(new KeyValuePair<string, bool>(dialogue, clickable)));
    }

    /// <summary>
    /// Start the dialogue pool.
    /// </summary>
    public void StartDialogue() {
        if (_iterating) {
            _queuedToPlay = true;
            Debug.Log(string.Format("Dialogue pool is currently in use. Queued to play set to ==> '{0}'", _queuedToPlay));
            return;
        }
        StartCoroutine(_StartDialogue());
    }

    /// <summary>
    /// Clear the dialogue pool of any remaining/old dialogue items which were not iterated through.
    /// </summary>
    public void ClearQueue() {
        _dialoguePool.Clear();
        StopCoroutine(_StartDialogue());
        _iterating = false;
        _userIsReady = true;

    }

    /// <summary>
    /// Open the guideline url for the current scene. Guideline page mark is stored within the guidelineName array and is retrieved based on the current scene index.
    /// </summary>
    public void OpenGuidelineUrl() {
        //Split the camelcase scene name into individual words.
        //e.g. "KeyboardAccessible" -> "Keyboard", "Accessible".
        var words = Regex.Split(SceneManager.GetActiveScene().name, @"(?<!^)(?=[A-Z])");
        var sb = new StringBuilder();

        //Add the lowercase version of each word into a string.
        //For each word that is not the final world add a dash '-' afterwards. This is for the WCAG url format.
        foreach (var word in words) {
            sb.AppendFormat(word.ToLower() != words[words.Length - 1].ToLower() ? "{0}-" : "{0}", word.ToLower());
        }

        var url = string.Format("https://www.w3.org/TR/WCAG21/#{0}", sb);
        Debug.Log(string.Format("Opening URL: {0}", url));

#if !UNITY_EDITOR
        OpenUrl(url);
#endif

    }

    /// <summary>
    /// Adds the specified dialogue into the dialogue pool.
    /// If the pool is currently being used to display dialogue the request is instead queued and added when the pools iteration has finished.
    /// </summary>
    /// <param name="dialogue">The dialogue to add to the pool.</param>
    /// <returns></returns>
    private IEnumerator _AddToPool(KeyValuePair<string, bool> dialogue) {
        if (_iterating) {
            Debug.Log(string.Format("Rejecting request ==> '{0}'", dialogue));
            Debug.Log("Pool is being iterated across. Waiting for iteration to stop...");
            yield return new WaitUntil(() => !_iterating);
        }
        _dialoguePool.Enqueue(dialogue);
        Debug.Log(string.Format("'{0}' has been added to the pool.", dialogue));
        Debug.Log("Request complete.");
    }

    /// <summary>
    /// Start iterating across the dialogue pool and displaying it to the player.
    /// While this is in process no dialogue can be added to the pool.
    /// </summary>
    /// <returns></returns>
    private IEnumerator _StartDialogue() {
        //if (_queuedToPlay) _queuedToPlay = false;
        Debug.Log("Starting dialogue playback.");
        dialogueBox.SetActive(true);
        _iterating = true;

        while (_dialoguePool.Count > 0) {
            var kvp = _dialoguePool.Dequeue();
            Debug.Log(string.Format("Getting Dialogue from Pool ==> {0}", kvp.Key));
            robotText.text = kvp.Key;
            clickEvent.enabled = kvp.Value;
            _userIsReady = false;
            clickToContinue.SetActive(true);
            Debug.Log("Waiting for user to be ready...");
            yield return new WaitUntil(() => _userIsReady);
        }

        yield return new WaitWhile(() => _dialoguePool.Count > 0);
        Debug.Log("Dialogue pool complete.");

        yield return new WaitUntil(() => _userIsReady);
        robotText.text = "";
        _iterating = false;
        dialogueBox.SetActive(false);
        clickEvent.enabled = false;
        Debug.Log("Dialogue playback finished.");
        yield return new WaitForSecondsRealtime(0.5f);

    }

    /// <summary>
    /// Check if the user is finished reading the current dialogue text item and is ready to continue to the next/finish the dialogue sequence.
    /// </summary>
    /// <returns></returns>
    public bool IsUserReady(){
        return _userIsReady;
    }

    public bool IsDialoguePlaying(){
        return _iterating;
    }

    public bool IsPoolEmpty(){
        return _dialoguePool.Count == 0;
    }
#endregion
}
