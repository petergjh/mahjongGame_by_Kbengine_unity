using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KBEngine;
using UnityEngine.UI;
public class MJpanel : MonoBehaviour {
	Transform leaveBtn,changeRoomBtn, readyBtn;
	Text roomIdText;
	public int roomMaxPlayer = 2;
	public static MJpanel instance;
	List<playerInfo> playerInfoList = new List<playerInfo>();
	// Use this for initialization
	void Start () {
		instance = this;
		//创建玩家消息
		for (int i = 0; i < 4; i++) {
			playerInfoList.Add(new playerInfo(transform.Find("playerInfo/player"+(i+1)), i));
		}
		roomIdText = transform.Find("roomID").GetComponent<Text>();
		leaveBtn = transform.Find("leaveBtn");
		changeRoomBtn = transform.Find("changeBtn");
		readyBtn = transform.Find("readyBtn");
		EventInterface.AddOnEvent(leaveBtn, Click);
		EventInterface.AddOnEvent(changeRoomBtn, Click);
		EventInterface.AddOnEvent(readyBtn, Click);
		print("进入麻将房间成功");
		GameManager.GetInstance().gameSceneLoadeOver();
		installEvents();
	}
	void installEvents()
	{
		KBEngine.Event.registerOut("playerLevelRoom", this, "playerLevelRoom");

	}
	//转换成局部座位
	public int getLocalSeatIndex(int severSeatIndex)
	{
		int myIndex =((Account)KBEngineApp.app.player()).roomSeatIndex;
		if (myIndex == 0)
		{
			//自己就在0号位
			if (myIndex == severSeatIndex)
				return 0;
			else
			{
				//2个玩家的话就面对面做就可以了
				if (roomMaxPlayer == 2)
					return 2;
			}
			return severSeatIndex;
		}
		else {
			if (myIndex == severSeatIndex)
				return 0;
			else {
				//2个玩家的话就面对面做就可以了
				if (roomMaxPlayer == 2)
				{
					return 2;
				}
				//左移自己的偏移量给别的玩家
				int of = severSeatIndex - roomMaxPlayer;
				if (of > 0)
				{
					return of;
				}
				else
				{
					return (roomMaxPlayer + of);
				}
			}

		}


	}
	public void playerLevelRoom() {
		UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("hall");
	}
	public void EnterWorld(KBEngine.Entity entity)
	{
		if (entity.className == "Account") {
			print(((Account)entity).playerName + "进入房间了");
			int oldIndex = ((Account)entity).roomSeatIndex;
			print("oldIndex----" + oldIndex);
			int newIndex = getLocalSeatIndex(oldIndex);
			print("newIndex----" + newIndex);
			playerInfoList[newIndex].initPlayer((Account)entity);

		}
		else if (entity.className == "Room")
		{
			print("该房间id--" + ((Room)entity).roomKey);
			roomIdText.text =""+ ((Room)entity).roomKey;
		}
	}
	// Update is called once per frame
	void Click(Transform tr) {
		if (tr == leaveBtn)
		{
			KBEngine.Account acc = (KBEngine.Account)KBEngine.KBEngineApp.app.player();
			if (acc != null)
			{
				acc.cellEntityCall.LeaveRoom();
			}

		}
		else if (tr == changeRoomBtn) {
			KBEngine.Account acc = (KBEngine.Account)KBEngine.KBEngineApp.app.player();
			if (acc != null)
			{
				acc.baseEntityCall.reqChangeRoom();
				instance = null;
			}

		} else if (tr == readyBtn) {
			KBEngine.Account acc = (KBEngine.Account)KBEngine.KBEngineApp.app.player();
			if (acc != null)
			{
				
			}
		}
		
	}
	void OnDestroy()
	{
		instance = null;
		KBEngine.Event.deregisterOut(this);
	}
}
