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

    CumulativeNum,
   
}




public class SettingMgr : MonoBehaviour {

    public static SettingMgr current;
    void Awake()
    {

        current = this;
        LoadFile();

    }


    void Start()
    {

        if (RightHanded != 1)
        {
            RightHandMgr.current.ResetPos();
        }


    }
    const string settingFileName = "setting.dt";
    public PlayState _state = PlayState.Normal;

    public int SoundControl = 1; //bool
    public int Hint = 0;// 0 1 2
    public int Draw3 = 0;//bool
    public int VegasCumulative = 0;//bool
    public int AllWinning = 0;//int 100
    public int Orientation = 0;//int 0竖 1横 2自动 

    public int CongratulationsScreen = 1;//bool
    public int TapMove = 1;//int 0 1 2 on off auto

    public int TimeMode = 0;//bool
    public int RightHanded = 1;//bool


    public int CumulitiveNum = 0;
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
            AllWinning = setJs.GetInt(SettingEnum.WinningDeals.ToString());
            Orientation = setJs.GetInt(SettingEnum.Orientation.ToString());

            CongratulationsScreen = setJs.GetInt(SettingEnum.CongratulationsScreen.ToString());
            TapMove = setJs.GetInt(SettingEnum.TapMove.ToString());

            TimeMode = setJs.GetInt(SettingEnum.Time_Moves.ToString());
            RightHanded = setJs.GetInt(SettingEnum.RightHanded.ToString());



            CumulitiveNum = setJs.GetInt(SettingEnum.CumulativeNum.ToString());
        }

        SetToggleState();

        AddToggleListener();


    }

    void AddToggleListener()
    {
        sound.onValueChanged.AddListener(OnsoundToggle);
        Draw3Tog.onValueChanged.AddListener(OnDraw3Toggle);
        allwinning.onValueChanged.AddListener(OnallwinningToggle);
        vegasmode.onValueChanged.AddListener(OnvegasmodeToggle);
        vegascumulative.onValueChanged.AddListener(OnvegascumulativeToggle);
        timermode.onValueChanged.AddListener(OntimermodeToggle);
        lefthanded.onValueChanged.AddListener(OnLefthandedToggle);
        autohint.onValueChanged.AddListener(OnautohintToggle);
      


    }

    void SetToggleState()
    {
        sound.isOn = SoundControl == 1;
        Draw3Tog.isOn = Draw3== 1;
        allwinning.isOn = AllWinning== 1;
        vegasmode.isOn = _state==PlayState.Vegas;
        vegascumulative.isOn = VegasCumulative== 1;
        timermode.isOn = TimeMode== 1;
        lefthanded.isOn = RightHanded== 0;
        autohint.isOn = Hint== 1;


        TimeOnlyForm.SetActive(TimeMode == 1);
        NormalForm.SetActive(TimeMode != 1);


    }

    void PlayToggleSound()
    {
        SoundManager.Current.Play_switch(0);
    }
    void OnsoundToggle(bool b)
    {
        //Debug.Log("OnsoundToggle" + b.ToString());
        PlayToggleSound(); 
        if (b)
        {
            SoundControl = 1;
        }else
        {
            SoundControl = 0;
        }
    }
    void OnDraw3Toggle(bool b)
    {
        PlayToggleSound();
        // Debug.Log("OnDraw3Toggle" + b.ToString());
        if (b)
        {
            Draw3 = 1;
        }
        else
        {
            Draw3 = 0;
        }
    }
    void OnallwinningToggle(bool b)
    {
        PlayToggleSound();
        // Debug.Log("OnallwinningToggle" + b.ToString());
        if (b)
        {
            AllWinning = 1;
        }
        else
        {
            AllWinning = 0;
        }
    }
    void OnvegasmodeToggle(bool b)
    {
        PlayToggleSound();
        //Debug.Log("OnvegasmodeToggle" + b.ToString());
        if (b)
        {
            _state = PlayState.Vegas;
        }else
        {
            _state = PlayState.Normal;

        }
    }
    void OnvegascumulativeToggle(bool b)
    {
        PlayToggleSound();
        // Debug.Log("OnvegascumulativeToggle" + b.ToString());
        if (b)
        {
            VegasCumulative = 1;
        }
        else
        {
            if (VegasCumulative == 1)
            {
                ShowVegasConfirmWindow();
               // gameObject.SetActive(false);
            }

            VegasCumulative = 0;
        }
    }
    void OntimermodeToggle(bool b)
    {
        PlayToggleSound();
        TimeOnlyForm.SetActive(b);
        NormalForm.SetActive(!b);
        //Debug.Log("OntimermodeToggle" + b.ToString());
        if (b)
        {
            TimeMode = 1;
           
        }
        else
        {
            TimeMode = 0;
        }
    }
    void OnLefthandedToggle(bool b)
    {
        PlayToggleSound();
        RightHandMgr.current.ResetPos();
        //Debug.Log("OnrighthandedToggle" + b.ToString());
        if (b)
        {
            RightHanded = 0;
        
        }
        else
        {
            RightHanded = 1;
        }
    }
    void OnautohintToggle(bool b)
    {
        PlayToggleSound();
        //Debug.Log("OnautohintToggle" + b.ToString());
        if (b)
        {
            Hint = 1;
        }
        else
        {
            Hint = 0;
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
        setJs.Set(SettingEnum.WinningDeals.ToString(), AllWinning);
        setJs.Set(SettingEnum.Orientation.ToString(), Orientation);

        setJs.Set(SettingEnum.CongratulationsScreen.ToString(), CongratulationsScreen);
        setJs.Set(SettingEnum.TapMove.ToString(), TapMove);

        setJs.Set(SettingEnum.Time_Moves.ToString(), TimeMode);
        setJs.Set(SettingEnum.RightHanded.ToString(), RightHanded);

        setJs.Set(SettingEnum.CumulativeNum.ToString(), CumulitiveNum);
        File.WriteAllText(GetPath(), setJs.ToString());
    }

    void OnApplicationQuit()
    {
        SaveToFile();
    }

    public void EraseVegasCumulitive()
    {
        CumulitiveNum = 0;
        HideVegasConfirmWindow();
    }

    public void SaveVegasCumulitive()
    {
        HideVegasConfirmWindow();
    }


    public GameObject VegasConfirmWindow;
    public void ShowVegasConfirmWindow()
    {
        VegasConfirmWindow.SetActive(true);
        SoundManager.Current.Play_ui_open(0);
    }

    public void HideVegasConfirmWindow()
    {
        if (VegasConfirmWindow.activeSelf)
        {
            SoundManager.Current.Play_ui_close(0);
        }
        VegasConfirmWindow.SetActive(false);

    }

    public GameObject NormalForm;
    public GameObject TimeOnlyForm;
    public GameObject HowToPlay;

    public void ShowHowToPlay()
    {
        if(HowToPlay.activeSelf != true)
        {
            SoundManager.Current.Play_ui_open(0);
        }
        HowToPlay.SetActive(true);
       
    }
    public void HideHowToPlay()
    {
        if (HowToPlay.activeSelf)
        {
            SoundManager.Current.Play_ui_close(0);
        }
        HowToPlay.SetActive(false);
    }
}
