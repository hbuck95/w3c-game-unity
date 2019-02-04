using UnityEngine;
using UnityEngine.UI;

public class CodeEditorInteraction : MonoBehaviour {
    public Text htmlTag;
    public bool Hovering { get; set; }
    public bool Selected { get; set; }
    private static readonly string[] colours = { "4292f4", "e60000", "BD6D0B", "19FFFF" }; //Dark Red > Blue > Light Red > Orange (0-1 = default colours || 2-3 = inverted hexadecimal values)
    public GameObject editorPanel;
    public FPController controller;

    /// <summary>
    /// Handle any input events.
    /// </summary>
	private void Update () {
	    if (Input.GetKeyDown(KeyCode.Mouse0))
            Selected = Hovering;
    }

    /// <summary>
    /// MouseOver()/PointerEnter() functionality used to interact with Unitys event system.
    /// </summary>
    public void Enter() {
        if (Selected) return;
        Debug.Log("Hovering over alt tag!");
        Hovering = true;
    }

    /// <summary>
    /// MouseExit()/PointerExit() functionality used to interact with Unitys event system.
    /// </summary>
    public void Exit() {
        Debug.Log("No longer hovering over alt tag!");
        Hovering = false;
        if (Selected) return;
    }

    /// <summary>
    /// Handles the html colour formatting for the alt tag within the code editor.
    /// - Blue/Red when not selected
    /// - Orange/Blue when selected (inverted colours based on their hexadecimal value)
    /// </summary>
    /// <param name="invert">Optional Parameter - Whether to invert the hexadecimal colour codes. (False by default)</param>
    public string ColourAltTag(string htmltag, string desc, bool invert = false) {
        if (Selected) invert = true;//When the text is selected we always want to invert the colour regardless.
        //Load the description which should be displayed (Check if their is a user description, if not then load the default)
        return !invert ? string.Format(htmltag, colours[0], colours[1], desc) : string.Format(htmltag, colours[2], colours[3], desc);
    }

    private void OnEnable(){
        MouseBehaviour.KeepCursorVisible(true);
        controller.enabled = false;
    }

    private void OnDisable() {
        MouseBehaviour.KeepCursorVisible(false);
        controller.enabled = true;
    }

    public void Show() {
        MouseBehaviour.KeepCursorVisible(true);
        controller.enabled = false;
        editorPanel.SetActive(true);
    }

    public void Hide() {
        MouseBehaviour.KeepCursorVisible(false);
        controller.enabled = true;
        editorPanel.SetActive(false);
    }

    public bool IsOpen() {
        return editorPanel.activeSelf;
    }


}
