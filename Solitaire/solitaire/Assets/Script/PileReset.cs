﻿using UnityEngine;
using System.Collections;

public class PileReset : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnMouseDown()
    {
        Debug.Log("pile refresh");
        LevelMgr.current.RefreshPile();

    }
}