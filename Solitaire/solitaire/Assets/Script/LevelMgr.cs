using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MTUnity.Actions;
using MonsterLove.StateMachine;
using UnityEngine.UI;
using System;
using DG.Tweening;

[Serializable]
public struct CardStruct
{
    public Sprite Icon;
    public Sprite Center;

}

public class LevelMgr : MonoBehaviour {

	public enum LevelState
	{
		Default,
		Playing,
		Win,
		Lose
	}

    public GameObject CardPrefab;

    public Transform start;

    public Text text;


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
    public List<GameObject> PileReadyList = new List<GameObject>();


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


    void Awake()
	{

        current = this;
        GenCard();
        CleanCard();
        InitCardPlatform();
        fsm = StateMachine<LevelState>.Initialize(this, LevelState.Default);

        

    }

    List<GameObject> CardList = new List<GameObject>();

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
    void InitCardPlatform()
    {
        for(int i  = 0; i  < CardPlatform.Length; i ++)
        {
            CardPlatform[i] = new List<GameObject>();
        }
    }

    void CleanCardPlatform()
    {
        for (int i = 0; i < CardPlatform.Length; i++)
        {
            CardPlatform[i].Clear();
        }

    }


    
    void ResetGame()
    {
        CardList.RandomShuffle<GameObject>();
        PileReadyList.Clear();

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

        foreach(var platform in PlatformPlace)
        {
            var cardSc = platform.GetComponent<CardAbstract>();
            cardSc.nextCard = null;
        }
        
        for(int i = 0; i <CardList.Count; i ++)
        {
            var curCard = CardList[i];

            curCard.transform.eulerAngles = new Vector3(0, 180f, 0);
            curCard.transform.position = start.position;
        }

        int index = 0;
        for(int i = 0; i < 7; i ++)
        {
            for(int j = 0; j <=i; j++ )
            {

                CardPlatform[i].Add(CardList[index]);
                
                index++;
            } 
        }

        

        for(int i  = 0; i <CardPlatform.Length;i ++)
        {
            var curCardList = CardPlatform[i];
            for(int j = 0; j < curCardList.Count;j++)
            {
                CardAbstract root = PlatformPlace[i].GetComponent<CardAbstract>();
                var curCard = curCardList[j];
                var tarPos = root.GetNextPos(j + 1);

                MTSequence seq = null;

                if(j == curCardList.Count - 1)
                {
                    seq = new MTSequence(
                        new MTDelayTime(1f), 
                        new MTMoveToWorld(i * 0.05f + j * 0.04f , tarPos), 
                        new MTRotateTo(0.2f , new Vector3(0, 0, 0)),
                        new MTDelayTime(0.1f),
                        new MTCallFunc(()=> curCard.transform.eulerAngles = Vector3.zero));
                }else
                {
                    seq = new MTSequence( new MTDelayTime(1f),new MTMoveToWorld(i * 0.05f + j * 0.04f, tarPos));
                }
                CardAbstract preCard = root;
                if (j != 0)
                {
                   preCard =  curCardList[j - 1].GetComponent<CardAbstract>();
                    
                }

                var cardSc = curCard.GetComponent<CardAbstract>();
                //preCard.PutCard(cardSc,false);
                cardSc.cardState = CardState.InPlatform;
                cardSc.preCard = preCard;
                preCard.nextCard = cardSc;
                curCard.RunActions(seq);
            }
        }


        StartCoroutine(SetPile(index));

    }


    void OnGUI()
    {
        if (GUILayout.Button("Restart"))
        {
            ResetGame();
        }

        if (GUILayout.Button("Refresh View"))
        {
            foreach (var card in CardList)
            {
                var cardSc = card.GetComponent<Card>();
                if(cardSc != null)
                {
                    cardSc.UpdateCardView();
                }
            }
        }
    }

    IEnumerator SetPile(int index)
    {
        yield return new WaitForSeconds(2f);
        for (int i = index; i < CardList.Count; i++)
        {
            var curCard = CardList[i];
            pileList.Add(curCard);

            curCard.transform.position = Pile.transform.position + Vector3.back * 0.1f; 
            curCard.transform.parent = Pile.transform;
            var cardSc = curCard.GetComponent<CardAbstract>();
            cardSc.cardState = CardState.InPile;
        }

    }

    public void FlipPile()
    {
        if (pileList.Count > 0)
        {
            var lastCard = pileList[pileList.Count - 1];
            pileList.Remove(lastCard);
            PileReadyList.Add(lastCard);
            RefreshPileReady();
        }
    }

    public void RefreshPileReady()
    {
        for(int i  = PileReadyList.Count-1,j = 2; i >= 0 ; i --,j--)
        {
            var curPileCard = PileReadyList[i].GetComponent<CardAbstract>();
            if (j >= 0)
            {
                var curPileTrans = PileReadyTrans[j];
               
                curPileCard.transform.parent = curPileTrans;
                curPileCard.transform.localRotation = Quaternion.identity;

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
        pileList.Clear();
        for(int i = PileReadyList.Count-1; i  >=0; i --)
        {
            var curCard = PileReadyList[i];
            pileList.Add(curCard);
            curCard.transform.position = Pile.transform.position + Vector3.back * 0.1f;
            curCard.transform.parent = Pile.transform;
            curCard.transform.eulerAngles = new Vector3(0, 180, 0);
        }
        PileReadyList.Clear();
    }


    public void RemoveFromPile(GameObject card)
    {
        pileList.Remove(card);
        PileReadyList.Remove(card);

        
    }

    public int pileIndex = 0;
    
    List<GameObject> pileList = new List<GameObject>();


	void Default_Enter()
	{

        text.text = "Default";
        ResetGame();

    }
	
	// Update is called once per frame
	void Default_Update () {




    }

    public void ToPlayState()
    {
        fsm.ChangeState(LevelState.Playing);
    }

    public void ToWinState()
    {
        fsm.ChangeState(LevelState.Win);
    }

    public void ToLoseState()
    {
        
        fsm.ChangeState(LevelState.Lose);
        
    }




	void Playing_Enter()
	{



    }


    void Playing_Update()
    {


        
    }

    const int MAX_NUM = 50;
    const float Delay_Time = 0.6f;


    void Playing_Exit()
    {
    }



	IEnumerator Win_Enter()
	{
        text.text = "You win";
        yield return null;

	
	}



	void Win_Update () {


	}

    const float END_FADE_TIME = 3f;

	IEnumerator Lose_Enter()
	{
        text.text = "You lose";
        yield return new WaitForSeconds(END_FADE_TIME);
        fsm.ChangeState(LevelState.Default);


    }

	void Lose_Update () {

        

	}

    float FADE_IN_TIME = 1.5f;
	IEnumerator Reset_Enter()
	{

       
        yield return null;
        
    }

 
}
