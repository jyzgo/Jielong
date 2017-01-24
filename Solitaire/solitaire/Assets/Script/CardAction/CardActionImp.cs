using UnityEngine;
using System.Collections;
using System;
using MTUnity.Actions;

public class CardActionImp : CardAction {

    CardAbstract _mainCard;
    CardAbstract _otherCard;
    Vector3 _originalPos;
    CardAbstract _otherOriginalPreCard;
    Transform _originalTrans;

    int _cardIndexInPile = -1;
    int _cardIndexInReady = -1;

    CardState _otherOriginalCardState = CardState.None;
    public void Init(CardAbstract card,CardAbstract bePutCard)
    {
        _mainCard = card;
        _otherCard = bePutCard;
        _originalPos = _otherCard.originalPos;
        _originalTrans = _otherCard.transform.parent;
        _otherOriginalPreCard = _otherCard.preCard;
        _otherOriginalCardState = _otherCard.cardState;

        //if(_otherCard.cardState == CardState.InPile)
        //{
        //    var pileReadyList = LevelMgr.current._pileReadyList;
        //    _cardIndexInReady = pileReadyList.IndexOf(_otherCard.gameObject);
            
        //}
    }
    public override void DoAction()
    {

        var gameState = LevelMgr.current._gameState;
        UpdateGameState(_mainCard, _otherCard);
        if (_otherCard.cardState == CardState.InPile)
        {
            LevelMgr.current.RemoveFromPile(_otherCard.gameObject);
            LevelMgr.current.RefreshPileReady();

        }

        if (_mainCard.cardState == CardState.InTarget)
        {
            _otherCard.nextCard = null;
        }
        _otherCard.cardState = _mainCard.cardState;
        _mainCard.nextCard = _otherCard;

        _otherCard.transform.parent = _mainCard.transform;
        if(_otherCard.DetachCardFromOriginal())
        {
            gameState.AddScore(gameState.FlipPlatCard());
        }

        _otherCard.preCard = _mainCard;

        _otherCard.transform.eulerAngles = Vector3.zero;
        _otherCard.gameObject.StopAllActions();
        _otherCard.RunActions(
            new MTMoveToWorld(0.1f,_mainCard.GetNextPos()),
            new MTCallFunc(()=> _otherCard.transform.eulerAngles = Vector3.zero));
       
        if(LevelMgr.current.CheckSuccess())
        {
            LevelMgr.current.ToWinState();
        }

      

        
    }

    public override void ReverseAction()
    {

        var gameState = LevelMgr.current._gameState;
        _mainCard.nextCard = null;
        _otherCard.preCard = _otherOriginalPreCard;
       // _otherCard.transform.parent = _originalTrans;
        _otherCard.cardState = _otherOriginalCardState;
        _otherCard.RunAction(new MTMoveToWorld(0.1f, _originalPos));    
        
        if(_otherOriginalCardState == CardState.InPlatform)
        {
            if(_otherOriginalPreCard.preCard != null)
            {
                gameState.AddScore(gameState.FlipPlatCard() * -1);
                _otherOriginalPreCard.gameObject.RunActions(
                    new MTRotateTo(0.2f, new Vector3(0, 180, 0)),
                    new MTCallFunc(()=> _otherOriginalPreCard.transform.eulerAngles = new Vector3(0,180,0)));
            }
        }

        LevelMgr.current.RunActions(new MTDelayTime(0.2f), new MTCallFunc(() => _otherCard.transform.parent = _originalTrans));

        if(_otherCard.cardState == CardState.InPile)
        {
            LevelMgr.current._pileReadyList.Add(_otherCard.gameObject);
            LevelMgr.current.RefreshPileReady();
        }


        SoundManager.Current.Play_put_success(0);

        gameState.AddScore(addScoreFrom *-1);
        gameState.AddScore(gameState.Reverse());
        LevelMgr.current.UpdateUI();

    }

    int addScoreFrom = 0;
    public void UpdateGameState(CardAbstract mainCard,CardAbstract othCard)
    {

        int addScore = 0;
        var gameState = LevelMgr.current._gameState;

        if (othCard.cardState == CardState.InPile && mainCard.cardState == CardState.InTarget)
        {

            addScore = gameState.FromPileToTarget();
        }
        else if (othCard.cardState == CardState.InPlatform && mainCard.cardState == CardState.InTarget)
        {
            addScore = gameState.FromPlatToTarget();
        }
        else if (othCard.cardState == CardState.InPile && mainCard.cardState == CardState.InPlatform)
        {
            addScore = gameState.FromPileToPlat();
        }
        else if (othCard.cardState == CardState.InTarget && mainCard.cardState == CardState.InPlatform)
        {
            addScore = gameState.FromTarToPlat();
        }


        addScoreFrom = gameState.AddScore(addScore);
        LevelMgr.current.UpdateUI();
    }


}
