using System.Collections;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

public class Lift : MonoBehaviour {
    public SceneLoader SceneLoader;
    public GameObject DoorLeft, DoorRight;
    private const float _Offset = 1.45f; //The degree of offset to move the doors by to open/close them.
    public DoorState State { get; private set; }
    private RaycastHit _hit;
    private Ray _ray;

    [Space]
    [Header("Lift Buttons:")]
    public GameObject ButtonPanel;
    private MeshRenderer _buttonRenderer;
    public Material[] ButtonPanelMaterials = new Material[4]; //Materials for each button panel. <-- Would have preferred a completed and uncompleted mat per for use on each button.
    public Material ButtonPanelNoneComplete;

    private void Start() {
        _buttonRenderer = ButtonPanel.GetComponent<MeshRenderer>();
        ConfigureButtons();

        StandardManager.SelectStandard(StandardManager.A);
    }

    private void Update() {
        CheckForEvents();


        if (Input.GetKeyDown(KeyCode.P)) {
            PlayerPrefs.DeleteAll();
            ConfigureButtons();
        }

        if (Input.GetKeyDown(KeyCode.O)) {
            StandardManager.MarkComplete(StandardManager.GetNextUncompletedRequirement(StandardManager.A));
            ConfigureButtons();
        }
    }

    /// <summary>
    /// Load a hallway instance for a specific requirement.
    /// </summary>
    /// <param name="requirement">
    /// Specified requirements array index within the selected standard.
    /// (e.g. StandardManager.A.Requirements[1] for the second requirements of the A standard.)
    /// </param>
    public void GoToFloor(int requirement) {
        if (requirement < 1 || requirement > StandardManager.SelectedStandard.Requirements.Length) {
            Debug.LogErrorFormat("Parameter must be a value that is within the bounds of the quantity of requirements for the selected standard.");
            Debug.LogErrorFormat("Selected Standard: {0}\nNumber of Requirements: {1}", StandardManager.SelectedStandard.Type, StandardManager.SelectedStandard.Requirements.Length);
            return;
        }

        StandardManager.SelectRequirement(StandardManager.SelectedStandard.Requirements[requirement-1]);
        SceneLoader.Load("Hallway");

    }

    // I would suggest recreating the button textures and just having one texture per button and rewriting this method.
    // The buttons will need to split into individual models so we can either hide or a put a plain texture on them for levels with less than 4 requirements. (e.g. Robust)
    // More buttons will need to be added in the case of AA or AAA requirements having more than 4 levels.
    // Assumes that the all prior floors must be complete in order to reach the current floor.
    // e.g. Perceivable must be complete in order to reach Operable.
    /// <summary>
    /// Sets up the button textures according to which requirements have been completed.
    /// </summary>
    private void ConfigureButtons() {
        if (StandardManager.SelectedStandard.Type == StandardType.AA || StandardManager.SelectedStandard.Type == StandardType.AAA) {
            Debug.LogWarning("The lift buttons are not setup for AA and AAA requirements as they only have one set of textures.");
            Debug.LogWarningFormat("The 'A' requirements will now be loaded instead of the '{0}' requirements.", StandardManager.SelectedStandard.Type);
        }

        //Replace with StandardManager.SelectedStandard.Requirements.Length when other requirements have been added.
        //Reverse the array as we can look to see if the last level is complete to see if the previous levels has been completed.     
        Requirement[] requirements = StandardManager.A.Requirements.Reverse().ToArray();

        for (var i = 0; i < requirements.Length; i++) {
            if (StandardManager.CheckCompletion(requirements[i])) {
                _buttonRenderer.material = ButtonPanelMaterials[i];
                return;
            }
        }

        _buttonRenderer.material = ButtonPanelNoneComplete;
        _buttonRenderer.material = ButtonPanelMaterials[3];
    }

    private void CheckForEvents()
    {
        _ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Physics.Raycast(_ray, out _hit, 5f, LayerMask.GetMask("Lift"));

        if (_hit.transform == null) {
            if (InteractionEvent.GetEventHash() != 0)
                InteractionEvent.CancelActiveEvent();
            return;
        }

        //If the event hash is the same as the hit object then return as this means that the hit object already has an event in progress.
        //By returning we stop the event from being cancelled and then recreated again.
        if (InteractionEvent.GetEventHash() == _hit.transform.gameObject.GetHashCode())
            return;

        if (InteractionEvent.GetEventHash() != 0)
            InteractionEvent.CancelActiveEvent();

        var floor = int.Parse(_hit.transform.name);
        InteractionEvent.CreateEvent(new Event('E', string.Format("to go to {0}", StandardManager.SelectedStandard.Requirements[floor - 1]), _hit.transform.gameObject.GetHashCode(), () => GoToFloor(floor)));

    }

    public void Open() {
        if (State == DoorState.Locked) return;
        StartCoroutine(_Open());
    }

    public void Close() {
        StartCoroutine(_Close());
    }

    public void SetState(DoorState state) {
        switch (state) {
            case DoorState.Locked:
            case DoorState.Closed:
                StartCoroutine(_Close());
                break;
            case DoorState.Open:
                StartCoroutine(_Open());
                break;
            case DoorState.Unlocked:
                break;
        }

        State = state;
        Debug.Log(string.Format("The door is now {0}.", state.ToString()));
    }

    private IEnumerator _Open() {
        Vector3 targetLeft = DoorLeft.transform.position;
        targetLeft.x += _Offset;

        Vector3 targetRight = DoorRight.transform.position;
        targetRight.x -= _Offset;

        while (Vector3.Distance(DoorLeft.transform.position, targetLeft) >= 0.1f) {
            DoorLeft.transform.position = Vector3.Lerp(DoorLeft.transform.position, targetLeft, 0.5f);
            DoorRight.transform.position = Vector3.Lerp(DoorRight.transform.position, targetRight, 0.5f);
            yield return null;
        }

    }
    
    private IEnumerator _Close() {
        Vector3 targetLeft = DoorLeft.transform.position;
        targetLeft.x -= _Offset;

        Vector3 targetRight = DoorRight.transform.position;
        targetRight.x += _Offset;

        while (Vector3.Distance(targetLeft, DoorLeft.transform.position) >= 0.1f) {
            DoorLeft.transform.position = Vector3.Lerp(targetLeft, DoorLeft.transform.position, 0.5f);
            DoorRight.transform.position = Vector3.Lerp(targetRight, DoorRight.transform.position, 0.5f);
            yield return null;
        }

    }

    private void OnTriggerEnter(Collider c) {
        if (c.CompareTag("Player"))
            SetState(DoorState.Open);
    }

    private void OnTriggerExit(Collider c) {
        if (c.CompareTag("Player"))
            SetState(DoorState.Closed);
    }

    public enum DoorState {
        Open,
        Closed,
        Locked,
        Unlocked
    }

}
