using UnityEngine;
using System.Collections;

public class VegasState : GameState {
    public override int FromPileToTarget()
    {

        return 5;
    }

    public override int FromPlatToTarget()
    {
        return 5;
    }


    public override int FromPileToPlat()
    {
        return 0;
    }

    bool isCumulitive = false;
    public override void Init()
    {
        base.Init();
        _totalScore = -52;
        isCumulitive = SettingMgr.current.VegasCumulative == 1;
        if(isCumulitive)
        {
            _totalScore += SettingMgr.current.CumulitiveNum;
            SettingMgr.current.CumulitiveNum = _totalScore;
        }

        
    }


    public override int FromTarToPlat()
    {
        return 0;
    }

    public override int Reverse()
    {
        return 0;
    }

    public override int FlipPlatCard()
    {
        return 0;
    }

    public override int RefreshPile()
    {
        return 0;
    }

    public override int AddScore(int score)
    {
        _totalScore += score;
        if (isCumulitive)
        {
            SettingMgr.current.CumulitiveNum = _totalScore;
        }

        return score;
    }
    public override int GetScore()
    {
        return _totalScore;
    }
}
