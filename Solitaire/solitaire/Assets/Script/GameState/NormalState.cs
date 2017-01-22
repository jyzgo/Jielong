using UnityEngine;
using System.Collections;

public class NormalState : GameState {

    public override int FromPileToTarget()
    {
        return 15;
    }

    public override int FromPlatToTarget()
    {
        return 10;
    }


    public override void Init()
    {
        base.Init();
    }


    public override int FromPileToPlat()
    {
        return 5;
    }


    public override int FromTarToPlat()
    {
        return -15;
    }

    public override int Reverse()
    {
        return -2;
    }

    public override int FlipPlatCard()
    {
        return 5;
    }

    public override int RefreshPile()
    {
        //
        return 0;
    }

    public override int AddScore(int score)
    {
        if(_totalScore + score < 0)
        {
            int t = _totalScore;
            _totalScore = 0;
            return t;
        }

        _totalScore += score;
        return score;

    }

    public override int GetScore()
    {

        return _totalScore;
    }

    const int ReduceInterval = 10;
    int current = 0;
    public override void ReduceScoreByTime()
    {
        current += 1;
        if(current == ReduceInterval)
        {
            current = 0;
            AddScore(-2);
            LevelMgr.current.UpdateUI();
        }
    }
}
