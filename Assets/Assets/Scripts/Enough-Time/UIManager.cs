using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    private Vector3 offset;
    private GameObject currentNode;
    public GameObject standardTurret, bombTurret;
    public Button standardTurretButton, destoryTurretButton, bombTurretButton;
    private GameObject currentTurret;

	// Use this for initialization
	void Start () {

        offset = new Vector3 (1.5f,2,1.5f);

		standardTurretButton.onClick.AddListener(placeStandardTurret);

        destoryTurretButton.onClick.AddListener(destoryTurret);

        bombTurretButton.onClick.AddListener(placeBombTurret);

	}

    void placeStandardTurret(){

    if (turretManager.Instance.canBuild == true){
        if (turretManager.Instance.sTurretAmount > 0){
            currentNode = GameObject.FindGameObjectWithTag("activeBuildNode");
            if (currentNode.transform.childCount == 0)
            {
                currentTurret = Instantiate(standardTurret, currentNode.transform.position + offset, standardTurret.transform.rotation);
                currentTurret.transform.SetParent(currentNode.transform);
                turretManager.Instance.sTurretAmount -= 1;
            }
        }
        return;

    }
    }

    void placeBombTurret()
    {
    if (turretManager.Instance.canBuild == true){
        if (turretManager.Instance.bTurretAmount > 0)
        {
            currentNode = GameObject.FindGameObjectWithTag("activeBuildNode");
            if (currentNode.transform.childCount == 0)
            {
                currentTurret = Instantiate(bombTurret, currentNode.transform.position + offset, bombTurret.transform.rotation);
                currentTurret.transform.SetParent(currentNode.transform);
                turretManager.Instance.bTurretAmount -= 1;
            }
        }
        return;
    }

    }

    void destoryTurret(){
        currentNode = GameObject.FindGameObjectWithTag("activeBuildNode");
        var children = new List<GameObject>();
        foreach (Transform child in currentNode.transform) children.Add(child.gameObject);
        if (children.Count != 0 && children[0].name.Contains("StandardTurret"))
        {
            turretManager.Instance.sTurretAmount += 1;
        }
        if (children.Count != 0 && children[0].name.Contains("BombTurret"))
        {
            turretManager.Instance.bTurretAmount += 1;
        }
        children.ForEach(child => Destroy(child));
        return;
    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }
}
