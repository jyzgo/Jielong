using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MTUnity.Actions;
using MonsterLove.StateMachine;
using UnityEngine.UI;
using System;
using DG.Tweening;
using MTUnity.Utils;

[Serializable]
public struct CardStruct
{
    public Sprite Icon;
    public Sprite Center;

}


[Serializable]
public struct CardData
{
   public CardColor cardColor;

    [Range(1, 13)]
    public int CardNum;
}

public class LevelMgr : MonoBehaviour {

	public enum LevelState
	{
		Playing,
		Win
		
	}

    public GameObject MenuCanvas;
    public GameObject WinCanvas;


    Lang _langSet;

    List<CardAction> _CardActions = new List<CardAction>();

    public GameObject CardPrefab;

    public Transform start;

    public Text Score;
    public Text UseTime;
    public Text UseTime2;
    public Text Moves;

    public GameState _gameState;

  
    public void UpdateUI()
    {
        
        Score.text = _langSet.GetLang(LangEnum.Score) +":" + _gameState.GetScore().ToString();
        UpdateMoves();

    }


    public void ShowHint()
    {
        Debug.Log("Hint");
    }

    public void UpdateMoves()
    {
        Moves.text = _langSet.GetLang(LangEnum.Moves) + ":" + _gameState.Moves.ToString();
    }

    public void UpdateTime()
    {
        UseTime.text = _langSet.GetLang(LangEnum.Time) + ":" + _gameState.GetTime();
    }


