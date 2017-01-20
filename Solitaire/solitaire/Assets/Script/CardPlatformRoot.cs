using UnityEngine;
using System.Collections;

public class CardPlatformRoot : CardAbstract {

    public override bool isCardPutable(CardAbstract card)
    {
        return nextCard == null;
    }
}
