using UnityEngine;

public class MysteryBoxInteraction : MonoBehaviour {

    /// <summary>
    /// Handle game events when the players cursor is over this object.
    /// </summary>

    private void Update() {

        if (!Input.GetKeyDown(KeyCode.Mouse0)) return;

        RaycastHit hit;
        var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out hit, Mathf.Infinity)) {
            if (hit.transform.CompareTag("MysteryBox"))
            {
                MysteryBox.Instance.OpenBox();
                RobotDialogue.Instance.QueueDialogue(MysteryBox.Instance._currentObject.itemDescription);
                RobotDialogue.Instance.StartDialogue();
            }
        }
    }
    //private void OnMouseDown() {
    //    if (Input.GetKeyDown(KeyCode.Mouse0)) {
    //        MysteryBox.Instance.OpenBox();
    //        Debug.Log("Opening Mystery Box.");
    //    }
    //}
}
