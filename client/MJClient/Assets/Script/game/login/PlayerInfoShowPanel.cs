using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
using KBEngine;
public class PlayerInfoShowPanel : MonoBehaviour {
	Transform root,addFriendBtn,closeBtn;
	bool isOver;
	Account showac;
	// Use this for initialization
	void Start () {
		closeBtn = root.Find("close");
		EventInterface.AddOnEvent (addFriendBtn, Click);
		EventInterface.AddOnEvent(closeBtn, Click);
		root.localScale = new Vector3 (0,0,0);
		root.DOScale (new Vector3 (1, 1, 1), 0.3f).SetEase (Ease.OutBack);
	}
	public void showPanel(Account ac ) {
		showac = ac;
		print(showac.playerID);
		root = transform.Find("Root");
		addFriendBtn = root.Find("addFriend");
		print(ac.playerName);
		root.Find("name").GetComponent<Text>().text = ac.playerName;
		root.Find("gold").GetComponent<Text>().text = ac.playerGold+"";
		int dbid = ac.playerID - 10000;
		Account player = (Account)KBEngineApp.app.player();
		bool has = false;
		foreach (var item in player.friendsList)
		{
			if ((int)item == dbid) {
				has = true;
			}
		}
		if (!has) {
			addFriendBtn.gameObject.SetActive(true);
		}
		else {
			root.Find("tip").GetComponent<Text>().text ="你们已经是好友了";
			addFriendBtn.gameObject.SetActive(false);
		}
	}
	void OnDestroy()
	{
	
	}
	
	void close(){	
		root.DOScale(new Vector3(0,0,0),0.3f).SetEase(Ease.InBack).OnKill(()=>{
			Destroy(gameObject);
		});
	}
	// Update is called once per frame
	void Click (Transform tr) {
		if (isOver)
			return;
		isOver = true;
		audioManager.GetInstance().PlaySoundEffect("Music/buttonClick");
		if (tr == addFriendBtn) {
			Account player = (Account)KBEngineApp.app.player();
			
			player.baseEntityCall.reqAddFriend((ulong)showac.playerID - 10000);
		}
		close();
	}
}
