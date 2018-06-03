using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KBEngine;
public class gameSceneManager : MonoBehaviour {

	// Use this for initialization
	public static gameSceneManager instance;
	void Start()
	{
		instance = this;
		GameManager.GetInstance().LoadPanel("Prefabs/MJPanel", GameObject.Find("Canvas/Root").transform);	
	}	
		// Update is called once per frame
		void Update () {
		
	}
}
