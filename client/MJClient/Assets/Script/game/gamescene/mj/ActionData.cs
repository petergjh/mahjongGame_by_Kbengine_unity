using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KBEngine;
public class ActionData
{
	Transform ActionPanel, btn_gang, btn_pen, btn_hu, btn_guo;
	public int pai;
	public bool hu;
	public bool peng;
	public bool gang;
	public List<int> gangpai = new List<int>();
	public Account account;
	public ActionData()
	{
		account = (Account)KBEngineApp.app.player();
		ActionPanel = MJpanel.instance.transform.Find("ActionPanel");
		btn_gang = ActionPanel.Find("gang");
		btn_pen = ActionPanel.Find("pen");
		btn_hu = ActionPanel.Find("hu");
		btn_guo = ActionPanel.Find("guo");
		EventInterface.AddOnEvent(btn_gang, click);
		EventInterface.AddOnEvent(btn_pen, click);
		EventInterface.AddOnEvent(btn_hu, click);
		EventInterface.AddOnEvent(btn_guo, click);
		ActionPanel.gameObject.SetActive(false);
	}
	public void showAction()
	{
		if (account.actionData.gang == 1 || account.actionData.peng == 1 || account.actionData.hu == 1)
		{
			ActionPanel.gameObject.SetActive(true);
			int actionCount = 0;
			if (hu)
			{
				btn_hu.localPosition = btn_guo.localPosition + new Vector3(-90, 0, 0);
				btn_hu.gameObject.SetActive(true);
				actionCount++;
			}
			if (peng)
			{
				actionCount++;
				btn_pen.localPosition = btn_guo.localPosition + new Vector3(-90 * actionCount, 0, 0);
				btn_pen.gameObject.SetActive(true);
			}
			if (gang)
			{
				actionCount++;
				btn_gang.localPosition = btn_guo.localPosition + new Vector3(-90 * actionCount, 0, 0);
				btn_gang.gameObject.SetActive(true);
			}
			btn_guo.gameObject.SetActive(true);
		}	
	}
	public void UpDataActionData()
	{
		clearData();
		pai = (int)account.actionData.pai;
		hu = account.actionData.hu == 1 ? true : false;
		peng = account.actionData.peng == 1 ? true : false;
		gang = account.actionData.gang == 1 ? true : false;
		MJ_LIST _gangpai = account.actionData.gangpai;
		gangpai.Clear();
		for (int i = 0; i < _gangpai.Count; i++)
		{
			gangpai.Add((sbyte)_gangpai[i]);
		}
		showAction();
	}
	void clearData()
	{
		for (int i = 0; i < ActionPanel.childCount; i++)
		{
			ActionPanel.GetChild(i).gameObject.SetActive(false);
		}
		pai = 0;
		hu = false;
		gang = false;
		peng = false;
		gangpai.Clear();
	}
	void click(Transform tr)
	{
		audioManager.GetInstance().PlaySoundEffect("Music/buttonClick");
		Account ac = (Account)KBEngineApp.app.player();
		Room rm = MJpanel.instance.room;
		if (tr == btn_gang)
		{
			if (gangpai.Count == 1)

				rm.cellEntityCall.reqGang(ac.actionData.gangpai[0]);
			else
			{
				for (int i = 0; i < gangpai.Count; i++)
					MonoBehaviour.print("第" + i + "张杆牌为 ==" + gangpai[i]);

				//暂时默认杆第一张
				rm.cellEntityCall.reqGang(ac.actionData.gangpai[0]);
			}
		}
		else if (tr == btn_pen)
		{
			rm.cellEntityCall.reqPeng();
		}
		else if (tr == btn_hu)
		{
			rm.cellEntityCall.reqHu();
		}
		else if (tr == btn_guo)
		{
			rm.cellEntityCall.reqGuo();
		}
		clearData();
		ActionPanel.gameObject.SetActive(false);
	}
}