using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
public class sendMsgPanel : MonoBehaviour {
	Transform root,creatBtn;
	InputField nameInput;
	bool isOver;
	Transform infoTr;
	// Use this for initialization
	void Start () {
		root = transform.Find ("root");
		creatBtn = root.Find ("creatBtn");
		nameInput = root.Find ("nameInput").GetComponent<InputField> ();
		EventInterface.AddOnEvent (creatBtn,Click);
		root.localScale = new Vector3 (0,0,0);
		root.DOScale (new Vector3 (1, 1, 1), 0.3f).SetEase (Ease.OutBack);
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
		audioManager.GetInstance().PlaySoundEffect("Music/buttonClick");
		if (tr == creatBtn) {
			if (nameInput.text == "") {
				GameManager.GetInstance ().showMessagePanel ("不能发送空才消息！");
				return;
			}
				
			KBEngine.Account acc = (KBEngine.Account)KBEngine.KBEngineApp.app.player ();
			if (acc != null) {
				isOver = true;
				string targetDBID = infoTr.Find("dbid").GetComponent<Text>().text;
				string targetName = infoTr.Find("name").GetComponent<Text>().text;
				acc.baseEntityCall.reqSendMail(ulong.Parse(targetDBID), targetName,1, nameInput.text);
				GameManager.GetInstance().showMessagePanel("发送成功！");
				close();
			}
		}
	}

	internal void showPanel(Transform tr)
	{
		infoTr = tr;
	}
}
