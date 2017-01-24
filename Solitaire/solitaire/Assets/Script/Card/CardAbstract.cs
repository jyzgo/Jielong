using UnityEngine;
using System.Collections;
using MTUnity.Actions;
using System.Collections.Generic;

public class CardAbstract : MonoBehaviour
{

    public virtual void Start()
    {
        RightHandMgr.current.AddCard(this);
    }

    public CardColor cardColor;
    [Range(1, 13)]
    public int CardNum;

    public CardState cardState = CardState.None;

    public virtual bool isCardPutable(CardAbstract card)
    {
        return true;
    }

    const float zOffset = -0.1f;
    readonly Vector3 nextCardPos = new Vector3(0, -0.2f, zOffset);
    readonly Vector3 inTargetPos = new Vector3(0, 0, zOffset);
    readonly Vector3 inPlatformUpPos = new Vector3(0, -0.3f, zOffset);
    readonly Vector3 inPlatformDownPos = new Vector3(0, -0.2f, zOffset);

    public virtual void BlockTouch(float t = 0.2f)
    {

    }

    public virtual bool isUp()
    {
        return false;
    }

    public virtual Vector3 GetNextPos(int i = 1)
    {
        var nextPos = Vector3.zero; 

        if(cardState == CardState.InTarget)
        {
            nextPos = inTargetPos;
        }else if(cardState == CardState.InPile)
        {
            nextPos = inTargetPos;
        }else if(cardState == CardState.InPlatform)
        {
            if(isUp())
            {
                nextPos = inPlatformUpPos;
            }else
            {
                nextPos = inPlatformDownPos;
            }
        }

        return transform.position + nextPos * i;
    }

    //[HideInInspector]
    public CardAbstract preCard;
   // [HideInInspector]
    public CardAbstract nextCard;

    public CardAbstract GetTopCard()
    {
        CardAbstract card = this;
        while (card.nextCard != null)
        {
            card = card.nextCard;
        }
        return card;
    }

    public Vector3 offsetPos;
    public Vector3 originalPos;

    public void PutCard(CardAbstract otherCard)
    {


        SoundManager.Current.Play_put_success(0);
        var cardAction = new CardActionImp();
        cardAction.Init(this, otherCard);
        cardAction.DoAction();
        LevelMgr.current.AddAction(cardAction);

    }



    public bool DetachCardFromOriginal()
    {
        if (preCard != null)
        {
            preCard.nextCard = null;
            if (preCard.cardState == CardState.InPlatform)
            {
                preCard.FlipCard();
                return true;
            }
        }
        return false;

    }

    public virtual void FlipCard()
    {
        gameObject.RunActions(new MTRotateTo(0.2f, Vector3.zero),new MTCallFunc(() => transform.eulerAngles = Vector3.zero ));
       
    }
    protected bool isFloating = false;

    public HashSet<GameObject> _enter = new HashSet<GameObject>();
    void OnTriggerEnter2D(Collider2D col)
    {
        if (isFloating)
        {
            var card = col.GetComponent<CardAbstract>();
            if (card != null && card.IsPutAble())
            {
                _enter.Add(card.gameObject);
            }

        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        _enter.Remove(col.gameObject);
    }

    public virtual bool IsPutAble()
    {
        return true;
    }
}
