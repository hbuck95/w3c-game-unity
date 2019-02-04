using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class ServerInterface : MonoBehaviour {
    public NavigableGameplay gameplay;
    public FPController con;
    public Map map;
    public GameObject transferDialog, icons;
    public Slider progressBar;
    public Text percentage, currentFile, timeLeft, robotIconName;
    public Image robotIcon, fillImage;
    private bool _downloadedFile;
    public Texture2D mouseCursor;

    public void Cancel() {
        RobotDialogue.Instance.QueueDialogue("Don't do that, we need the file to get out of here!");
        RobotDialogue.Instance.StartDialogue();
    }

    public void OnEnable() {
        StartCoroutine(_OnEnable());
        Cursor.SetCursor(mouseCursor, Vector2.zero, CursorMode.ForceSoftware);
    }

    public void OnDisable() {
        con.enabled = true;
        MouseBehaviour.KeepCursorVisible(false);
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private IEnumerator _OnEnable() {
        gameplay.UpdateState("Found_Server", true);
        con.enabled = false;
        MouseBehaviour.KeepCursorVisible(true);
        yield return new WaitForSeconds(1f);
        RobotDialogue.Instance.QueueDialogue("Oh look at that, there's also a floor plan that you can download on to me - how convenient!");
        RobotDialogue.Instance.QueueDialogue("We should be able to use it to navigate ourselves out of here.");
        RobotDialogue.Instance.StartDialogue();
        yield return new WaitUntil(() => RobotDialogue.Instance.IsUserReady());
        RobotDialogue.Instance.QueueDialogue("Okay, I've just connected myself up. You should see my icon appear on screen in a second.");
        RobotDialogue.Instance.StartDialogue();
        yield return new WaitForSeconds(2f);
        robotIcon.enabled = true;
        robotIconName.enabled = true;
        robotIcon.GetComponent<Collider2D>().enabled = true;
        yield return new WaitForSeconds(1f);
        RobotDialogue.Instance.ClearQueue();
        RobotDialogue.Instance.QueueDialogue("There it is! Drag the floor plan and encryption key on to it so I can download them.");
        RobotDialogue.Instance.StartDialogue();
    }

    private IEnumerator TransferFile(GameObject g) {
        icons.GetComponentsInChildren<DragUI>().ToList().ForEach(x => x.enabled = false);
        RobotDialogue.Instance.ClearQueue();
        transferDialog.SetActive(true);
        currentFile.text = string.Format("Current File: <b>{0}</b>", g.name == "Map" ? "FloorPlan.map" : "EncryptionKey.p12");
        yield return new WaitForSeconds(1);
        timeLeft.text = "Time Left: <b>6 Sec</b>";
        fillImage.enabled = true;

        //Transfer dialogue time left and the progress bar
        for (var i = 1; i <= 50; i++) {
            if (i % 5 == 0)
                timeLeft.text = string.Format("Time Left: <b>{0} Sec</b>", (50 - i )/10+1);
            progressBar.value = i*2;
            percentage.text = string.Format("{0}%", i * 2);
            yield return new WaitForSeconds(0.1f);
       
        }

        yield return new WaitForSeconds(1f);
        transferDialog.SetActive(false);
        percentage.text = "0%";
        timeLeft.text = "Time Left: <b>Time Left: ----</b>";
        fillImage.enabled = false;
        Debug.Log("Transfer complete.");
        Destroy(g);

        RobotDialogue.Instance.ClearQueue();
        RobotDialogue.Instance.QueueDialogue(g.name == "Map"
            ? "Now that I've got the floor plan I can guide us both out of here, to view the map just interact with my terminal."
            : "I can use this key to open up the lift doors, that will allow us to leave this floor and go to the next one.");
        RobotDialogue.Instance.StartDialogue();

        if (_downloadedFile) {
            yield return new WaitForSeconds(1.7f);
            yield return new WaitUntil(() => RobotDialogue.Instance.IsUserReady());
            RobotDialogue.Instance.QueueDialogue("That's everything, let's leave.");
            RobotDialogue.Instance.StartDialogue();
            Robot.Instance.SetAction(map.ToggleMap);
            yield return new WaitForSeconds(1f);
            transform.root.gameObject.SetActive(false);
            yield break;
        }
        RobotDialogue.Instance.QueueDialogue(string.Format("Now get the {0}!", g.name != "Map" ? "floor plan" : "encryption key"));
        RobotDialogue.Instance.StartDialogue();
        _downloadedFile = true;
        icons.GetComponentsInChildren<DragUI>().ToList().ForEach(x => x.enabled = true);
    }

    private void OnTriggerEnter2D(Collider2D c) {
        switch (c.gameObject.name) {
            case "Map":
                gameplay.UpdateState("Downloaded_Map", true);
                StartCoroutine(TransferFile(c.gameObject));
                Debug.Log("The player has downloaded the map");
                break;
            case "Key":
                gameplay.UpdateState("Downloaded_Key", true);
                StartCoroutine(TransferFile(c.gameObject));
                Debug.Log("The player has downloaded the encryption key.");
                break;
            default:
                RobotDialogue.Instance.ClearQueue();
                RobotDialogue.Instance.QueueDialogue("Not that file, get the floor plan and the encryption key!");
                RobotDialogue.Instance.StartDialogue();
                Debug.Log("That's not what we need");
                c.gameObject.GetComponent<DragUI>().Reset();
                break;
        }
    }
}
