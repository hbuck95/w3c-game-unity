using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Object class for the AGE game player.
/// Generate this class and its values through the database login call.
/// </summary>
public class Player {

    public string Username { get; private set; }
    public int Achievements { get; private set; }
    public Sprite Avatar { get; internal set; }

    /// <summary>
    /// Create a player from a deserialized json player object.
    /// </summary>
    /// <param name="p"></param>
    public Player(Player p) {
        Username = p.Username;
        Achievements = p.Achievements;
    }

    public Player(string username, int achievements) {
        Username = username;
        Achievements = achievements;
    }

}

//Player does not inherit from Monobehaviour as it is created via its constructor.
//PlayerHandler can be used to make database calls on behalf of a player and perform other duties as needed.
//Alternatively create a new monobehaviour instance of Player via AddComponent and create a Player.Create method to setup properties.
public class PlayerHandler : MonoBehaviour {
    private static PlayerHandler _Instance;

    public static PlayerHandler Instance {
        get {
            if (_Instance == null) {
                _Instance = FindObjectOfType<PlayerHandler>();

                if (_Instance == null) {
                    var g = new GameObject {name = "ScriptHolder"};
                    _Instance = g.AddComponent<PlayerHandler>();
                }
            }

            return _Instance;
        }
    }

    private IEnumerator<WWW> _GetAvatar(Player p) {
        //New WWW call with the server url, any post data, and any specific headers.
        using (var www = new WWW("URL_GOES_HERE")) {
            yield return www;
            var t = www.texture;
            p.Avatar = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f));
        }
    }

}