    IEnumerator TimeTick()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);
            _gameState.Tick();
            UpdateTime();

        }
    }


	StateMachine<LevelState> fsm;

    public static LevelMgr current;

    public CardStruct[] HeartSet;
    public CardStruct[] SpadeSet;
    public CardStruct[] DiamondSet;
    public CardStruct[] ClubSet;

    public Transform[] TargetPlace;
    public Transform[] PlatformPlace;
    public Transform Pile;

    public Transform[] PileReadyTrans;
    public List<GameObject> _pileReadyList = new List<GameObject>();


    public CardStruct GetCard(CardColor co,int CardNum)
    {
        if(CardNum >= 10)
        {
            CardNum -= 10;
        }else
        {
            CardNum = 0;
        }
        if (co == CardColor.Heart)
        {
            return HeartSet[CardNum];
        }
        else if (co == CardColor.Spade)
        {
            return SpadeSet[CardNum];
        }
        else if (co == CardColor.Dimaond)
        {
            return DiamondSet[CardNum];
        }
        else if (co == CardColor.Club)
        {
            return ClubSet[CardNum];
        }
        Debug.Assert(true,"Assert");
        return ClubSet[0];
        
    }

    SettingMgr _setting;
    IEnumerator _timeTick;
    void Awake()
	{
        _langSet = new Lang();
        _langSet.Init();
        _setting = GetComponent<SettingMgr>();
        _setting.LoadFile();
        if(_setting._state == PlayState.Normal)
        {
            _gameState = new NormalState();
        }else
        {
            _gameState = new VegasState();
        }
        _gameState.Init();

        UpdateUI();
        UpdateMoves();
        _timeTick = TimeTick();
        StartCoroutine(_timeTick);


        current = this;
        GenCard();
        CleanCard();
        InitCardPlatform();
        fsm = StateMachine<LevelState>.Initialize(this, LevelState.Playing);
    }

    

    List<GameObject> CardList = new List<GameObject>();
    List<CardAbstract> _platformList = new List<CardAbstract>();
    List<CardAbstract> _targetList = new List<CardAbstract>();

    void GenCard()
    {
        if (CardList.Count > 0)
        {
            return;
        }

        foreach (CardColor _cardColr in Enum.GetValues(typeof(CardColor)))
        {
            for (int i = 0; i < 13; i++)
            {
                var gb = Instantiate<GameObject>(LevelMgr.current.CardPrefab);
                var cardSc = gb.GetComponent<Card>();
                cardSc.cardColor = _cardColr;
                cardSc.CardNum = i + 1;
                gb.name = "card " +  _cardColr.ToString() + (i + 1).ToString();
                cardSc.UpdateCardView();
                gb.transform.position = new Vector3(i * 0.5f, (int)_cardColr, i * -0.5f);
                CardList.Add(gb);
            }
        }

        for(int i =0; i < PlatformPlace.Length; i ++)
        {
            var cardAb = PlatformPlace[i].GetComponent<CardAbstract>();
            _platformList.Add(cardAb);
        }

        for(int i = 0; i  < TargetPlace.Length; i ++)
        {
            var cardAb = TargetPlace[i].GetComponent<CardAbstract>();
            _targetList.Add(cardAb);
        }
       

    }
    void CleanCard()
    {
        var en = CardList.GetEnumerator();
        while(en.MoveNext())
        {
            var curCard = en.Current;
            curCard.transform.position = new Vector3(0, -10f, 0);
            var curRotation = curCard.transform.eulerAngles;
            curCard.transform.eulerAngles = new Vector3(curRotation.x, 180, curRotation.y);
            
        }
    }





    List<GameObject>[] CardPlatform = new List<GameObject>[7];
    List<CardAbstract> PlatformCardList = new List<CardAbstract>();
    void InitCardPlatform()
    {
        for(int i  = 0; i  < CardPlatform.Length; i ++)
        {
            CardPlatform[i] = new List<GameObject>();
        }

        for(int i = 0; i < PlatformPlace.Length;i ++)
        {
            var card = PlatformPlace[i].GetComponent<CardAbstract>();
            PlatformCardList.Add(card);
        }
    }

    void CleanCardPlatform()
    {
        for (int i = 0; i < CardPlatform.Length; i++)
        {
            CardPlatform[i].Clear();
        }

    }


    public CardAbstract FindTheBestCard(CardAbstract card)
    {
        for(int i  = 0; i < _targetList.Count; i  ++)
        {
            var curCard = _targetList[i].GetTopCard() ;
            if(curCard.isCardPutable(card))
            {
                return curCard;
            }

        }

        for(int i = 0; i < _platformList.Count;i ++)
        {
            var curCard = _platformList[i].GetTopCard();
            if (curCard.isCardPutable(card))
            {
                return curCard;
            }

        }


        return null;
    }

    float lastNewGameTime = -10f;
    public void NewGame()
    {
        if (lastNewGameTime + 2 > Time.time)
        {
            return;
        }
        lastNewGameTime = Time.time;
        CardList.RandomShuffle<GameObject>();
        ResetGame();

    }

    void CleanBoard()
    {
        _pileReadyList.Clear();
        _pileList.Clear();
        _CardActions.Clear();

        foreach (var card in _targetList)
        {
            card.nextCard = null;
        }

        foreach (var card in _platformList)
        {
            card.nextCard = null;
        }


        CleanCardPlatform();
        foreach (var curCard in CardList)
        {
            var curCardSc = curCard.GetComponent<CardAbstract>();
            curCardSc.cardState = CardState.None;
            curCardSc.preCard = null;
            curCardSc.nextCard = null;
            curCard.transform.position = Vector3.zero;
            curCard.transform.parent = null;
        }

        foreach (var platform in PlatformCardList)
        {
            platform.nextCard = null;
        }

        for (int i = 0; i < CardList.Count; i++)
        {
            var curCard = CardList[i];

            curCard.transform.eulerAngles = new Vector3(0, 180f, 0);
            curCard.transform.position = start.position;
        }
    }

    float lastResetGame = -10f;
    public void ResetGame()
    {
        if (lastResetGame + 2 > Time.time)
        {
            return;
        }
        lastResetGame = Time.time;
        CleanScore();
        CleanBoard();
        int index = 0;
        for(int i = 0; i < 7; i ++)
        {
            for(int j = 0; j <=i; j++ )
            {
                CardPlatform[i].Add(CardList[index]);
                index++;
            } 
        }
        PlayDealCard();
        StartCoroutine(SetPile(index));
    }

    void CleanScore()
    {

    }

    public void LoadGame(MTJSONObject js)
    {

        int index = 0;
        for(int i= 0; i < 7; i ++)
        {
            var curList = js.Get(i.ToString()).list;

            for(int j = 0; j < curList.Count;j ++)
            {
                var curCardJs = curList[j];
                CardColor curColor = (CardColor)curCardJs.GetInt("CardColor");
                int curNum = curCardJs.GetInt("CardNum");
                var curCard = CardList[index].GetComponent<Card>();
                curCard.cardColor = curColor;
                curCard.CardNum = curNum;
                curCard.UpdateCardView();
                index++;
            }
            
        }

        var pileList = js.Get("7").list;
        for(int i = 0; i  < pileList.Count; i++)
        {
            var curCardJs = pileList[i];
            CardColor curColor = (CardColor)curCardJs.GetInt("CardColor");
            int curNum = curCardJs.GetInt("CardNum");
            var curCard = CardList[index].GetComponent<Card>();

            curCard.cardColor = curColor;
            curCard.CardNum = curNum;
            curCard.UpdateCardView();
            index++;
        }


        ResetGame();
    }


    void PlayDealCard()
    {
        for (int i = 0; i < CardPlatform.Length; i++)
        {
            var curCardList = CardPlatform[i];
            for (int j = 0; j < curCardList.Count; j++)
            {
                CardAbstract root = PlatformPlace[i].GetComponent<CardAbstract>();
                var curCard = curCardList[j];
                var tarPos = root.GetNextPos(j + 1);

                MTSequence seq = null;

                if (j == curCardList.Count - 1)
                {
                    seq = new MTSequence(
                        new MTDelayTime(1f),
                        new MTMoveToWorld(i * 0.05f + j * 0.04f, tarPos),
                        new MTRotateTo(0.2f, new Vector3(0, 0, 0)),
                        new MTDelayTime(0.1f),
                        new MTCallFunc(() => curCard.transform.eulerAngles = Vector3.zero));
                }
                else
                {
                    seq = new MTSequence(new MTDelayTime(1f), new MTMoveToWorld(i * 0.05f + j * 0.04f, tarPos));
                }
                CardAbstract preCard = root;
                if (j != 0)
                {
                    preCard = curCardList[j - 1].GetComponent<CardAbstract>();

                }

                var cardSc = curCard.GetComponent<CardAbstract>();
                cardSc.cardState = CardState.InPlatform;
                cardSc.preCard = preCard;
                preCard.nextCard = cardSc;
                curCard.RunActions(seq);
            }
        }
    }

    public void AddAction(CardAction action)
    {
        _CardActions.Add(action);
    }

    public void ReverseAction()
    {
        if(_CardActions.Count > 0)
        {
            var lastAction = _CardActions[_CardActions.Count - 1];
            _CardActions.Remove(lastAction);
            lastAction.ReverseAction();
        }

    }

    public void RefreshCardView()
    {
        foreach (var card in CardList)
        {
            var cardSc = card.GetComponent<Card>();
            if (cardSc != null)
            {
                cardSc.UpdateCardView();
            }
        }
    }

    IEnumerator SetPile(int index)
    {
        yield return new WaitForSeconds(2f);
        for (int i = index; i < CardList.Count; i++)
        {
            var curCard = CardList[i];
            _pileList.Add(curCard);

            curCard.transform.position = GetPileCardPos();
            curCard.transform.parent = Pile.transform;
            var cardSc = curCard.GetComponent<CardAbstract>();
            cardSc.cardState = CardState.InPile;
        }

    }

    public bool CheckSuccess()
    {
        if (_pileList.Count != 0 || _pileReadyList.Count != 0)
        {
            return false;
        }

        for (int i = 0; i < _platformList.Count; i++)
        {
            if(_platformList[i].GetTopCard() != _platformList[i])
            {
                return false;
            }
        }

        return true;

    }

    public bool isAutoFinish = true;

    public bool CheckFinish()
    {
        if(_pileList.Count != 0 || _pileReadyList.Count != 0)
        {
            return false;
        }

        for(int i  = 0; i  < _platformList.Count;i++)
        {

        }

        return true;
    }


    public Vector3 GetPileCardPos()
    {

        return Pile.transform.position + Vector3.back * 0.1f;
    }

    public int flipNum = 3;
    public void FlipPile()
    {
        var flipCardList = new List<GameObject>();
        
        for(int i = _pileList.Count - 1, count = flipNum; i >= 0 && count > 0; i--,count--)
        {
            flipCardList.Add(_pileList[i]);
        }

        if(flipCardList.Count> 0 )
        {
            var lastCard = _pileList[_pileList.Count - 1];
            var flipCardAction = new FlipCardAction();
            flipCardAction.Init(flipCardList);
            flipCardAction.DoAction();
            AddAction(flipCardAction);
        }


    }

    public void RefreshPileReady()
    {
        for(int i  = _pileReadyList.Count-1,j = 2; i >= 0 ; i --,j--)
        {
            var curPileCard = _pileReadyList[i].GetComponent<CardAbstract>();
            if (j >= 0)
            {
                var curPileTrans = PileReadyTrans[j];
               
                curPileCard.transform.parent = curPileTrans;
                curPileCard.transform.localRotation = Quaternion.identity;
                curPileCard.BlockTouch(0.15f);

                curPileCard.StopAllActions();
                curPileCard.RunAction(new MTMoveTo(0.1f, Vector3.back * 0.1f));
            }else
            {
                curPileCard.transform.localPosition = new Vector3(0, 0, 50-i);
                //curPileCard.transform.eulerAngles = new Vector3(0, 180f, 0);
            }
            
        }
    }


    public void RefreshPile()
    {

        var refreshPile = new RefreshPileAction();
        refreshPile.DoAction();
        _CardActions.Add(refreshPile);

    }


    public void RemoveFromPile(GameObject card)
    {
        
        _pileList.Remove(card);
        _pileReadyList.Remove(card);

        
    }

    public int pileIndex = 0;
    
    public List<GameObject> _pileList = new List<GameObject>();




    public void ToPlayState()
    {
        fsm.ChangeState(LevelState.Playing);
    }

    public void ToWinState()
    {
        fsm.ChangeState(LevelState.Win);
    }





	void Playing_Enter()
	{

        MenuCanvas.SetActive(true);
        WinCanvas.SetActive(false);
        NewGame();

    }


    void Playing_Update()
    {


        
    }

    const int MAX_NUM = 50;
    const float Delay_Time = 0.6f;


    void Playing_Exit()
    {
    }



	void Win_Enter()
	{
        MenuCanvas.SetActive(false);
        WinCanvas.SetActive(true);
       // yield return null;

	
	}

 
}
