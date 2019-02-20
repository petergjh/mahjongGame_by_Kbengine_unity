using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KBEngine;
using DG.Tweening;
using System;
public class playerInfo  {
	int foldsMaxMJCount_a_line = 14;   //打出的牌一行显示几张牌
	public Transform root, isRead;
	public int localIndex = 0;
	public int ServerIndex = 0;
	public Text playerName, PlayerGold;
	Transform talkRoot;
	Text talkText;
	public bool isPlayer;
	public Account account;
	public Transform iconBG;
	//牌类信息
	public Transform MyCardlist, holdsCardPrfb, holdsRoot;
	Transform tingPaiInfo, tingPaiPrfb, tingPaiInfoRoot;
	public Transform Myflodslist, flodsCardPrfb, flodsRoot, flodsRoot2, playPaiPos;
	public Transform MyPengList, pengPrfb, pengRoot;
	public Transform MyGangList, diangangsPrf, diangangsRoot, wangangsPrf, wangangsRoot, angangsPrf, angangsRoot;
	public Transform MyHUList, huPrf, huRoot;
	public float holdsCardWeight, flodsCardWeight, flodsCardHeight, pengRootWeight;
	public Vector3 holdsStartPos;
	Transform seleMJpai;
	Dictionary<string, NoUsePai> _NoUsePai = new Dictionary<string, NoUsePai>();
	public playerInfo(Transform tr, int index)
	{
		root = tr;
		localIndex = index;
		isRead = root.Find("isReady");
		playerName = root.Find("playerName").GetComponent<Text>();
		PlayerGold = root.Find("playerGold").GetComponent<Text>();
		talkRoot = root.Find("talkRoot");
		if (talkRoot)
			talkText = talkRoot.Find("Text").GetComponent<Text>();
		
	}
	public void initSeat(int serverSeatIndex) {
		ServerIndex = serverSeatIndex;
		//0号位玩家的手牌单独操作
		MonoBehaviour.print("索引为--" + localIndex);
		iconBG = root.Find("iconBG");
		if (localIndex == 0)
		{
			isPlayer = true;
			MyCardlist = MJpanel.instance.transform.Find("MycardList");
			tingPaiInfo = MJpanel.instance.transform.Find("tingPaiInfo");
			tingPaiInfoRoot = tingPaiInfo.Find("root");
			tingPaiPrfb = tingPaiInfo.Find("prfb");
		}
		else
		{
			MyCardlist = root.Find("MycardList");
			holdsStartPos = MyCardlist.Find("holdCardStart").localPosition;
			EventInterface.AddOnEvent(iconBG, iconBGClick);
		}
		holdsCardPrfb = MyCardlist.Find("HoldsCard");
		holdsRoot = MyCardlist.Find("holdsRoot");
		holdsCardWeight = holdsCardPrfb.GetComponent<RectTransform>().rect.width;
		//打出的牌
		Myflodslist = root.Find("MyfoldsList");
		flodsCardPrfb = Myflodslist.Find("FoldsCord");
		flodsRoot = Myflodslist.Find("Root");
		flodsRoot2 = Myflodslist.Find("Root2");
		playPaiPos = MyCardlist.Find("playPaiPos");
		MyPengList = root.Find("MyPenList");
		pengPrfb = MyPengList.Find("pengPrf");
		pengRoot = MyPengList.Find("pengRoot");
		MyGangList = root.Find("MyGangList");
		diangangsPrf = MyGangList.Find("diangangsPrf");
		diangangsRoot = MyGangList.Find("diangangsRoot");
		wangangsPrf = MyGangList.Find("wangangsPrf");
		wangangsRoot = MyGangList.Find("wangangsRoot");
		angangsPrf = MyGangList.Find("angangsPrf");
		angangsRoot = MyGangList.Find("angangsRoot");
		MyHUList = root.Find("MyHuList");
		huPrf = MyHUList.Find("huCord");
		huRoot = MyHUList.Find("Root");
		flodsCardWeight = flodsCardPrfb.GetComponent<RectTransform>().rect.width;
		flodsCardHeight = flodsCardPrfb.GetComponent<RectTransform>().rect.height;
		pengRootWeight = pengPrfb.GetComponent<RectTransform>().rect.width;
	}

	private void iconBGClick(Transform tr)
	{
		if (account == null)
			return;
		GameObject go = GameManager.GetInstance().LoadPanel("Prefabs/PlayerInfoShowPanel", GameObject.Find("Canvas/Root").transform);
		go.GetComponent<PlayerInfoShowPanel>().showPanel(account);
	}

