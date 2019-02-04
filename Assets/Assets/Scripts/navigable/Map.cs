using System.Linq;
using UnityEngine;
using UnityEngine.UI;

#pragma warning disable 649 //_playerIcon and _player are assigned via the inspector as they have been serialized.

/// <summary>
/// Handles the methods used for the map which helps the player navigate throughout the room maze.
/// </summary>
public class Map : MonoBehaviour {
    private Canvas _map;
    [SerializeField] private Image _playerIcon;
    [SerializeField] private GameObject _player;
    [SerializeField] private Text[] _breadcrumbs = new Text[6];//The trail of breadcrumbs in any order.
    private Text _currentCrumb; //The current crumb in the trail.
    private const float XOffSet = -826f; //The offset required for the player image to line up correctly (based off of players x-axis)
    private const float YOffSet = -366f; //The offset required for the player image to line up correctly (based off of players z-axis)

    /// <summary>
    /// Assign any variables and perform an initial setting up.
    /// </summary>
    private void Awake() {
        _map = GetComponent<Canvas>();
        _map.sortingOrder = 1; //Draw this canvas over the other scene canvases.
    }

    private void Update() {      
        if (!_map.enabled) return;
        _playerIcon.rectTransform.rotation = Quaternion.Euler(0, -0, _player.transform.rotation.eulerAngles.y * -1);
        _playerIcon.rectTransform.anchoredPosition = new Vector2(XOffSet + _player.transform.position.x * 4, YOffSet + _player.transform.position.z * 2.5f);
    }

    /// <summary>
    /// Toggle the maps visibility based on its current status.
    /// </summary>
    public void ToggleMap() {
        _map.enabled = !_map.enabled;
    }

    public bool IsActive() {
        return _map.enabled;
    }

    /// <summary>
    /// Highlights the crumb of the room/coridoor that the player is currently in.
    /// Blue = Current (i.e. html/css link styling)
    /// Black = Not-current
    /// </summary>
    /// <param name="location">The name of the room/coridoor the player is currently in.</param>
    public void HighlightCrumb(string location) {
        var crumb = _breadcrumbs.Single(x => location.Contains(x.name));
        if (_currentCrumb == crumb) return;
        if (_currentCrumb != null) _currentCrumb.color = Color.black;
        crumb.color = Color.blue;
        _currentCrumb = crumb;
    }

    public void ClearCrumb() {
        if (_currentCrumb == null) return;
        _currentCrumb.color = Color.black;
        _currentCrumb = null;
    }
}
