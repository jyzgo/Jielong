using UnityEngine;
using System.Collections;
using System.IO;
using MTUnity.Utils;
using UnityEngine.UI;

public enum PlayState
{
    Normal,
    Vegas,
}

public enum SettingEnum
{
    PlayState,
    
    SoundControl,
    Hint,
    Draw3,
    VegasCumulative,
    WinningDeals,
    Orientation,

    CongratulationsScreen,
    TapMove,

    Time_Moves,
    RightHanded,
   
}




public class SettingMgr : MonoBehaviour {
    const string settingFileName = "setting.dt";
    public PlayState _state = PlayState.Normal;

    public int SoundControl = 1; //bool
    public int Hint = 0;// 0 1 2
    public int Draw3 = 0;//bool
    public int VegasCumulative = 0;//bool
    public int WinningDeals = 0;//int 100
    public int Orientation = 0;//int 0竖 1横 2自动 

    public int CongratulationsScreen = 1;//bool
    public int TapMove = 1;//int 0 1 2 on off auto

    public int Time_Moves = 0;//bool
    public int RightHanded = 1;//bool
    public void LoadFile()
    {
        var filePath = GetPath();
        if (File.Exists(filePath))
        {
            LoadSetting();
        }
        else
        {
            SaveToFile();
        }
    }


    public Toggle sound;
    public Toggle Draw3Tog;
    public Toggle allwinning;
    public Toggle vegasmode;
    public Toggle vegascumulative;
    public Toggle timermode;
    public Toggle lefthanded;
    public Toggle autohint;
    public Toggle rules;


    void LoadSetting()
    {
        string content = File.ReadAllText(GetPath());
        MTJSONObject setJs = MTJSON.Deserialize(content);
        if(setJs == null)
        {
            SaveToFile();
        }else
        {
            _state = (PlayState)setJs.GetInt(SettingEnum.PlayState.ToString());
            SoundControl = setJs.GetInt(SettingEnum.SoundControl.ToString());
            Hint = setJs.GetInt(SettingEnum.Hint.ToString());
            Draw3 = setJs.GetInt(SettingEnum.Draw3.ToString());
            VegasCumulative = setJs.GetInt(SettingEnum.VegasCumulative.ToString());
            WinningDeals = setJs.GetInt(SettingEnum.WinningDeals.ToString());
            Orientation = setJs.GetInt(SettingEnum.Orientation.ToString());

            CongratulationsScreen = setJs.GetInt(SettingEnum.CongratulationsScreen.ToString());
            TapMove = setJs.GetInt(SettingEnum.TapMove.ToString());

            Time_Moves = setJs.GetInt(SettingEnum.Time_Moves.ToString());
            RightHanded = setJs.GetInt(SettingEnum.RightHanded.ToString());
        }


    }

    string GetPath()
    {
        return Application.persistentDataPath + "/" + settingFileName;
    }

    void SaveToFile()
    {
        MTJSONObject setJs = MTJSONObject.CreateDict();
        setJs.Set(SettingEnum.PlayState.ToString(), (int)_state);

        setJs.Set(SettingEnum.SoundControl.ToString(), SoundControl);
        setJs.Set(SettingEnum.Hint.ToString(), Hint);
        setJs.Set(SettingEnum.Draw3.ToString(), Draw3);
        setJs.Set(SettingEnum.VegasCumulative.ToString(), VegasCumulative);
        setJs.Set(SettingEnum.WinningDeals.ToString(), WinningDeals);
        setJs.Set(SettingEnum.Orientation.ToString(), Orientation);

        setJs.Set(SettingEnum.CongratulationsScreen.ToString(), CongratulationsScreen);
        setJs.Set(SettingEnum.TapMove.ToString(), TapMove);

        setJs.Set(SettingEnum.Time_Moves.ToString(), Time_Moves);
        setJs.Set(SettingEnum.RightHanded.ToString(), RightHanded);
        File.WriteAllText(GetPath(), setJs.ToString());
    }

    void OnApplicationQuit()
    {
        SaveToFile();
    }

}
