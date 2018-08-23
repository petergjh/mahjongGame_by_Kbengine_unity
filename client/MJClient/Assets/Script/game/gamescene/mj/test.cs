using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KBEngine;
public class test : MonoBehaviour {
	public InputField input,gangInput;
	public Transform sendPaiBtn,showActionDataBtn,pengBtn,gangBtn, huBtn, guoBtn, showPublicInfoBtn, showTingPaiInfoBtn;
	// Use this for initialization
	void Start () {
		EventInterface.AddOnEvent(sendPaiBtn, Click);
		EventInterface.AddOnEvent(showActionDataBtn, Click);
		EventInterface.AddOnEvent(pengBtn, Click);
		EventInterface.AddOnEvent(gangBtn, Click);
		EventInterface.AddOnEvent(huBtn, Click);
		EventInterface.AddOnEvent(guoBtn, Click);
		EventInterface.AddOnEvent(showPublicInfoBtn, Click);
		EventInterface.AddOnEvent(showTingPaiInfoBtn, Click);
	}
	
	// Update is called once per frame
	void Click(Transform tr) {
		Account ac = (Account)KBEngineApp.app.player();
		Room rm = MJpanel.instance.room;
		if (tr == sendPaiBtn) {
			int pai = int.Parse(input.text);
			if (ac != null && rm != null)
			{
				rm.cellEntityCall.chuPai((sbyte)pai);
			}
		} else if (tr == showActionDataBtn) {
			ac.showActionData();
			if (ac.actionData.peng == 1)
			{
				pengBtn.gameObject.SetActive(true);
			}
			else {
				pengBtn.gameObject.SetActive(false);
			}
			if (ac.actionData.gang == 1)
			{
				gangBtn.gameObject.SetActive(true);
				if (ac.actionData.gangpai.Count > 1)
				{
					gangInput.gameObject.SetActive(true);
					gangBtn.Find("Text").GetComponent<Text>().text = "杠";
				}
				else {

					gangBtn.Find("Text").GetComponent<Text>().text = "杠-" + ac.actionData.gangpai[0];
				}
			}
			else
			{
				gangBtn.gameObject.SetActive(false);
				gangInput.gameObject.SetActive(false);
			}
			if (ac.actionData.hu == 1)
			{
				huBtn.gameObject.SetActive(true);
			}
			else {
				huBtn.gameObject.SetActive(false);
			}

			if (ac.actionData.hu == 1 || ac.actionData.peng == 1 || ac.actionData.gang == 1)
			{
				guoBtn.gameObject.SetActive(true);
			}
			else {
				guoBtn.gameObject.SetActive(false);
			}
		} else if (tr == pengBtn) {
			rm.cellEntityCall.reqPeng();
		}
		else if (tr == gangBtn)
		{
			if (ac.actionData.gangpai.Count == 1)
			{
				rm.cellEntityCall.reqGang(ac.actionData.gangpai[0]);
			}
			else
			{
				if (gangInput.text == "") {
					return;
				}
				rm.cellEntityCall.reqGang((sbyte)int.Parse(gangInput.text));
			}


		}
		else if (tr == huBtn)
		{
			rm.cellEntityCall.reqHu();
		}
		else if (tr == guoBtn)
		{
			rm.cellEntityCall.reqGuo();
		}
		else if (tr == showPublicInfoBtn)
		{
			print("房间最大人数--  " + rm.public_roomInfo.playerMaxCount);
			print("当前还剩于麻将牌--  " + rm.public_roomInfo.numOfMJ);
			print("当前游戏状态--  " + rm.public_roomInfo.state);
			print("当前庄家索引--  " + rm.public_roomInfo.button);
			print("当前操作玩家索引---  " + rm.public_roomInfo.turn);
			print("当前正在打出的牌为--  " + rm.public_roomInfo.chuPai);
			print("当前房间类型为--  " + rm.public_roomInfo.RoomType);
			print("-------开始展示玩家信息----------");
			foreach (PLAYER_PUBLIC_INFO _playerinfo in rm.public_roomInfo.playerInfo) {
				print("--开始展示玩家，id为--  " + _playerinfo.userId);
				print("打出的牌--" + showListToString(_playerinfo.folds));
				print("暗杠的牌--" + showListToString(_playerinfo.angangs));
				print("点杠的牌--" + showListToString(_playerinfo.diangangs));
				print("弯杠的牌--" + showListToString(_playerinfo.wangangs));
				print("碰的牌--" + showListToString(_playerinfo.pengs));
				print("胡的牌--  " + showListToString(_playerinfo.hus));
				print("手牌数量--  " + _playerinfo.holdsCount);
				print("--展示结束-------");
			}
		} else if (tr == showTingPaiInfoBtn) {
			if (ac.TingPaiList.Count == 0) {
				print("没有听牌啊！！！");
				return;
			}
			ac.showTingPaiInfo();

		}


	}
	string showListToString(MJ_LIST arr) {
		string info = "(";
		foreach (sbyte pai in arr) {
			if (info != "(")
				info = info + "," + pai.ToString();
			else
				info = info + pai + "";
		}
		return info+")";
	}
}
