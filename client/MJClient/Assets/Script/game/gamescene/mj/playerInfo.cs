using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KBEngine;
public class playerInfo  {
	public Transform root, isRead;
	public int localIndex = 0;
	public int ServerIndex = 0;
	public Text playerName, PlayerGold;
	Transform talkRoot;
	Text talkText;
	Account account;
	public seatInfo _seatInfo;
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
	public void initSeatInfo(seatInfo si)
	{
		_seatInfo = si;
		PlayerGold.text = si.score+"";
		if (si.ready)
		{
			isRead.gameObject.SetActive(true);
		}
		else {
			isRead.gameObject.SetActive(false);
		}
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

public class seatInfo
{
	public int userId = 0;
	public int score = 0;
	public bool ready = false;
}
