using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class MainMenu : MonoBehaviour {
    [Header("UI Elements:")]
    public GameObject LoginButton;
    public GameObject A, AA, AAA;
    public GameObject RegisterButton, ForgottenPasswordButton;
    private List<Image> _aImage, _aaImage, _aaaImage;
    public Sprite StandardGlow, StandardNoGlow;
    public VideoPlayer BackgroundPlayer;
    public RawImage BackgroundTexture;

    [Space]
    [Header("Scene Loading:")]
    public SceneLoader SceneLoader;

    [Space]
    [Header("Player Information:")]
    public GameObject playerInformation;
    public Text playerName, playerAchievements;
    public Image playerAvatar;
    public Player player;

    //Does not work.
    //Need to modify the UnityLoader.js to open the game window via javascript in order to close it.
    //Or find a JS exploit to force the browser to the tab.
    [DllImport("__Internal")]
    private static extern void Close();

    /// <summary>
    /// Initialise the image lists by grabbing all the images in the children of the standard objects.
    /// </summary>
	private void Awake() {
        PlayerPrefs.DeleteAll();
        StartCoroutine(SetupVideo());
	    _aImage = A.GetComponentsInChildren<Image>().ToList();
	    _aaImage = AA.GetComponentsInChildren<Image>().ToList();
	    _aaaImage = AAA.GetComponentsInChildren<Image>().ToList();
        SelectStandard(1); //Select 'A' standard by default.
        SetupPlayer();
	}

    private IEnumerator SetupVideo() {
        if (string.IsNullOrEmpty(BackgroundPlayer.url))
            BackgroundPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "RobotHover1.m4v");
        BackgroundPlayer.Prepare();
        yield return new WaitWhile(() => !BackgroundPlayer.isPrepared);
        BackgroundTexture.texture = BackgroundPlayer.texture;
        BackgroundPlayer.Play();
    }

    /// <summary>
    /// Select settings for a specific defined W3C standard.
    /// </summary>
    /// <param name="standard">Integer representation of the defined standard: AA (1), AA (2), AAA (3)</param>
    public void SelectStandard(int standard) {
        if (standard < 1 || standard > 3) {
            Debug.LogErrorFormat("No standard defined for '{0}'\nDefined Standards: AA (1), AA (2), AAA (3)", standard);
            return;
        }

        StandardManager.SelectStandard(StandardManager.GetStandard((StandardType) standard));
        StandardManager.SelectRequirement(StandardManager.GetNextUncompletedRequirement(StandardManager.SelectedStandard));

        switch (standard) {
            case 1:
                _aImage.ForEach(x => x.sprite = StandardGlow);
                _aaImage.ForEach(x => x.sprite = StandardNoGlow);
                _aaaImage.ForEach(x => x.sprite = StandardNoGlow);
                return;
            case 2:
                _aImage.ForEach(x => x.sprite = StandardNoGlow);
                _aaImage.ForEach(x => x.sprite = StandardGlow);
                _aaaImage.ForEach(x => x.sprite = StandardNoGlow);
                Debug.LogWarning("AA standards have not yet been implemented. Defaulting to A standards instead.");
                StandardManager.SelectStandard(StandardManager.A); //Remove this when AA standards have been added.
                return;
            case 3:
                _aImage.ForEach(x => x.sprite = StandardNoGlow);
                _aaImage.ForEach(x => x.sprite = StandardNoGlow);
                _aaaImage.ForEach(x => x.sprite = StandardGlow);
                Debug.LogWarning("AAA standards have not yet been implemented. Defaulting to A standards instead.");
                StandardManager.SelectStandard(StandardManager.A); //Remove this when AAA standards have been added.
                return;
            default:
                throw new ArgumentOutOfRangeException();
        }

    }

    private void SetupPlayer() {
        //Setup any saved settings here.
        //Load any player data.

        var b = LoginButton.GetComponent<Button>();
        b.onClick.RemoveAllListeners();

        //If the player is null then the player has not logged in.
        var loggedIn = player != null;

        if (!loggedIn) {
            b.onClick.AddListener(Login);
            LoginButton.GetComponentInChildren<Text>().text = "Login";
        } else {
            b.onClick.AddListener(Logout);
            LoginButton.GetComponentInChildren<Text>().text = "Logout";
            playerName.text = player.Username;
            playerAchievements.text = player.Achievements.ToString();
            //playerAvatar.sprite = player.Avatar; //<--- Uncomment this out when the database logic for this has been added.
        }

        playerInformation.SetActive(loggedIn);
        ForgottenPasswordButton.SetActive(!loggedIn);
        RegisterButton.SetActive(!loggedIn);

    }

    /// <summary>
    /// Button press event for the 'Start Game' button.
    /// In turn calls the Load function within SceneLoader and passes the _levelToLoad as param.
    /// </summary>
    public void StartGame() {
        SceneLoader.LoadHallway();
    }

    public void Login() {
        //Database calls here.
        player = new Player("BestPlayer84", 15);
        SetupPlayer();
    }

    public void Logout() {
        player = null;
        SetupPlayer();
    }

    public void Register() {
        //Database calls here.
        //Show register fields etc.
        throw new NotImplementedException();
    }

    public void ViewLeaderboards() {
        //Database calls here.
        //Show leaderboard UI etc.
        throw new NotImplementedException();
    }

    public void ViewAchievements() {
        //Database calls here?
        //Alternatively player prefs for locally storing achievement information.
        //Show Achievement UI etc.
        throw new NotImplementedException();
    }

    public void Settings() {
        //Database calls here?
        //Depending on if you wish to store game configuration so that the player can pick up their settings on a different system.
        //Alternatively player prefs/config file.
        //Show settings UI etc.
        throw new NotImplementedException();
    }

    public void Help() {
        //Show Help UI
        throw new NotImplementedException();
    }

    public void About() {
        //Show About UI
        throw new NotImplementedException();

    }

    public void ForgottenPassword() {
        throw new NotImplementedException();
    }

    public void Quit() {
        //Ask 'are you sure you want to quit?' etc
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
//Need to modify UnityLoader to load the game within a new script created tab in order to close the window for quit.
//Only windows/tabs created via a js script can be closed within a js script.
        Close();
#endif
    }

}
