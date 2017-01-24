using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using MTUnity.Utils;
using System.IO;


public class CardEditMgr : MonoBehaviour {


    public static CardEditMgr current;

    void Awake()
    {
        current = this;
    }

	// Use this for initialization
	void Start () {


        foreach (CardColor _cardColr in Enum.GetValues(typeof(CardColor)))
        {
            for (int i = 0; i < 13; i++)
            {
                var gb = Instantiate<GameObject>(ResMgr.current.cardEdit);
                var cardSc = gb.GetComponent<CardEdit>();
                cardSc.cardColor = _cardColr;
                cardSc.cardNum = i + 1;
                gb.name = "card " + _cardColr.ToString() + (i + 1).ToString();
                cardSc.UpdateView();
                gb.transform.position = new Vector3(i * 0.5f, (int)_cardColr-3, i * -0.5f);
            }
        }

    }

    public CardLoader[] cardLoaders;


    public List<CardEdit> _list = new List<CardEdit>();
	
	// Update is called once per frame
	void Update () {
	
	}

    public string saveFileName = string.Empty;
    public string loadFileName = string.Empty;
    public void GenP()
    {
        Debug.Log("Card not in use " + _list.Count);

        if(_list.Count >0 )
        {
            //return;
        }
        SerializeToJson();
    }

    void SerializeToJson()
    {
        MTJSONObject data = MTJSONObject.CreateDict();
        for(int i = 0; i < cardLoaders.Length;i ++)
        {
            var curCardLoader = cardLoaders[i];
            var curList = curCardLoader._list;
            MTJSONObject listJs = MTJSONObject.CreateList();
            //List<MTJSONObject> listJs = new List<MTJSONObject>();
            for(int j = 0; j <curList.Count;j++)
            {
                MTJSONObject cardJs = MTJSONObject.CreateDict();

                var curCardEdit = curList[j];
                int cardColor = (int)curCardEdit.cardColor;
                int cardNum = curCardEdit.cardNum;
                cardJs.Set("CardColor", cardColor);
                cardJs.Set("CardNum", cardNum);
                listJs.Add(cardJs);
            }
            
            data.Set(i.ToString(), listJs);
        }

        string fileName = Application.persistentDataPath+ "/" + saveFileName;
        Debug.Log("f " + fileName);
        File.WriteAllText(fileName, data.ToString());

    }

    public void LoadFrom()
    {
        string content = File.ReadAllText(Application.persistentDataPath + "/" + loadFileName);
        Debug.Log(" read " + loadFileName + "  : "  + content);
        MTJSONObject js = MTJSON.Deserialize(content);
        
        Debug.Log(" jj " + js.Get("1").ToString());
    }
}
