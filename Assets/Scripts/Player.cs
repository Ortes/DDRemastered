using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : SelectableObject {

    public Vector2 coordinates;


    public Material Blue;
    public Material Green;

    private MeshRenderer selectedRend;

	// Use this for initialization
	void Start () {
        selectedRend = GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void Select()
    {
        selectedRend.material = Green;
    }
    public override void Deselect()
    {
        selectedRend.material = Blue;
    }
}
