using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Runtime.InteropServices;

public class QueryManager : MonoBehaviour {

    #region UnityToJS.jslib
    /// <summary>
    /// Pulls the raw url parameter arguments using the UnityToJS library.
    /// </summary>
    /// <returns>UTF-8 formatted string</returns>
    [DllImport("__Internal")]
    private static extern string PassArgs();

    #endregion

    #region Unity Methods
    /// <summary>
    /// Unity initialisation.
    /// Also handles startup parameter checks (e.g. checking if an initial level has been specified)
    /// </summary>
    private void Start () {

        //Platform dependent compilaton used to avoid accessing the UnityToJS library within the editor and causing crashes.
#if !UNITY_EDITOR
        //Do an inital param check to see if it tells us to load a level or setup a specific user/gameplay element.
        var param = PassArgs();

        //If no param has been specified for this scene load we automatically start on the first level.

        //if (string.IsNullOrEmpty(param)) {
        //    Debug.Log("No inital param parsed. Loading level 1.");
        //    if (SceneManager.GetActiveScene().buildIndex == 0) {
        //        Debug.Log("This is already the default scene!");
        //        return;
        //    }
        //    SceneManager.LoadScene(0);
        //}
        //if (SceneManager.GetActiveScene().buildIndex == 0) {
        //    if (string.IsNullOrEmpty(param)) {
        //        Debug.Log("No inital param parsed. Loading level 1.");
        //        SceneManager.LoadScene(0);
        //        return;
        //    }
        //} 

        if(string.IsNullOrEmpty(param))
            return;

        ReceiveArgs(param);
#endif
    }

    #endregion

    /// <summary>
    /// Handles query string parameters parsed from the UnityToJS library method PassArgs.
    /// </summary>
    /// <param name="args">The raw argument parameters (e.g. "level=1")</param>
    public void ReceiveArgs(string args) {

        if (string.IsNullOrEmpty(args)) {
            Debug.Log("No arguments parsed.");
            return;
        }

        //split the argument list and sort them into KeyValuePairs
        var argList = args.Split('&');
        var kvps = argList.Select(arg => arg.Split('=')).ToDictionary(kvp => kvp[0], kvp => kvp[1]);

        foreach (var kvp in kvps) {
            RunParam(kvp);
        }

    }

    #region QueryManager

    /// <summary>
    /// Executes the input parameters in the form of a KVP.
    /// </summary>
    /// <param name="param">KeyValuePair where the key is the argument. (e.g. '<"level","1">'</param>
    private void RunParam(KeyValuePair<string,string> param) {

        switch (param.Key) {
            case "level":
                //Stop an infinite loading loop.
                if (SceneManager.GetActiveScene().buildIndex == int.Parse(param.Value) - 1) {
                    Debug.Log(string.Format("Scene {0} is the current scene. Aborting scene load.", param.Value));
                    return;
                }

                try {
                    Debug.Log(string.Format("Loading level {0}", param.Value));
                    SceneManager.LoadScene(int.Parse(param.Value) - 1);
                } catch (IndexOutOfRangeException e) {
                    Debug.LogException(e);
                    Debug.Log("No such level exists!");
                }

                return;
            default:
                Debug.Log(string.Format("Parameter '{0}' is not supported.", param.Key));
                return;
        }

    }
}

#endregion