using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// An extension class for DragUI used for the platform elements within the adaptability level.
/// Makes use of all the DragUI methods used throughout other levels but also requires some additional functionality.
/// </summary>
public class PlatformElement : DragUI {
    private bool _colliding;

    private void OnCollisionStay2D(Collision2D c) {

        //Stop images being applied to more than 1 element if the collision area 
        //encompasses more than 1 placeholder of the same type.
        //Also compare the tag to see if it is a component which can be placed within the element.
        if (_colliding || !c.gameObject.CompareTag(tag)|| gameObject.layer != c.gameObject.layer) return;

        if (c.gameObject.name == name && Input.GetMouseButtonUp(0)) {

            _colliding = true; //We don't need to reset this as when it is true it's destroyed at the bottom of the function.

            var element = c.gameObject.GetComponent<PlatformElementGeneration>();

            //Does this object already have its image displayed? If it does then return.
            //Needed for instances where there are images of the same type.
            if (element.ImageDisplayed) return;

            element.ShowImage();

            //Remove this asset as it's no longer needed.
            try {
                transform.root.GetComponent<PlatformContent>().Toolbox.Remove(this);
            } catch (NullReferenceException nrex) {
                Debug.LogWarningFormat(nrex.StackTrace, "Null reference when attempting to remove the {0} element from the toolbox.\nThis should just mean it was already removed however.", name);
            }

            Destroy(gameObject);
        }

    }

    private new void OnCollisionEnter2D(Collision2D c) {
        if (c.gameObject.CompareTag("UI_EdgeOfPlatform") && gameObject.layer != LayerMask.NameToLayer("Toolbox")) {
            Reset();
            return;
        }

        base.OnCollisionEnter2D(c);
    }

    private void OnTriggerEnter2D(Collider2D c) {
        //Move the platform element out of the toolbox and onto the platform itself.
        if (c.CompareTag("ToolboxArea")) {
            RefreshPosition();
            transform.SetParent(c.gameObject.transform.root);
            transform.SetAsLastSibling();
            StartCoroutine(_WaitAndUpdatePosition());
        }
    }

    /// <summary>
    /// Updated the reset position of the platform element after it has been moved out of the toolbox onto the platform.
    /// </summary>
    /// <returns></returns>
    private IEnumerator _WaitAndUpdatePosition() {
        yield return new WaitWhile(() => Input.GetMouseButton(0));
        RefreshPosition();
        gameObject.layer = LayerMask.NameToLayer("UI");
        Debug.Log("Reset position updated.");
    }

}
