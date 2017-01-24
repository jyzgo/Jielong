using UnityEngine;
using System.Collections;

public class PileReset : CardAbstract {

	// Use this for initialization
	public override void Start () {
        base.Start();
	}
	
	// Update is called once per frame
	void OnMouseDown () {
        LevelMgr.current.RefreshPile();
	}



}