	//获取一张要实例化的麻将牌，用于展示
	/// <summary>
	/// Gets the MJ prfb.
	/// </summary>
	/// <param name="prfb">预制物，当节点上没有找到就实例化一个咯.</param>
	/// <param name="root">节点.</param>
	/// <param name="index">节点索引.</param>
	public GameObject getMJPrfb(GameObject prfb, Transform root, int index)
	{
		Transform tr = null;
		try
		{
			tr = root.GetChild(index);
		}
		catch
		{

		}

		if (tr == null)
		{
			return GameObject.Instantiate(prfb.gameObject);
		}
		else
		{
			return tr.gameObject;
		}
	}
	List<sbyte> copy(List<SByte> olditem)
	{
		List<SByte> newItem = new List<sbyte>();
		for (int i = 0; i < olditem.Count; i++) {
			newItem.Add(olditem[i]);
		}
		return newItem;
	}
	void initMyHandCords(bool isTurn)
	{
		for (int i = 0; i < holdsRoot.childCount; i++) {
			GameObject.Destroy(holdsRoot.GetChild(i).gameObject);
		}
		Account account = (Account)KBEngineApp.app.player();
		List<SByte> handCards = copy(account.holds);
		sbyte pai = account.holds[account.holds.Count - 1];
		if (isTurn)
		{
			handCards.Remove(pai);
		}
		handCards.Sort();
		handCards.Reverse();
		float paiWeight = holdsCardPrfb.GetComponent<RectTransform>().rect.width;
		for (int i = 0; i < handCards.Count; i++)
		{
			int hs = handCards[i];
			GameObject go = GameObject.Instantiate(holdsCardPrfb.gameObject);
			go.name = hs + "";
			go.transform.Find("hs").GetComponent<Image>().sprite = MJpanel.instance.getUISprite("UI/MJSprite/" + MJpanel.instance.getMJType(hs));
			go.transform.SetParent(holdsRoot, false);
			go.transform.localPosition = new Vector3(holdsCardPrfb.localPosition.x - i * paiWeight, holdsCardPrfb.localPosition.y, 0);
			go.SetActive(true);
			EventInterface.AddOnEvent(go.transform, ClickPai);
		}
		if (isTurn)
		{
			GameObject go = GameObject.Instantiate(holdsCardPrfb.gameObject);
			go.name = pai + "";
			go.transform.Find("hs").GetComponent<Image>().sprite = MJpanel.instance.getUISprite("UI/MJSprite/" + MJpanel.instance.getMJType(pai));
			go.transform.SetParent(holdsRoot, false);
			go.transform.localPosition = new Vector3(holdsCardPrfb.localPosition.x + 2 * paiWeight, holdsCardPrfb.localPosition.y, 0);
			go.SetActive(true);
			EventInterface.AddOnEvent(go.transform, ClickPai);
			handCards.Add(pai);
		}
	}
	void ClickPai(Transform tr) {
		if (seleMJpai == null)
		{
			seleMJpai = tr;
			seleMJpai.DOLocalMoveY(holdsCardPrfb.localPosition.y+50, 0.3f);
		}
		else {
			if (tr != seleMJpai)
			{
				seleMJpai.DOLocalMoveY(holdsCardPrfb.localPosition.y, 0.3f);
				seleMJpai = tr;
				seleMJpai.DOLocalMoveY(holdsCardPrfb.localPosition.y + 50, 0.3f);
			}
			else {
				
				Room rm = MJpanel.instance.room;
				Account ac = (Account)KBEngineApp.app.player();
				if (ac != null && rm != null)
				{
					if (rm.public_roomInfo.turn == ServerIndex)
					{
						rm.cellEntityCall.chuPai((sbyte)(int.Parse(seleMJpai.name)));
						heidTingPaiInfo();
						return;
					}
					else {
						seleMJpai.DOLocalMoveY(holdsCardPrfb.localPosition.y, 0.3f);
						seleMJpai = null;
					}
				}
				else{
					MonoBehaviour.print("网络不稳定，即将断线重连！！！");
				}
				
			}


		}
		if (seleMJpai.Find("jt").gameObject.activeSelf)
			showTingPaiInfo(seleMJpai.name);
		else
			heidTingPaiInfo();
	}
	void showTingPaiInfo(string pai)
	{
		heidTingPaiInfo();
		if (hasPai_IN_nousePai(pai, _NoUsePai))
		{
			tingPaiInfo.Find("bg").GetComponent<RectTransform>().sizeDelta = new Vector2(116 * _NoUsePai[pai].tingPaiList.Count, tingPaiInfo.Find("bg").GetComponent<RectTransform>().rect.height);
			for (int i = 0; i < _NoUsePai[pai].tingPaiList.Count; i++)
			{
				GameObject item = getMJPrfb(tingPaiPrfb.gameObject, tingPaiInfoRoot, i);
				item.name = pai + "";
				item.transform.Find("hs").GetComponent<Image>().sprite = MJpanel.instance.getUISprite("UI/MJSprite/" + MJpanel.instance.getMJType(int.Parse(_NoUsePai[pai].tingPaiList[i])));
				item.transform.localPosition = tingPaiPrfb.transform.localPosition + new Vector3(i * 108, 0, 0);
				item.transform.Find("Text").GetComponent<Text>().text = _NoUsePai[pai].tingPaiFans[i] + "番";
				item.transform.SetParent(tingPaiInfoRoot, false);
				item.gameObject.SetActive(true);
			}
			tingPaiInfo.gameObject.SetActive(true);
		}
	}
	void heidTingPaiInfo()
	{
		tingPaiInfo.gameObject.SetActive(false);
		for (int i = 0; i < tingPaiInfoRoot.childCount; i++)
		{
			tingPaiInfoRoot.GetChild(i).gameObject.SetActive(false);
		}
	}
	bool hasPai_IN_nousePai(string pai, Dictionary<string, NoUsePai> no)
	{
		if (no.ContainsKey(pai))
			return true;
		return false;
	}

