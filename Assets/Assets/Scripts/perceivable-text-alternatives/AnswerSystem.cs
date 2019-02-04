using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class AnswerSystem : MonoBehaviour {
    public CodeEditorInteraction codeEditorInteraction;
    public GameObject codeEditor, endGame;
    private string _playerDesc; //The custom description entered by the player. We can use this to check if the player has entered anything.
    public Text robotText;
    private const string _AltTag = "<color='#{0}'><img <color='#{1}'>src</color>=\"item_???.jpg\" <color='#{1}'>alt</color>=\"{2}\" <color='#{1}'>height</color>=\"256\" <color='#{1}'>width</color>=\"256\"></color>";
 

    #region UnityToJS.jslib
    /// <summary>
    /// Prompts the user to enter a valid description for the mystery object identified through the JS function 'window.prompt' and returns a UTF-8 formatted string.
    /// </summary>
    /// <returns>UTF-8 String of input.</returns>
    [DllImport("__Internal")]
    private static extern string SubmitDescription();

    #endregion

    #region Unity Methods

    //Can get rid of this by using the MouseEnter/Exit/Click functionality on the code editor.
    private void Update () {
        if (codeEditorInteraction.Hovering) {
            if (Input.GetKeyDown(KeyCode.Mouse0))
                WriteDescription();
            FormatTag(true);
        } else {
            FormatTag();
        }
    }

    public void HideEditor() {
        codeEditorInteraction.Selected = false;
        codeEditorInteraction.Hovering = false;
        codeEditor.transform.root.gameObject.SetActive(false);
    }

    #endregion

    #region AnswerSystem

    /// <summary>
    /// Allows the player the write a new description for the mystery item using the UnityToJS library method 'SubmitDescription()' to interact with the browser.
    /// </summary>
    public void WriteDescription() {
#if !UNITY_EDITOR
        _playerDesc = SubmitDescription().ToLower();
#else
        Debug.LogWarning("Can't access UnityToJS plugin within the editor. You must be in a WEBGL build for U2JS functionality.");
        _playerDesc = MysteryBox.Instance.GetCurrentItem().itemName.ToLower();
#endif
        FormatTag();
        StartCoroutine(_CheckAnswer());
    }

    private IEnumerator _CheckAnswer() {

        if (string.IsNullOrEmpty(_playerDesc)) {
            Debug.LogError("The player has not entered a description yet!");
            yield break;
        }
        
        //Check to see if the description entered by the player contains the answer keyword
        //e.g. For the Birthday Cake:
        //     Player Desc: A Birthday Cake
        //     Keyword: Cake
        if (_playerDesc.Contains(MysteryBox.Instance.GetCurrentItem().itemName.ToLower())) {
            RobotDialogue.Instance.ClearQueue();
            RobotDialogue.Instance.QueueDialogue("Nice! You got it correct! Onto the next item, just open the box to reveal!");
            RobotDialogue.Instance.StartDialogue();
            yield return new WaitWhile(() => RobotDialogue.Instance.IsDialoguePlaying());

            HideEditor();
            MysteryBox.Instance.NewBox();
        } else {
            RobotDialogue.Instance.ClearQueue();
            RobotDialogue.Instance.QueueDialogue("Nope. Want to try again?");
            RobotDialogue.Instance.QueueDialogue(MysteryBox.Instance.GetCurrentItem().itemDescription);
            RobotDialogue.Instance.StartDialogue();
            yield return new WaitWhile(() => RobotDialogue.Instance.IsDialoguePlaying());
            WriteDescription();
        }

        _playerDesc = null;

    }

    /// <summary>
    /// Formats the html alt tag within the editor window. 
    /// </summary>
    /// <param name="invert">Should the colours be inverted? (Inverted colours are used to signify that the cursor is hovering over it.)</param>
    private void FormatTag(bool invert = false) {
        var descToDisplay = string.IsNullOrEmpty(_playerDesc) ? MysteryBox.Instance.GetCurrentItem().itemDescription : _playerDesc;
        codeEditorInteraction.htmlTag.text = codeEditorInteraction.ColourAltTag(_AltTag, descToDisplay, invert);
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

#endregion
}
