using UnityEngine;

/// <summary>
/// The object class for the items used (e.g. potato, fish, etc)
/// Class contains definitions for their description and name.
/// </summary>
public class Item : MonoBehaviour {
    public string itemName, itemDescription;

    /// <summary>
    /// Handle game events when the players cursor is over this object.
    /// </summary>
    private void OnMouseDown() {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            Debug.Log(itemDescription);
        }
    }
}