	public void upDataPlayerHandCards()
	{
		Account account = (Account)KBEngineApp.app.player();
		//初始化玩家手中牌
		if (account.roomSeatIndex == MJpanel.instance.room.public_roomInfo.turn && MJpanel.instance.room.public_roomInfo.chuPai == -1)
		{
			initMyHandCords(true);

			//显示听牌信息
			showTingPaiInfo();
		}
		else
		{
			initMyHandCords(false);
		}
	}
	//显示听牌信息
	public void showTingPaiInfo()
	{
		heidAllJT();
		if (account == null)
			account = (Account)KBEngineApp.app.player();
		TING_PAI_LIST d = account.TingPaiList;
		if (d.Count == 0)
			return;
		Dictionary<string, string> data = new Dictionary<string, string>();
		List<Dictionary<int, int>> fanshu = new List<Dictionary<int, int>>();
		List<Dictionary<string, string>> d1 = new List<Dictionary<string, string>>();
		List<int> noUsePai = new List<int>();
		List<NoUsePai> no = new List<NoUsePai>();
		Dictionary<string, NoUsePai> no1 = new Dictionary<string, NoUsePai>();
		for (int i = 0; i < d.Count; i++)
		{
			TING_PAI_DIC ff = d[i];
			string pai = ff.nousepai;
			string P = ff.pai;
			string fan = ff.fan;
			if (!no1.ContainsKey(pai))
			{  //!hasPai_IN_nousePai(pai,no1)
				NoUsePai _no = new NoUsePai();
				_no.pai = pai + "";
				_no.tingPaiList.Add(P);
				_no.tingPaiFans.Add(fan);
				no1.Add(pai, _no);
			}
			else
			{
				NoUsePai _no = no1[pai];
				if (!_no.tingPaiList.Contains(P))
				{
					_no.tingPaiList.Add(P);
					_no.tingPaiFans.Add(fan);
				}
			}
		}
		_NoUsePai = no1;
		for (int i = 0; i < holdsRoot.childCount; i++)
		{
			Transform tr = holdsRoot.GetChild(i);
			if (no1.ContainsKey(tr.name))
			{
				tr.Find("jt").gameObject.SetActive(true);
			}
		}
	}
	//隐藏听牌信息
	public void heidAllJT()
	{
		for (int i = 0; i < holdsRoot.childCount; i++)
		{
			Transform tr = holdsRoot.GetChild(i);
			tr.Find("jt").gameObject.SetActive(false);
		}
	}

