using UnityEngine;
using System.Collections;

public class ResMgr : MonoBehaviour {

    public Sprite heart;
    public Sprite club;
    public Sprite diamond;
    public Sprite spade;
    public GameObject cardEdit;
    public static ResMgr current;
    // Use this for initialization
    void Awake()
    {
        current = this;
    }


	// Update is called once per frame
	void Update () {
	
	}
}
