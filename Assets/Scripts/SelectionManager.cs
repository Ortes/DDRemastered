using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour {

    SelectableObject selected;
    public LayerMask clickablesLayer;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButton(0))
        {
            RaycastHit rayHit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out rayHit, Mathf.Infinity, clickablesLayer))
            {
                selected = rayHit.collider.GetComponent<SelectableObject>();
                selected.Select();
            }
            else
            {
                selected.Deselect();
            }
        }
    }
}
