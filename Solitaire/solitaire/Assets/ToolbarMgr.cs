using UnityEngine;
using System.Collections;

public class ToolbarMgr : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ShowPlayMenu()
    {
        Debug.Log("show play menu");
    }

    public void HidePlayMenu()
    {
        Debug.Log("hide playMenu");
    }

    public void ShowSettingMenu()
    {
        Debug.Log("showSettingMenu");
    }

    public void HideSettingMenu()
    {
        Debug.Log("hidesettingmenu");
    }
}
