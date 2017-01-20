using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using MTUnity.Actions;

public enum CardColor
{
    Spade,
    Heart,
    Club,
    Dimaond,
}

public enum CardState
{
    None,
    
    InPile,
    InPileReady,
    InPlatform,
    InTarget
}

public class Card : CardAbstract {

    public SpriteRenderer Center;
    public SpriteRenderer Icon;
    public Text num;

	// Use this for initialization
	void Start () {

        UpdateCardView();
	}

    public void UpdateCardView()
    {
        CardStruct cardData = LevelMgr.current.GetCard(cardColor, CardNum);
        Center.sprite = cardData.Center;
        Icon.sprite = cardData.Icon;
        string name = CardNum.ToString();
        if (CardNum == 11)
        {
            name = "J";
        }
        else if (CardNum == 12)
        {
            name = "Q";
        }
        else if (CardNum == 13)
        {
            name = "K";
        }else if(CardNum ==1 )
        {
            name = "A";
        }

        num.text = name;


        if ((int)cardColor % 2 == 0)
        {
            num.color = Color.black;
        }
        else
        {
            num.color = Color.red;
        }

        if (CardNum <= 10)
        {
            Center.transform.localScale = new Vector3(2f, 2f, 1);
        }
 
    }

    public override bool isUp()
    {
        if (transform.eulerAngles.y > 160f && transform.eulerAngles.y < 190f)
        {
            return false;
        }
        return true;
    }

    bool isCtrlAble = false;
	// Update is called once per frame
    void OnMouseDrag()
    {
        if (isUp())
        {
            if (isCtrlAble)
            {
                Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z);
                Vector3 objPos = Camera.main.ScreenToWorldPoint(mousePosition);
                transform.position = new Vector3(objPos.x + offsetPos.x, objPos.y + offsetPos.y, -5f);
            }

        }
    }


    public override bool IsPutAble()
    {
        return isUp();
    }

    float downTime = 0f;
    
    Vector3 offsetPos;
    Vector3 originalPos;
    void OnMouseDown()
    {
        var curReadyList = LevelMgr.current.PileReadyList;
        isCtrlAble = false;
        if (!curReadyList.Contains(gameObject))
        {
            isCtrlAble = true;
        }else
        {
           if( curReadyList[curReadyList.Count-1] == gameObject)
            {

                isCtrlAble = true;
            }
        }
        if (isCtrlAble)
        {
            isFloating = true;
            downTime = Time.time;
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z);
            Vector3 objPos = Camera.main.ScreenToWorldPoint(mousePosition);
            offsetPos = transform.position - objPos;
            originalPos = transform.position;
        }
  

    }

    const float pressInterval = 0.1f;
    void OnMouseUp()
    {
        isFloating = false;
        if (isCtrlAble)
        {
            if (Time.time - downTime < 0.2f)
            {
                Press();
                // BackToOriginalPos();
            }
            else
            {
                Release();
            }
        }
        isCtrlAble = false;
    }

    void Press()
    {
        if(cardState == CardState.InPile)
        {
            if (isUp() == false)
            {
                LevelMgr.current.FlipPile();
            }
        }
        
    }

    void Release()
    {
       
        GameObject card = null;
        foreach(var en in _enter)
        {
            if(card == null)
            {
                card = en;
            }else
            {
                var curPos = transform.position;
                var curPos2D = new Vector2(curPos.x, curPos.y);

                var otherPos = en.transform.position;
                var other2D = new Vector2(otherPos.x, otherPos.y);

                var cardPos = card.transform.position;
                var cardPos2D = new Vector2(cardPos.x, cardPos.y);

                if(Vector2.Distance(other2D,curPos2D) < Vector2.Distance(cardPos2D,curPos2D))
                {
                    card = en;
                }
            }
        }

        if(card != null)
        {
            var cardSc = card.GetComponent<CardAbstract>();
           if(cardSc.isCardPutable(this))
            {
                cardSc.PutCard(this);

            }else
            {
                BackToOriginalPos();
            }
            
        }else
        {
            BackToOriginalPos();
        }

    }

    void BackToOriginalPos()
    {
        this.RunAction(new MTMoveToWorld(0.2f, originalPos));

    }




    public override bool isCardPutable(CardAbstract card)
    {


        if(nextCard != null)
        {
            return false;
        }

        if(IsPutAble() != true)
        {
            return false;
        }

        if(cardState == CardState.InPlatform)
        {
            if ((int)cardColor % 2 != (int)card.cardColor % 2 && card.CardNum == CardNum-1 && CardNum > 1)
            {
                return true;
            }
            else
            {
                return false;
            }
            
        }else if(cardState == CardState.InTarget)
        {
            if(cardColor == card.cardColor && card.CardNum == CardNum +1)
            {
                return true;
            }
            return false;
        }else if(cardState == CardState.InPile)
        {
            return true;
        }

        return false;
    }










}
