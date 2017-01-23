using UnityEngine;
using System.Collections;

public class ToolbarMgr : MonoBehaviour {

    public static ToolbarMgr current;
    void Awake()
    {
        current = this;
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ShowPlayMenu()
    {
        Debug.Log("show play menu");
        PlayCanvas.SetActive(true);
    }

    public void HidePlayMenu()
    {
        PlayCanvas.SetActive(false);
    }

    public void ShowSettingMenu()
    {
        SettingMenu.SetActive(true);
        Debug.Log("showSettingMenu");
    }

    public void HideSettingMenu()
    {
        SettingMenu.SetActive(false);
        Debug.Log("hidesettingmenu");
    }

    public GameObject PlayCanvas;
    public GameObject SettingMenu;
}
