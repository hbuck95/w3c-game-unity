using UnityEngine;
using System.Collections.Generic;

public class MysteryBox : MonoBehaviour {
    public static MysteryBox Instance;
    public List<GameObject> objectModels = new List<GameObject>();
    public Item _currentObject;
    public Transform displayPosition;
    public GameObject displayObject, mysteryBoxObject, codeEditor;
    public int _openedBoxes;
    private bool _isOpen;
    private const int BOX_LIMIT = 5;
    public TextAlternativeGameplay gameplay;

#region Unity Functions

    /// <summary>
    /// Create the singleton instance and setup any variables needed within this script.
    /// </summary>
    private void Start () {
        Instance = this; //Create a singleton instance to reference later.
        _openedBoxes = 0;
	}

    #endregion

    #region Box methods

    /// <summary>
    /// Opens up a mystery box to the player to display one of the random items/images.
    /// </summary>
    public void OpenBox() {
        Debug.Log("Opening mystery box!");
        _isOpen = true;
       
        //Get a random box item to instantiate and position it where the placeholder is positioned.
        var boxItem = Instantiate(GetBoxItem());
        _currentObject = boxItem.GetComponent<Item>();
        boxItem.transform.position = displayPosition.position;
        _openedBoxes++;

        //When opening the box just set the box to inactive and we can just re-enable it when we need it again.     
        mysteryBoxObject.SetActive(false);
        codeEditor.SetActive(true);

        RobotDialogue.Instance.QueueDialogue(_currentObject.itemDescription);
        RobotDialogue.Instance.StartDialogue();
    }

    /// <summary>
    /// Create a new mystery box within the game world for the player to open
    /// </summary>
    public void NewBox() {
        //Destroy the current object if one exists.
        if (_currentObject != null)
            Destroy(_currentObject.gameObject);

        //If the player has opened the required amount of boxes to complete the level then we complete the level.
        if (_openedBoxes == BOX_LIMIT){
            _LevelComplete();
            return;
        }
        mysteryBoxObject.SetActive(true);
        _isOpen = false;
    }

    /// <summary>
    /// Gets a random model from the objectModels array using the System.Random function within the C# lib.
    /// Remove the random model from the list of models so it cannot be returned more than once.
    /// </summary>
    /// <returns>Random objectModels gameobject.</returns>
    public GameObject GetBoxItem() {
        var model = objectModels[new System.Random().Next(0, objectModels.Count)];
        objectModels.Remove(model);
        return model;
    }

    /// <summary>
    /// Get the current mystery box item.
    /// </summary>
    /// <returns>An 'Item' object of the current mystery box item.</returns>
    public Item GetCurrentItem() {
        return _currentObject;
    }

    /// <summary>
    /// Check if mystery box has been opened.
    /// </summary>
    /// <returns></returns>
    public bool IsOpen() {
        return _isOpen;
    }

    /// <summary>
    /// The dialogue 
    /// </summary>
    private void _LevelComplete(){
        Robot.Instance.Call();
        RobotDialogue.Instance.QueueDialogue("You did it! You managed to successfuly guess all of the items!");
        RobotDialogue.Instance.QueueDialogue("Hopefully now you can understand the importance of implementing text alternatives.");
        RobotDialogue.Instance.QueueDialogue("Click here to take a look at the 'Text Alternatives' guideline on the w3c website.", true);
        RobotDialogue.Instance.StartDialogue();
        gameplay.doors.SetState(DoorState.Unlocked);
        gameplay.doors.Open();
        //Achievement here for completing the Text Alternatives level.
    }
}

    #endregion


