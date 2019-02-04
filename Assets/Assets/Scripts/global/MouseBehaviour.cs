using UnityEngine;

public class MouseBehaviour: MonoBehaviour {
    private static MouseBehaviour _Instance;

    public static MouseBehaviour Instance {
        get {
            if (_Instance == null) {
                _Instance = FindObjectOfType<MouseBehaviour>();
                if (_Instance == null) {
                    var g = new GameObject { name = "ScriptHolder" };
                    _Instance = g.AddComponent<MouseBehaviour>();
                }
            }
            return _Instance;
        }
    }

    public bool cameraLock;
    private static bool _keepCursorVisible;


    private void Start() {
        //Setup cursor defaults.
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    public static void KeepCursorVisible(bool state) {
        _keepCursorVisible = state;
        Cursor.visible = state;
        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Confined;
    }

 
    private void Update () {
        if (_keepCursorVisible) return;

        //Toggle between having the cursor visible on the screen.
        //if (Input.GetKey(KeyCode.Period)) {
        //    Cursor.lockState = Cursor.lockState == CursorLockMode.Confined ? CursorLockMode.Locked : CursorLockMode.Confined;
        //    Cursor.visible = !Cursor.visible;
        //}

        //Toggle between having the cursor locked/unlocked.
        if (Input.GetKeyDown(KeyCode.Period)) {
            Cursor.visible = !Cursor.visible;
            cameraLock = Cursor.visible;
            Cursor.lockState = Cursor.lockState == CursorLockMode.None ? CursorLockMode.Confined : CursorLockMode.None;
            
        }

    }

}
