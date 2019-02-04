using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class buildNode : MonoBehaviour {

    public Color hoverColour;
    public GameObject buildUI;
    public static GameObject currentBuildUI;
    public GameObject canvas;
    private GameObject[] activeNodes;

    private Renderer rend;
    private Color startColour;

    private Vector3 mousePos;

    private void Start()
    {
        rend = GetComponent<Renderer>();
        startColour = rend.material.color;
        canvas =  GameObject.Find("Canvas");
    }

    void FixedUpdate() {
        mousePos = Input.mousePosition;
    }

    private void OnMouseEnter()
    {
        rend.material.color = hoverColour;
    }

    private void OnMouseExit()
    {
        rend.material.color = startColour;
    }

    private void OnMouseDown()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (currentBuildUI == null)
            {
                tag = "activeBuildNode";
                Debug.Log("boop");
            }
            if (currentBuildUI != null)
            {
                Destroy(currentBuildUI);
                activeNodes = GameObject.FindGameObjectsWithTag("activeBuildNode");
                activeNodes[0].tag = "buildNode";
                Debug.Log("boop2");
                return;
            }
            currentBuildUI = Instantiate(buildUI, mousePos, Quaternion.identity);
            currentBuildUI.transform.SetParent(canvas.transform, true);
            currentBuildUI.transform.position = mousePos;
            return;
        }
    }

}
