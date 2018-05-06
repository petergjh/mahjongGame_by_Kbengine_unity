using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
public class CreatPlayerPanel : MonoBehaviour {
	Transform root,creatBtn;
	InputField nameInput;
	bool isOver;
	// Use this for initialization
	void Start () {
		root = transform.Find ("root");
		creatBtn = root.Find ("creatBtn");
		nameInput = root.Find ("nameInput").GetComponent<InputField> ();
		EventInterface.AddOnEvent (creatBtn,Click);
		root.localScale = new Vector3 (0,0,0);
		root.DOScale (new Vector3 (1, 1, 1), 0.3f).SetEase (Ease.OutBack);
		installEvents ();
	}
	void installEvents()
	{
		KBEngine.Event.registerOut("OnReqCreateAvatar", this, "OnReqCreateAvatar");
	}
	void OnDestroy()
	{
		KBEngine.Event.deregisterOut(this);
	}
	public void OnReqCreateAvatar(int code)
	{
		GameManager.GetInstance().closeWaitingPanel();
		print("OnReqCreateAvatar " + code);
		if (code == 0)
		{
			GameManager.GetInstance().showMessagePanel("创建成功");

			close();
			isOver = true;
		}
		else if (code == 1)
		{
			GameManager.GetInstance().showMessagePanel("该名字已经被使用了！");
		}
		else if (code == 2)
		{
			GameManager.GetInstance().showMessagePanel("已经存在一个角色了！");

			close();
			isOver = true;
		}
	}
	void close(){
		if (isOver)
			return;		
		root.DOScale(new Vector3(0,0,0),0.3f).SetEase(Ease.InBack).OnKill(()=>{
			Destroy(gameObject);
		});
	}
	// Update is called once per frame
	void Click (Transform tr) {
		audioManager.GetInstance().PlaySoundEffect("Music/buttonClick");
		if (tr == creatBtn) {
			if (nameInput.text == "") {
				GameManager.GetInstance ().showMessagePanel ("角色名不能为空！");
				return;
			}
			if (nameInput.text.Length>=11) {
				GameManager.GetInstance ().showMessagePanel ("角色名不能超过11位！");
				return;
			}
			GameManager.GetInstance().showWaiting("正在创建中....", 10, () => {
				GameManager.GetInstance().showMessagePanel("连接超时！");
			});	
			KBEngine.Account acc = (KBEngine.Account)KBEngine.KBEngineApp.app.player ();
			if (acc != null) {
				acc.baseEntityCall.reqCreateAvatar(nameInput.text);
			}
		}
	}
}
