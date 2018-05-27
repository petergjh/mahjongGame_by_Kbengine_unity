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
		//GameManager.GetInstance().LoadPanel("Prefabs/MJPanel", GameObject.Find("Canvas/Root").transform);
		print("进入房间成功");
		GameManager.GetInstance().gameSceneLoadeOver();
	}

	public void EnterWorld(KBEngine.Entity entity)
	{
		if (entity.className == "Account")
			print(((Account)entity).playerName + "进入房间了");
		else if (entity.className == "Room") {
			print("该房间id--"+((Room)entity).roomKey);
		}
	}
		// Update is called once per frame
		void Update () {
		
	}
}
