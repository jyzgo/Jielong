using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CardLoader : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    public List<CardEdit> _list = new List<CardEdit>();
    public void AddCard(CardEdit card)
    {
        _list.Add(card);
        RefreshList();
    }

    public void RemoveCard(CardEdit card)
    {
        _list.Remove(card);
        RefreshList();

    }

    void RefreshList()
    {
        for(int i = 0; i < _list.Count; i ++)
        {
            var card = _list[i];
            card.transform.position = transform.position + (i+1) * new Vector3(0, -0.4f, -0.1f);
        }
    }
}
