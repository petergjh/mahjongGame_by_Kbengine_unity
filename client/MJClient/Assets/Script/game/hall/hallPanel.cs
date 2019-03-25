using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KBEngine;
using System;
using UnityEngine.UI;
using DG.Tweening;
public class hallPanel : MonoBehaviour {
	Account account;
	public static hallPanel instance;
	Transform topRoot,rangRoot,seleRoot,bottomRoot;
	Text playerNameText,goldText,roomCardText;
	Transform enterRoomBtn,xzddBtn,CreatRoomBtn;
	Transform enterRoomPanel,enterRoomPanel_enterRoomBtn,enterRoomPanel_returnBtn;
	InputField enterRoomPanel_InputField;
	Transform Friends_Content,Friend_item;
	Transform mailBtn;
	string _name,_id;
	public Transform noFriendsTr;
	// Use this for initialization
	void init(){
		topRoot = transform.Find ("top");
		seleRoot = transform.Find ("selePanel");
		playerNameText = topRoot.Find ("playerName").GetComponent<Text> ();
		goldText = topRoot.Find ("goldBG/Text").GetComponent<Text> ();
		roomCardText = topRoot.Find ("FKBG/Text").GetComponent<Text> ();
		rangRoot =  transform.Find ("Ranking");
		//topRoot.gameObject.SetActive (false);
		//rangRoot.gameObject.SetActive (false);
		enterRoomBtn = seleRoot.Find ("enterRoom");
		xzddBtn = seleRoot.Find ("XZMJ");
		CreatRoomBtn = seleRoot.Find ("CreatRoom");
		Friends_Content = transform.Find ("Ranking/Scroll View/Viewport/Content");
		Friend_item = transform.Find ("Ranking/item");
		EventInterface.AddOnEvent (enterRoomBtn, Click);
		EventInterface.AddOnEvent (xzddBtn, Click);
		EventInterface.AddOnEvent (CreatRoomBtn, Click);

		enterRoomPanel = transform.Find ("enterRoomPanel");
		enterRoomPanel_InputField = enterRoomPanel.Find ("InputField").GetComponent<InputField> ();
		enterRoomPanel_enterRoomBtn = enterRoomPanel.Find ("enterRoomBtn");
		enterRoomPanel_returnBtn = enterRoomPanel.Find ("returnBtn");
		EventInterface.AddOnEvent (enterRoomPanel_returnBtn, Click);
		EventInterface.AddOnEvent (enterRoomPanel_enterRoomBtn, Click);

		bottomRoot = transform.Find ("bottom");
		mailBtn = bottomRoot.Find ("message_btn");
		EventInterface.AddOnEvent (mailBtn, Click);
		account = (Account)KBEngineApp.app.player();
		account.baseEntityCall.reqFriendsList();
		showMailInfo(account);
	}

	public void showMailInfo(Account ac) {
		print("有邮件----"+account.mailList.Count);
		bool hasNew = false;
		if (account.mailList.Count > 0) {
			
			foreach (var item in account.mailList)
			{
				if (item.lookOver == 0)
					hasNew = true;
				break;
			}
		}
		if (hasNew)
		{
			mailBtn.GetChild(0).gameObject.SetActive(true);
		}
		else {
			mailBtn.GetChild(0).gameObject.SetActive(false);
		}
	}
	public void initFriendsListOK(PLAYRE_DATA_LIST arg1)
	{
		if (arg1.Count == 0)
		{
			MonoBehaviour.print("没有好友！！！");
			noFriendsTr.gameObject.SetActive(true);
		}
		else
		{
			noFriendsTr.gameObject.SetActive(false);
			foreach (Transform item in Friends_Content)
			{
				Destroy(item.gameObject);
			}
			MonoBehaviour.print("有好友---！" + arg1.Count);
			foreach (var item in arg1)
			{
				MonoBehaviour.print(item.playerName + "---" + item.playerDBID + "---" + item.playerGold + "---" + item.isOnLine);
				Transform tr = Instantiate(Friend_item);
				tr.SetParent(Friends_Content,false);
				tr.gameObject.SetActive(true);
				string active = item.isOnLine == 0 ? "离线" : "在线";
				tr.Find("name").GetComponent<Text>().text = item.playerName+"("+ active + ")";
				tr.Find("dbid").GetComponent<Text>().text = item.playerDBID+"";
				tr.Find("goldText").GetComponent<Text>().text = item.playerGold+"";
				Transform btn = tr.Find("Give");
				EventInterface.AddOnEvent(btn, SendMsg);
			}
		}

	}

	private void SendMsg(Transform tr)
	{
		//GameObject go = GameManager.GetInstance().LoadPanel("Prefabs/sendMsgPanel", GameObject.Find("Canvas/Root").transform);
		//go.GetComponent<sendMsgPanel>().showPanel(tr.parent);
		string d = tr.parent.Find("dbid").GetComponent<Text>().text;
		string name = tr.parent.Find("name").GetComponent<Text>().text;
		account.baseEntityCall.reqGiveGold(ulong.Parse(d), name);
	}

	void Click(Transform tr){
		if (tr == xzddBtn) {
			Account acc = (Account)KBEngineApp.app.player();
			if (acc != null)
			{
				acc.baseEntityCall.EnterMatchesMatch();
			}
		} else if (tr == enterRoomBtn) {
			
			
		} else if (tr == enterRoomPanel_returnBtn) {
			
		}else if (tr == enterRoomPanel_enterRoomBtn) {
			
		}else if(tr == CreatRoomBtn){
			
		}else if(tr == mailBtn){
			//account.baseEntityCall.reqSendMail(2, "xfsaf",3,account.playerName);
			GameManager.GetInstance().LoadPanel("Prefabs/MailPanel", GameObject.Find("Canvas/Root").transform);

		}
	}
	void initPlayerData() {
		playerNameText.text = "名字："+account.playerName_base + "    ID:" + account.playerID_base;

	}
	void loseNet() {
		if (account == null)
		{
			GameManager.GetInstance().showMessagePanel("已和服务器断开连接！", () => {
				Application.LoadLevel("Login");
			});
		}

	}

	
	void Start () {
		instance = this;
		init ();
		installEvents ();
		if (KBEngineApp.app == null) {
			loseNet();
			return;
		}
		account = (Account)KBEngineApp.app.player ();
		if (account == null) {
			loseNet();
			return;
		}
		if (account.isNewPlayer==1) {			
			GameManager.GetInstance().LoadPanel("Prefabs/CreatPlayerPanel", GameObject.Find("Canvas/Root").transform);
			return;
		}
		initPlayerData();
	}
	
	void installEvents()
	{
		KBEngine.Event.registerOut("OnReqCreateAvatar", this, "OnReqCreateAvatar");
		KBEngine.Event.registerOut("initFriendsListOK", this, "initFriendsListOK");
		KBEngine.Event.registerOut("upDataMailList", this, "upDataMailList");
		
	}
	public void upDataMailList() {
		showMailInfo(account);
	}
	public void OnReqCreateAvatar(int code)
	{
		if (code == 0)
		{

			initPlayerData();


		}
	}
	void OnDestroy()
	{
		KBEngine.Event.deregisterOut(this);
	}
	// Update is called once per frame
	void Update () {
		
	}
}
