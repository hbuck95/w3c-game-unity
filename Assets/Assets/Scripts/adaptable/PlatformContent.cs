using UnityEngine;

public class PlatformContent : MonoBehaviour {
    public Platform contentPlatform;
    public Toolbox Toolbox;

    private void OnEnable() {
        MouseBehaviour.KeepCursorVisible(true);
    }

    private void OnDisable() {
        MouseBehaviour.KeepCursorVisible(false);
    }

}
