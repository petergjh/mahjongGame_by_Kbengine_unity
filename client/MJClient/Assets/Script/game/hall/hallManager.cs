using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hallManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GameManager.GetInstance().LoadPanel("Prefabs/HallPanel", GameObject.Find("Canvas/Root").transform);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
