using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public class SceneLoader : MonoBehaviour {
    private VideoPlayer _player;
    private bool _loading;
    private string _sceneToLoad;

    public void Load(string sceneName){
        _sceneToLoad = sceneName;
        StartCoroutine(_Load());
    }

    /// <summary>
    /// Quick method call to load the hallway scene.
    /// Used within the interaction events to be passed as the parameter action to exit doors (levels) and return to the hallway.
    /// </summary>
    public void LoadHallway() {
        _sceneToLoad = "Hallway";
        StartCoroutine(_Load());
    }

    private void Awake() {
        _player = GetComponent<VideoPlayer>();
        _player.source = VideoSource.Url;
        _player.audioOutputMode = VideoAudioOutputMode.Direct;
        _player.renderMode = VideoRenderMode.CameraNearPlane;
        _player.targetCamera = Camera.main;
        _player.isLooping = true;
        _player.playOnAwake = false;

        //Note: WEBGL does not support loading video clips from any source which is not from a URL.
        //Load the clip from streaming assets if it hasn't been set.
        if(string.IsNullOrEmpty(_player.url))
            _player.url = System.IO.Path.Combine(Application.streamingAssetsPath, "LoadingScreen.mp4");

    }

    private IEnumerator _Load() {
        if (_loading) {
            Debug.LogError("A scene is already loading!");
            yield break;
        }

        //Check and see if a level has been set to load.
        if (string.IsNullOrEmpty(_sceneToLoad)){
            Debug.LogErrorFormat("Can't load a scene which has not been set.\nMake sure that the 'LevelToLoad' variable has been set in the inspector for 'LoadScene' on '{0}'.", name);
            yield break;
        }

        //Check and see if that scene is valid by looking through the build settings.
        //If the scene has not been added to the build settings it won't be valid.
        if (!IsSceneValid(_sceneToLoad)) {
            Debug.LogErrorFormat("'{0}' is not a valid scene.", _sceneToLoad);
            yield break;
        }

        //Destroy all UI elements in the scene so the loading screen can be played directly to the camera viewport.
        //This is needed as UI elements are always the first thing drawn on screen and will hide the video player.
        //Destroy rather than disable as some are enabled/disabled automatically by scripts (i.e. interaction events)
        FindObjectsOfType<Canvas>().ToList().ForEach(x => Destroy(x.gameObject));

        //Play the loading screen
        _player.Play();

        //Load the level
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(_sceneToLoad);
        loadOperation.allowSceneActivation = false;
        _loading = true;

        while (!loadOperation.isDone) {
            var p = Mathf.Clamp01(loadOperation.progress / 0.9f) * 100; //Load operation stops at 0.9 as 1.0 is reserved for activation.
            Debug.Log(string.Format("Progress: {0}%", p));

            //Get a confirmation that the scene has loaded and then load rather than activating it instantly.
            if (Mathf.Approximately(loadOperation.progress, 0.9f)) {
                Debug.Log("Load Complete!");
                _loading = false;
                loadOperation.allowSceneActivation = true;
            }
           //yield return new WaitForSeconds(2f); //<-- delay to test the loading screen as it loads extremely fast.   
            yield return null;
        }

    }

    /// <summary>
    /// Is the given scene name a valid scene?
    /// Performs an interation across all scenes listed in the build and checks if the param scene is found.
    /// </summary>
    /// <param name="sceneName">bool</param>
    /// <returns></returns>
    private static bool IsSceneValid(string sceneName) {
        var scenes = new List<string>();

        //Cannot use SceneManager.GetSceneByBuildIndex(i) as the value will always be null unless the scene had been loaded during the current play time of the application.
        //https://issuetracker.unity3d.com/issues/scenemanager-dot-getscenebybuildindex-dot-name-returns-an-empty-string-if-scene-is-not-loaded
        for (var i = 0; i < SceneManager.sceneCountInBuildSettings; i++) {
            var path = SceneUtility.GetScenePathByBuildIndex(i);     
            scenes.Add(path.Substring(0, path.Length - 6).Substring(path.LastIndexOf('/') + 1));
        }

        return scenes.Contains(sceneName);
    }

}



