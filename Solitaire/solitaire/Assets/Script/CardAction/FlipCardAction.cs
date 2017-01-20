using UnityEngine;
using System.Collections;

public class FlipCardAction : CardAction
{

    CardAbstract _flipCard;
    Vector3 _originalCardPos;
    public void Init(GameObject card)
    {
        _flipCard = card.GetComponent<CardAbstract>();
        _originalCardPos = _flipCard.originalPos;

    }

    public override void DoAction()
    {
        var pileList = LevelMgr.current._pileList;
        var pileReadyList = LevelMgr.current._pileReadyList;
        pileList.Remove(_flipCard.gameObject);
        pileReadyList.Add(_flipCard.gameObject);
        LevelMgr.current.RefreshPileReady();

    }

    public override void ReverseAction()
    {
        var pileList = LevelMgr.current._pileList;
        var pileReadyList = LevelMgr.current._pileReadyList;
        pileList.Add(_flipCard.gameObject);
        pileReadyList.Remove(_flipCard.gameObject);
        LevelMgr.current.RefreshPileReady();
        _flipCard.transform.position = LevelMgr.current.GetPileCardPos();
        _flipCard.transform.eulerAngles = new Vector3(0, 180, 0);
        _flipCard.originalPos = LevelMgr.current.GetPileCardPos(); 
    }
}
