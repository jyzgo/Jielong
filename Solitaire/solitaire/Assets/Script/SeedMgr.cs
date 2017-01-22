using UnityEngine;
using System.Collections;

public class SeedMgr : MonoBehaviour {


    public SeedData _seedData;


    public static SeedMgr current;
    // Use this for initialization
   void Awake()
    {
        current = this;
    } 
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