	//刷新手牌
	public void upDataHandCards() {
		if (isPlayer)
		{
			upDataPlayerHandCards();
		}
		else {

			PLAYER_PUBLIC_INFO si = MJpanel.instance.room.public_roomInfo.playerInfo[ServerIndex];
			bool isPlay = (MJpanel.instance.room.public_roomInfo.turn == ServerIndex && MJpanel.instance.room.public_roomInfo.chuPai==-1) ? true : false;
			int cardCount = si.holdsCount;

			if (holdsRoot.childCount > cardCount)
			{

				for (int i = holdsRoot.childCount - 1; i >= cardCount; i--)
				{
					MonoBehaviour.Destroy(holdsRoot.GetChild(i).gameObject);
				}
			}
			int index = 0;
			for (int i = 0; i < cardCount; i++)
			{
				GameObject item = getMJPrfb(holdsCardPrfb.gameObject, holdsRoot, i);
				item.name = i + "";
				item.transform.SetParent(holdsRoot, false);
				if (isPlay && i == cardCount - 1)
					item.transform.localPosition = new Vector3(holdsStartPos.x - (holdsCardWeight * 1.5f), holdsStartPos.y, 0);
				else
					item.transform.localPosition = new Vector3(holdsStartPos.x + ((holdsCardWeight - 3) * index), holdsStartPos.y, 0);
				item.gameObject.SetActive(true);
				index++;
			}
		}

	}
	//刷新打出的牌
	public void upDataFlodsCard() {
		PLAYER_PUBLIC_INFO _seatInfo = MJpanel.instance.room.public_roomInfo.playerInfo[ServerIndex];
		MonoBehaviour.print(_seatInfo.userId);
		int cardCount = _seatInfo.holdsCount;
		int index = ServerIndex;
		MonoBehaviour.print(_seatInfo.folds.Count);
		for (int i = 0; i < _seatInfo.folds.Count; i++)
		{
			GameObject item;
			if (i > foldsMaxMJCount_a_line)
			{
				item = getMJPrfb(flodsCardPrfb.gameObject, flodsRoot2, i - foldsMaxMJCount_a_line + 1);
			}
			else
			{
				item = getMJPrfb(flodsCardPrfb.gameObject, flodsRoot, i);
			}

			int pai = (int)(sbyte)_seatInfo.folds[i];
			if (item.name == pai + "")
				continue;
			item.name = pai + "";
			item.transform.Find("hs").GetComponent<Image>().sprite = MJpanel.instance.getUISprite("UI/MJSprite/" + MJpanel.instance.getMJType(pai));
			if (i > foldsMaxMJCount_a_line)
			{
				item.transform.SetParent(flodsRoot2, false);
				item.transform.localPosition = new Vector3(flodsCardWeight * (i - foldsMaxMJCount_a_line - 1), 0, 0);
			}
			else
			{
				item.transform.SetParent(flodsRoot, false);
				item.transform.localPosition = new Vector3(flodsCardWeight * i, 0, 0);
			}
			item.gameObject.SetActive(true);
		}
	}
	public void upDataPengCard() {
		PLAYER_PUBLIC_INFO si = MJpanel.instance.room.public_roomInfo.playerInfo[ServerIndex];
		for (int i = 0; i < si.pengs.Count; i++)
		{
			int pai = si.pengs[i];
			GameObject item = getMJPrfb(pengPrfb.gameObject, pengRoot, i);
			if (item.name == pai + "")
				continue;
			item.name = pai + "";
			for (int j = 0; j < item.transform.childCount; j++)
			{
				item.transform.GetChild(j).Find("hs").GetComponent<Image>().sprite = MJpanel.instance.getUISprite("UI/MJSprite/" + MJpanel.instance.getMJType(pai));
			}
			item.transform.SetParent(pengRoot, false);
			item.transform.localPosition = new Vector3(pengRootWeight * i + (i * 20), 0, 0);
			item.gameObject.SetActive(true);
		}
	}
	public void showChuPai(int pai) {
		playPaiPos.gameObject.SetActive(true);
		playPaiPos.transform.Find("hs").GetComponent<Image>().sprite = MJpanel.instance.getUISprite("UI/MJSprite/" + MJpanel.instance.getMJType(pai));
	}
	void deleAllChild(Transform tr) {
		for (int i = 0; i < tr.childCount; i++)
		{
			GameObject.Destroy(tr.GetChild(i).gameObject);
		}
	}
	public void clearAllCard() {
		//删除手牌
		deleAllChild(holdsRoot);
		//删除打出去的牌
		deleAllChild(flodsRoot);
		deleAllChild(flodsRoot2);
		//删除碰
		deleAllChild(pengRoot);
		deleAllChild(diangangsRoot);
		deleAllChild(wangangsRoot);
		deleAllChild(angangsRoot);
		deleAllChild(huRoot);
	}
	public void heidChuPai()
	{
		playPaiPos.gameObject.SetActive(false);
	}
	public void upDataHuCard() {
		PLAYER_PUBLIC_INFO si = MJpanel.instance.room.public_roomInfo.playerInfo[ServerIndex];
		for (int i = 0; i < si.hus.Count; i++)
		{
			MonoBehaviour.print("--hupai--" + si.hus[i]);
			int pai = si.hus[i];
			GameObject item = getMJPrfb(huPrf.gameObject, huRoot, i);
			if (item.name == pai + "")
				continue;
			item.name = pai + "";
			item.transform.Find("hs").GetComponent<Image>().sprite = MJpanel.instance.getUISprite("UI/MJSprite/" + MJpanel.instance.getMJType(pai));
			item.transform.SetParent(huRoot, false);
			item.transform.localPosition = new Vector3(flodsCardWeight * i, 0, 0);
			item.gameObject.SetActive(true);
		}
	}
	public void upDataGangCard() {
		PLAYER_PUBLIC_INFO si = MJpanel.instance.room.public_roomInfo.playerInfo[ServerIndex];
		int stratPosInt = si.pengs.Count;
		//处理点杆
		for (int i = 0; i < si.diangangs.Count; i++)
		{
			MonoBehaviour.print("si.diangangs == " + si.diangangs[i]);
			int pai = si.diangangs[i];
			GameObject item =getMJPrfb(diangangsPrf.gameObject, diangangsRoot, i);
			if (item.name == pai + "")
				continue;
			item.name = pai + "";
			for (int j = 0; j < item.transform.childCount; j++)
			{
				item.transform.GetChild(j).Find("hs").GetComponent<Image>().sprite = MJpanel.instance.getUISprite("UI/MJSprite/" + MJpanel.instance.getMJType(pai));
			}
			item.transform.SetParent(diangangsRoot, false);
			item.transform.localPosition = new Vector3(pengRootWeight * (i + stratPosInt) + ((i + stratPosInt) * 20), 0, 0);
			item.gameObject.SetActive(true);
		}
		stratPosInt += si.diangangs.Count;
		//处理wanGang
		for (int i = 0; i < si.wangangs.Count; i++)
		{
			MonoBehaviour.print("si.wangangs == " + si.wangangs[i]);
			int pai = si.wangangs[i];
			GameObject item = getMJPrfb(wangangsPrf.gameObject, wangangsRoot, i);
			if (item.name == pai + "")
				continue;
			item.name = pai + "";
			for (int j = 0; j < item.transform.childCount; j++)
			{
				item.transform.GetChild(j).Find("hs").GetComponent<Image>().sprite = MJpanel.instance.getUISprite("UI/MJSprite/" + MJpanel.instance.getMJType(pai));
			}
			item.transform.SetParent(wangangsRoot, false);
			item.transform.localPosition = new Vector3(pengRootWeight * (i + stratPosInt) + ((i + stratPosInt) * 20), 0, 0);
			item.gameObject.SetActive(true);
		}
		stratPosInt += si.wangangs.Count;
		//处理暗杆
		for (int i = 0; i < si.angangs.Count; i++)
		{
			MonoBehaviour.print("si.angangs == " + si.angangs[i]);
			int pai = si.angangs[i];
			GameObject item = getMJPrfb(angangsPrf.gameObject, angangsRoot, i);
			if (item.name == pai + "")
				continue;
			item.name = pai + "";
			item.transform.Find("4/hs").GetComponent<Image>().sprite = MJpanel.instance.getUISprite("UI/MJSprite/" + MJpanel.instance.getMJType(pai));
			item.transform.SetParent(angangsRoot, false);
			item.transform.localPosition = new Vector3(pengRootWeight * (i + stratPosInt) + ((i + stratPosInt) * 20), 0, 0);
			item.gameObject.SetActive(true);
		}

	}
	public void leaveRoom() {
		account = null;
		ServerIndex = 0;
		playerName.text = "";
	}
	public void initPlayer(Account ac) {
		account = ac;
		ServerIndex = ac.roomSeatIndex;
		playerName.text = account.playerName;
	}
	
	public void clearPlayer() {
		playerName.text = "";
		PlayerGold.text = "";
		isRead.gameObject.SetActive(false);
	}
	public void changeReady(int state) {
		bool st = (state == 1 ?true: false);
		isRead.gameObject.SetActive(st);
	}
}
