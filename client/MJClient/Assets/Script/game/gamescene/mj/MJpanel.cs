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
	public Room room;
	public static bool roomIsIn;
	List<playerInfo> playerInfoList = new List<playerInfo>();
	public Transform MyCardlist, holdsCardPrfb, holdsRoot;
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
		installEvents();
		print("开始进入房间");
		if (GameManager.GetInstance().gameSceneLoadeOver())
		{
			print("进入麻将房间成功");
			roomIsIn = true;
		}
		else {
			timerManager.GetInstance().addTimer(0.1f, 0.1f, (timerID,userData) =>
			{
				if (GameManager.GetInstance().gameSceneLoadeOver()) {
					print("进入麻将房间成功222");
					timerManager.GetInstance().cancelTimer(timerID);
					roomIsIn = true;
				}
			});
		}
		
	}
	
	void installEvents()
	{
		KBEngine.Event.registerOut("playerLevelRoom", this, "playerLevelRoom");
		KBEngine.Event.registerOut("set_isReady", this, "set_isReady");
		KBEngine.Event.registerOut("gameStart", this, "gameStart");

	}
	public string getMJType(int pai)
	{
		if (pai >= 0 && pai < 9)
			return "tong" + (pai + 1); //筒
		else if (pai >= 9 && pai < 18)
			return "suo" + (pai - 8);  // 条
		else if (pai >= 18 && pai < 27)
			return "wan" + (pai - 17); //万

		return "";
	}
	public Sprite getUISprite(string path)
	{
		Sprite SpriteLocal = Resources.Load(path, typeof(Sprite)) as Sprite;
		if (SpriteLocal == null)
		{
			print("没有找到资源");
			return null;
		}
		return SpriteLocal;
	}

	void initMyHandCords(bool isTurn) {
		Account account = (Account)KBEngineApp.app.player();
		sbyte pai = account.holds[account.holds.Count - 1];
		if (isTurn) {
			account.holds.Remove(pai);
		}
		account.holds.Sort();
		float paiWeight = holdsCardPrfb.GetComponent<RectTransform>().rect.width;
		for (int i = 0; i < account.holds.Count; i++)
		{
			int hs = account.holds[i];
			print("第" + i + "张手牌为 ---" + GetPaiString(hs));
			GameObject go = Instantiate(holdsCardPrfb.gameObject);
			go.name = hs+"";
			go.transform.Find("hs").GetComponent<Image>().sprite = getUISprite("UI/MJSprite/" + getMJType(hs));
			go.transform.SetParent(holdsRoot, false);
			go.transform.localPosition = new Vector3(holdsCardPrfb.localPosition.x- i* paiWeight,holdsCardPrfb.localPosition.y,0);
			go.SetActive(true);
		}
		if (isTurn)
		{
			GameObject go = Instantiate(holdsCardPrfb.gameObject);
			go.name = pai + "";
			go.transform.Find("hs").GetComponent<Image>().sprite = getUISprite("UI/MJSprite/" + getMJType(pai));
			go.transform.SetParent(holdsRoot, false);
			go.transform.localPosition = new Vector3(holdsCardPrfb.localPosition.x+ 2* paiWeight, holdsCardPrfb.localPosition.y, 0);
			go.SetActive(true);
		}
	}
	void initGameState_Playing() {
		print("庄家索引是" + room.public_roomInfo.button);
		print(room.public_roomInfo.playerInfo[1].userId + " 玩家手牌数量==" + room.public_roomInfo.playerInfo[1].holdsCount);
		Account account = (Account)KBEngineApp.app.player();
		if (account.roomSeatIndex == room.public_roomInfo.turn)
		{
			initMyHandCords(true);
		}
		else
		{
			initMyHandCords(false);
		}
	}
	public void gameStart() {
		//判断游戏状态
		switch (room.public_roomInfo.state) {
			case "idel":
				break;
			case "playing":
				initGameState_Playing();
				break;
		}
		
		
	}
	public void set_isReady(Account entity, int state) {
		print((entity).playerName + "状态改变了");
		int oldIndex = (entity).roomSeatIndex;
		print("oldIndex----" + oldIndex);
		int newIndex = getLocalSeatIndex(oldIndex);
		print("newIndex----" + newIndex);
		playerInfoList[newIndex].changeReady(state);
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
	public void LeaveWorld(KBEngine.Entity entity) {
		if (entity.className == "Account")
		{
			print(((Account)entity).playerName + "离开房间了");
			int oldIndex = ((Account)entity).roomSeatIndex;
			print("oldIndex----" + oldIndex);
			int newIndex = getLocalSeatIndex(oldIndex);
			print("newIndex----" + newIndex);
			playerInfoList[newIndex].leaveRoom();
			

		}
	}
	string GetPaiString(int pai) {
		if (pai < 9) {
			return (pai+1)+"筒";
		} else if (pai < 18) {
			return ( pai-8) + "条";
		} else {
			return ( pai-17)+ "万";
		}
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
			//就位状态只有在idel状态才出现
			if (room.public_roomInfo.state == "idel")
			{
				playerInfoList[newIndex].changeReady(((Account)entity).isReady);
			}
			else {
				playerInfoList[newIndex].changeReady(0);
			}

		}
		else if (entity.className == "Room")
		{
			print("该房间id--" + ((Room)entity).roomKey);
			roomIdText.text =""+ ((Room)entity).roomKey;
			room = (Room)entity;
			gameStart();
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
			if (room != null && acc != null)
			{
				print(12321);
				room.cellEntityCall.reqChangeReadyState(acc.isReady);
			}
		}
		
	}
	void OnDestroy()
	{
		instance = null;
		roomIsIn = false;
		KBEngine.Event.deregisterOut(this);
	}
}
