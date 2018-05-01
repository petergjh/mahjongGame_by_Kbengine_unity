using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loginManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameManager.GetInstance().LoadPanel("Prefabs/LoginPanel",GameObject.Find("Canvas/Root").transform);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
