using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Toolbox : MonoBehaviour {
    private List<PlatformElement> _UIElements;

	public void Populate() {
        if(_UIElements == null)
            _UIElements = GetComponentsInChildren<PlatformElement>().ToList();
    }

    public void ResetUI() {
        _UIElements.ForEach(x => x.Reset());
    }

    public void Toggle(bool state) {
        gameObject.SetActive(state);
    }

    public void Remove(PlatformElement p) {
        if(_UIElements.Contains(p))
            _UIElements.Remove(p);

        if (IsEmpty()) {
            Toggle(false);
            PlatformHandler.Instance.MarkComplete();

        }
    }

    public bool IsEmpty() {
        return _UIElements.Count == 0;
    }

}
