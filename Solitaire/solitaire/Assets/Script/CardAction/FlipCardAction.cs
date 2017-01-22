using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlipCardAction : CardAction
{

    List<CardAbstract> _flipCardList = new List<CardAbstract>();
    List<Vector3> _oldCardPos = new List<Vector3>();
    List<Vector3> _originalCardPosList = new List<Vector3>();
    List<Vector3> _originlaCardEulerList = new List<Vector3>();
    public void Init(List<GameObject> flipList)
    {

        for(int i  = 0; i < flipList.Count;i ++)
        {
            var c = flipList[i].GetComponent<CardAbstract>();
            _flipCardList.Add(c);
            _oldCardPos.Add(c.transform.position);
            _originalCardPosList.Add(c.originalPos);
            _originlaCardEulerList.Add(c.transform.eulerAngles);
        }


    }

    public override void DoAction()
    {
        var pileList = LevelMgr.current._pileList;
        var pileReadyList = LevelMgr.current._pileReadyList;
        for(int i = 0; i <_flipCardList.Count; i ++ )
        {
            var flipCard = _flipCardList[i];
            pileList.Remove(flipCard.gameObject);
            pileReadyList.Add(flipCard.gameObject);
        }

        LevelMgr.current.RefreshPileReady();

    }

    public override void ReverseAction()
    {
        var gameState = LevelMgr.current._gameState;
        var pileList = LevelMgr.current._pileList;
        var pileReadyList = LevelMgr.current._pileReadyList;
        for(int i = _flipCardList.Count -1; i >= 0; i--)
        {
            var flipCard = _flipCardList[i].GetComponent<CardAbstract>();
            pileList.Add(flipCard.gameObject);
            pileReadyList.Remove(flipCard.gameObject);
            flipCard.transform.position = _oldCardPos[i];
            flipCard.transform.eulerAngles = _originlaCardEulerList[i];
            flipCard.originalPos = _originalCardPosList[i];
        }

        gameState.AddScore(gameState.Reverse());

        LevelMgr.current.RefreshPileReady();

    }
}
