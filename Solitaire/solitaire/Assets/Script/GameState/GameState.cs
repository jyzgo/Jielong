using UnityEngine;
using System.Collections;

public class GameState
{

    protected int _totalScore = 0;
    protected int _refreshPileTimes = 0;
    public virtual int FromPileToTarget()
    {
        return 0;
    }

    public virtual int FromPlatToTarget()
    {
        return 0;
    }


    public virtual int FromPileToPlat()
    {
        return 0;
    }




    public virtual int FromTarToPlat()
    {
        return 0;
    }


    public virtual int FlipPlatCard()
    {
        return 0;
    }

    public virtual int Reverse()
    {
        return 0;
    }

    public virtual int RefreshPile()
    {
        return 0;
    }

    public virtual int AddScore(int score)
    {
        return score;
    }
    
    public virtual void Init()
    {
        GameTime = 0;
        Moves = 0;
    }

    public virtual int GetScore()
    {
        return _totalScore;
    }


    float GameTime = 0;
    public string GetTime()
    {
        if ((int)(GameTime / 60) > 0)
        {
            return (int)(GameTime / 60) + ":" + (int)(GameTime % 60);
        }else
        {
            return ((int)GameTime).ToString();
        }
    }

    public void Tick()
    {
        GameTime += 1;
        ReduceScoreByTime();
    }


    public virtual void ReduceScoreByTime()
    { }

    public int Moves = 0;



}