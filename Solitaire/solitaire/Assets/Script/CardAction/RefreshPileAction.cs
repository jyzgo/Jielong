using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class RefreshPileAction : CardAction
{
    public override void DoAction()
    {
        LevelMgr levelMgr = LevelMgr.current;
        Record();
        levelMgr._pileList.Clear();
        for (int i = levelMgr._pileReadyList.Count - 1; i >= 0; i--)
        {
            var curCard = levelMgr._pileReadyList[i];
            levelMgr._pileList.Add(curCard);
            curCard.transform.position = levelMgr.Pile.transform.position + Vector3.back * 0.1f;
            curCard.transform.parent = levelMgr.Pile.transform;
            curCard.transform.eulerAngles = new Vector3(0, 180, 0);
        }
        levelMgr._pileReadyList.Clear();
    }

    List<GameObject> oldReadyList = new List<GameObject>();
    List<Vector3> oldCardPosList = new List<Vector3>();
    List<Transform> oldParentList = new List<Transform>();
    List<Vector3> oldEulerList = new List<Vector3>();
   public void Record()
    {
        LevelMgr levelMgr = LevelMgr.current;
        oldReadyList.Clear();
        oldCardPosList.Clear();
        oldParentList.Clear();
        oldEulerList.Clear();

        for(int i = 0; i <levelMgr._pileReadyList.Count;i ++)
        {
            var curCard = levelMgr._pileReadyList[i];
            oldReadyList.Add(curCard);
            oldCardPosList.Add(curCard.transform.position);
            oldParentList.Add(curCard.transform.parent);
            oldEulerList.Add(curCard.transform.eulerAngles);
        }
    }

    public override void ReverseAction()
    {
        LevelMgr levelMgr = LevelMgr.current;
        levelMgr._pileList.Clear();

        for(int i = 0; i <oldReadyList.Count; i ++)
        {
            var oldCard = oldReadyList[i];
            var oldPos = oldCardPosList[i];
            var oldParent = oldParentList[i];
            var oldEuler = oldEulerList[i];
            levelMgr._pileReadyList.Add(oldCard);
            oldCard.transform.parent = oldParent;
            oldCard.transform.position = oldPos;
            oldCard.transform.eulerAngles = oldEuler;
        }

    }


}
