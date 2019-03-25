using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KBEngine;
using UnityEngine.UI;
public class MJpanel : MonoBehaviour {
	Transform leaveBtn,changeRoomBtn, readyBtn;
	Text roomIdText;
	public Text numOfMJText;
	public int roomMaxPlayer = 2;
	public static MJpanel instance;
	public Room room;
	public Account account;
	public static bool roomIsIn;
	ActionData _actionData;
	List<playerInfo> playerInfoList = new List<playerInfo>();
	List<playerInfo> realyPlayer = new List<playerInfo>();
	public Transform MyCardlist, holdsCardPrfb, holdsRoot;
	// Use this for initialization
	void Start () {
		account = (Account)KBEngineApp.app.player();
		instance = this;
		//创建玩家消息
		for (int i = 0; i < 4; i++) {
			playerInfoList.Add(new playerInfo(transform.Find("playerInfo/player"+(i+1)), i));
		}
		_actionData = new ActionData();
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
	playerInfo GetPlayerInfoByUserID(int userID) {
		print(userID);
		foreach (playerInfo pi in playerInfoList) {
			if (pi.account != null && pi.account.id == userID) {
				return pi;
			}
				
		}
		return null;
	}
	playerInfo GetPlayerInfoByServerIndex(int index)
	{
		int newIndex = getLocalSeatIndex(index);
		return playerInfoList[newIndex];
	}
	void installEvents()
	{
		KBEngine.Event.registerOut("playerLevelRoom", this, "playerLevelRoom");
		KBEngine.Event.registerOut("set_isReady", this, "set_isReady");
		KBEngine.Event.registerOut("gameStart", this, "gameStart");
		KBEngine.Event.registerOut("onPlayCardOver", this, "onPlayCardOver");
		KBEngine.Event.registerOut("onPlayCard", this, "onPlayCard");
		KBEngine.Event.registerOut("onMoPai", this, "onMoPai");
		KBEngine.Event.registerOut("onOtherPlayerMoPai", this, "onOtherPlayerMoPai");
		KBEngine.Event.registerOut("onPeng", this, "onPeng");
		KBEngine.Event.registerOut("onGang", this, "onGang");
		KBEngine.Event.registerOut("onHu", this, "onHu");	
		KBEngine.Event.registerOut("chupai", this, "chupai");
		KBEngine.Event.registerOut("has_action", this, "has_action");
		KBEngine.Event.registerOut("GameOver", this, "GameOver");
		

	}
	public void GameOver() {
		foreach (var pi in realyPlayer)
		{
			pi.clearAllCard();
		}

		gameStart();
	}
	public void has_action() {
		_actionData.UpDataActionData();
	}
	public void chupai() {
		if (account.TingPaiList.Count > 0) {
			print("有听牌信息");
			account.showTingPaiInfo();
			//playerInfoList[0].showTingPaiInfo();
		}
	}
	public void onHu(int userID,bool isZimo, int pai)
	{
		foreach (var pi in realyPlayer)
		{
			pi.heidChuPai();
		}
		GetPlayerInfoByUserID(userID).upDataHandCards();
		GetPlayerInfoByUserID(userID).upDataHuCard();
	}

	public void onGang(int userID, int pai)
	{
		foreach (var pi in realyPlayer)
		{
			pi.heidChuPai();
		}
		GetPlayerInfoByUserID(userID).upDataHandCards();
		GetPlayerInfoByUserID(userID).upDataPengCard();
		GetPlayerInfoByUserID(userID).upDataGangCard();
	}

	public void onPeng(int userID, int pai) {
		foreach (var pi in realyPlayer) {
			pi.heidChuPai();
		}
		GetPlayerInfoByUserID(userID).upDataHandCards();
		GetPlayerInfoByUserID(userID).upDataPengCard();
	}
	public void onMoPai(int pai) {
		GetPlayerInfoByUserID(KBEngineApp.app.player().id).upDataHandCards();
		numOfMJText.text = room.public_roomInfo.numOfMJ + "";
	}
	public void onOtherPlayerMoPai(int userID)
	{
		GetPlayerInfoByUserID(userID).upDataHandCards();
		numOfMJText.text = room.public_roomInfo.numOfMJ + "";
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
	public void onPlayCard(int userID, int pai)
	{
		GetPlayerInfoByUserID(userID).showChuPai(pai);
		GetPlayerInfoByUserID(userID).upDataHandCards();
		if (userID == account.id) {
			print("隐藏----------------------------------------------------");
			GetPlayerInfoByUserID(userID).heidAllJT();
		}
	}

	public void onPlayCardOver(int userID,int pai) {
		
		GetPlayerInfoByUserID(userID).heidChuPai();
		GetPlayerInfoByUserID(userID).upDataFlodsCard();
	}

	void initGameState_Playing() {
		//隐藏退出和就位等按钮
		readyBtn.gameObject.SetActive(false);
		leaveBtn.gameObject.SetActive(false);
		changeRoomBtn.gameObject.SetActive(false);
		//显示麻将剩余数量
		numOfMJText.text = room.public_roomInfo.numOfMJ+"";
		//初始化打出牌，碰、杠、胡牌
		for (int i = 0; i < room.public_roomInfo.playerInfo.Count; i++) {
			PLAYER_PUBLIC_INFO pb = room.public_roomInfo.playerInfo[i];
			playerInfo pi = GetPlayerInfoByServerIndex(i);
			realyPlayer.Add(pi);  //按照房间人数安排的玩家列表
			pi.initSeat(i);
			//隐藏准备的图片
			pi.changeReady(0);
			pi.upDataHandCards(); //初始化手中牌
			pi.upDataFlodsCard();  //更新打出的牌
			pi.upDataPengCard();  //更新碰的牌
			pi.upDataGangCard(); //更新杠的牌
			pi.upDataHuCard();//更新胡的牌
		}
		
		//处理正在打出的牌
		int chuPai = room.public_roomInfo.chuPai;
		if (chuPai != -1) {
			GetPlayerInfoByServerIndex(room.public_roomInfo.turn).showChuPai(chuPai);
		}
		//处理操作信息
		if (account.actionData.gang ==1 || account.actionData.peng == 1 || account.actionData.hu == 1) {
			account.showActionData();
			_actionData.UpDataActionData();	
		}
	}
	public void initIdle() {
		readyBtn.gameObject.SetActive(true);
		leaveBtn.gameObject.SetActive(true);
		changeRoomBtn.gameObject.SetActive(true);
		numOfMJText.text = "";
		for (int i = 0; i < room.public_roomInfo.playerInfo.Count; i++)
		{
			//PLAYER_PUBLIC_INFO pb = room.public_roomInfo.playerInfo[i];
			playerInfo pi = GetPlayerInfoByServerIndex(i);
			pi.root.gameObject.SetActive(true);
			pi.initSeat(i);
		}
	}
	public void gameStart() {
		//判断游戏状态
		
		switch (room.public_roomInfo.state) {
			case "idle":
				initIdle();
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
			roomMaxPlayer = room.public_roomInfo.playerMaxCount;
			//隐藏没有用到的椅子
			if (roomMaxPlayer == 2) {
				transform.Find("playerInfo/player2").gameObject.SetActive(false);
				transform.Find("playerInfo/player4").gameObject.SetActive(false);
			} else if (roomMaxPlayer == 3) {
				transform.Find("playerInfo/player4").gameObject.SetActive(false);
			}
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
public class NoUsePai
{
	public string pai;
	public List<string> tingPaiList = new List<string>();
	public List<string> tingPaiFans = new List<string>();
}
