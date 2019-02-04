using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;
using System.IO;

[Obsolete("BundleManager has been deprecated as of 11th Jan 2018. Use QueryManager instead.")]
public class BundleManager : MonoBehaviour {
    private string _url = "file://G:/Unity/Greenwich%20Internship/Greenwich%20Internship/AssetBundles/WebGL/level1.dat";

    private IEnumerator Start() {
        UnityWebRequest req = UnityWebRequest.GetAssetBundle(_url);
        yield return req.Send();
        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(req);
        string[] scenes = bundle.GetAllScenePaths();
        SceneManager.LoadScene(Path.GetFileNameWithoutExtension(scenes[0]));
    }

    private static IEnumerator Load() {
        var u = Application.dataPath + "/StreamingAssets/level1.dat";
        UnityWebRequest req = UnityWebRequest.GetAssetBundle(u);
        yield return req.Send();
        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(req);
        var scenes = bundle.GetAllScenePaths();
        SceneManager.LoadScene(Path.GetFileNameWithoutExtension(scenes[0]));
    }

}
