using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KBEngine;
public class test : MonoBehaviour {
	public InputField input,gangInput;
	public Transform sendPaiBtn,showActionDataBtn,pengBtn,gangBtn, huBtn, guoBtn;
	// Use this for initialization
	void Start () {
		EventInterface.AddOnEvent(sendPaiBtn, Click);
		EventInterface.AddOnEvent(showActionDataBtn, Click);
		EventInterface.AddOnEvent(pengBtn, Click);
		EventInterface.AddOnEvent(gangBtn, Click);
		EventInterface.AddOnEvent(huBtn, Click);
		EventInterface.AddOnEvent(guoBtn, Click);
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


	}
}
