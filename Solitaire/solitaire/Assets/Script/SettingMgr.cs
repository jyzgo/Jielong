using UnityEngine;
using System.Collections;

public enum PlayState
{
    Normal,
    Vegas,
}
public class SettingMgr : MonoBehaviour {

    public PlayState _state = PlayState.Normal;


}
