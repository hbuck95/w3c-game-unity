using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Allows parent UI element object to be moved by the player through mouse input. (click and drag)
/// </summary>
[DisallowMultipleComponent]
[RequireComponent (typeof(RectTransform))]
public class DragUI : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler {
    private RectTransform _componentPosition;
    private Vector2 _startPos;
    public bool canDrag = true; //can this element currently be moved?

    public void OnEnable() {
        _componentPosition = GetComponent<RectTransform>();
        _startPos = _componentPosition.anchoredPosition;
    }

    protected void RefreshPosition() {
        _startPos = _componentPosition.anchoredPosition;
    }

    /// <inheritdoc />
    /// <summary>
    /// Repositions the parent object according to input.
    /// </summary>
    /// <param name="data">The event data collected from the primary input device (e.g. mouse)</param>
    public void OnDrag(PointerEventData data) {
        if(canDrag && Input.GetMouseButton(0))
        _componentPosition.position += new Vector3(data.delta.x, data.delta.y);
    }

    /// <summary>
    /// Resets this UI elements position to its inital position on the screen.
    /// This initial value is stored as soon as the level has finished loading through the Awake() behaviour.
    /// </summary>
    public void Reset() {
        if(gameObject.activeSelf)
            StartCoroutine(_Reset());
    }

    private void OnMouseOver() {
        canDrag = true;
    }

    private void OnMouseExit() {
        canDrag = false;
    }

    /// <summary>
    /// Resets this UI elements position to its inital position on the screen.
    /// This initial value is stored as soon as the level has finished loading through the Awake() behaviour.
    /// </summary>
    private IEnumerator _Reset() {
        canDrag = false;
        _componentPosition.anchoredPosition = _startPos;
        yield return new WaitForSeconds(1f);
    }

    public void OnPointerUp(PointerEventData eventData) {
        canDrag = false;
    }

    public void OnPointerDown(PointerEventData eventData) {
        canDrag = true;
    }

    protected void OnCollisionEnter2D(Collision2D c) {
        if (c.gameObject.CompareTag("UI_EdgeOfScreen"))
            Reset();       
    }

}
