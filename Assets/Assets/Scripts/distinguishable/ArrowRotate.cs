using UnityEngine;
using System.Linq;

public class ArrowRotate : MonoBehaviour {
    public GameObject player;
    private Vector3[] v = new[] { Vector3.left, Vector3.zero, Vector3.back, Vector3.down, Vector3.forward, Vector3.one, Vector3.right, Vector3.up, };
    private Vector3 dir = Vector3.positiveInfinity;

    private void Update () {
        if (!gameObject.activeSelf)
            return;

        if (Input.GetKeyDown(KeyCode.KeypadPlus)) {
            if (dir == Vector3.positiveInfinity) {
                dir = v[0];
                return; ;
            }
            dir = v[v.ToList().IndexOf(dir)+1];
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log(string.Format("Current dir is {0}", dir.ToString()));
        }


            transform.LookAt(player.transform, dir);	
	}
}
