using UnityEngine;
using System.Collections;

public class ToolbarMgr : MonoBehaviour {

    public static ToolbarMgr current;
    void Awake()
    {
        current = this;
    }

    public void ShowPlayMenu()
    {
        SoundManager.Current.Play_ui_open(0);
        //Debug.Log("show play menu");
        PlayCanvas.SetActive(true);
    }

    public void HidePlayMenu()
    {
        if (PlayCanvas.activeSelf)
        {
            SoundManager.Current.Play_ui_close(0);
        }
        PlayCanvas.SetActive(false);
       
    }

    public void ShowSettingMenu()
    {
        SoundManager.Current.Play_ui_open(0);
        SettingMenu.SetActive(true);
        //Debug.Log("showSettingMenu");
    }

    public void HideSettingMenu()
    {
        if (SettingMenu.activeSelf)
        {
            SoundManager.Current.Play_ui_close(0);
        }
        SettingMenu.SetActive(false);

        //  Debug.Log("hidesettingmenu");
    }

    public GameObject PlayCanvas;
    public GameObject SettingMenu;
}
