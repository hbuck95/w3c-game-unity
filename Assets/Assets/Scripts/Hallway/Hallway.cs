using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hallway : MonoBehaviour {
    private List<HallwayDoor> _doors;
    public SceneLoader SceneLoader;

    private void Awake() {
        _doors = FindObjectsOfType<HallwayDoor>().OrderBy(x => x.DoorNumber).ToList();
        SceneLoader = FindObjectOfType<SceneLoader>();
        Setup();
    }

    private void Setup() {
        if (StandardManager.SelectedRequirement == Requirement.None) {
            Debug.LogWarning("No requirement set. Defaulting to first 'A' standard requirement.");
            StandardManager.SelectRequirement(StandardManager.A.DefaultRequirement());
        }

        Level[] levels = StandardManager.GetLevelsForRequirement(StandardManager.SelectedRequirement);

        for (var i = 0; i < levels.Length; i++)
            _doors[i].Setup(levels[i], this);
        
    }
}
