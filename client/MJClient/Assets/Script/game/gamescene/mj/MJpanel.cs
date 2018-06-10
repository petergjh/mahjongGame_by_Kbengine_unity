using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KBEngine;
using UnityEngine.UI;
public class MJpanel : MonoBehaviour {
	Transform leaveBtn;
	Text roomIdText;
	public static MJpanel instance;
	// Use this for initialization
	void Start () {
		instance = this;
		roomIdText = transform.Find("roomID").GetComponent<Text>();
		leaveBtn = transform.Find("leaveBtn");
		EventInterface.AddOnEvent(leaveBtn, Click);
		print("进入麻将房间成功");
		GameManager.GetInstance().gameSceneLoadeOver();
		installEvents();
	}
	void installEvents()
	{
		KBEngine.Event.registerOut("playerLevelRoom", this, "playerLevelRoom");

	}
	public void playerLevelRoom() {
		UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("hall");
	}
	public void EnterWorld(KBEngine.Entity entity)
	{
		if (entity.className == "Account")
			print(((Account)entity).playerName + "进入房间了");
		else if (entity.className == "Room")
		{
			print("该房间id--" + ((Room)entity).roomKey);
			roomIdText.text =""+ ((Room)entity).roomKey;
		}
	}
	// Update is called once per frame
	void Click(Transform tr) {
		if (tr == leaveBtn) {
			KBEngine.Account acc = (KBEngine.Account)KBEngine.KBEngineApp.app.player();
			if (acc != null) {
				//acc.cellEntityCall.LeaveRoom();
				acc.baseEntityCall.reqChangeRoom();
			}

		}
		
	}
}
