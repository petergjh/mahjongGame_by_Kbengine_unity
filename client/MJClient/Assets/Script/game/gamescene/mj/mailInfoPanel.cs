using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KBEngine;
using UnityEngine.UI;
using DG.Tweening;
public class mailInfoPanel : MonoBehaviour {
    Transform root,returnBtn,okBtn,deleBtn;
	Account account;
    Text mailInfotext,okBtnText,deleBtnText;
    MAIL mail;
	// Use this for initialization
	void Awake () {
		root = transform.Find ("root");
		returnBtn = root.Find ("return");
        okBtn = root.Find("okBtn");
        deleBtn = root.Find("deleBtn");
        okBtnText = okBtn.Find("Text").GetComponent<Text>();
        deleBtnText = deleBtn.Find("Text").GetComponent<Text>();
		EventInterface.AddOnEvent (returnBtn, Click);
        EventInterface.AddOnEvent(okBtn, Click);
        EventInterface.AddOnEvent(deleBtn, Click);
		account = (Account)KBEngineApp.app.player ();
		root.DOScale (Vector3.one, 0.5f).SetEase (Ease.OutBack);
        mailInfotext = root.Find("info").GetComponent<Text>();
	}
	// Update is called once per frame
	void Click (Transform tr) {
		if(tr == okBtn){
            okAction();
        }else if(tr == deleBtn){
           
        }
        close();
	}
    void close(){
        root.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnKill(() => {
            DestroyObject(gameObject);
        });
    }
    void okAction(){
        switch (mail.mailType)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                
               
                break;
            case 3:
                break;
        }
    }
    public void showMailInfoPanel(MAIL _mail){
        mail = _mail;
        switch(mail.mailType){
            case 0:
                break;
            case 1:
                break;
            case 3:
                showAddFriendMail();
                break;
            case 2:
                showAddFriendSuccessMail();
                break;
        }
       
    }
    void showAddFriendMail(){
        mailInfotext.text = "  你好！ 我是 " +mail.senderName + " 我能添加你为好友吗？以后打牌叫我";
        okBtn.localPosition = new Vector3(-150,-161,0);
        deleBtn.localPosition = new Vector3(150, -161, 0);
        okBtn.gameObject.SetActive(true);
        deleBtn.gameObject.SetActive(true);
        okBtnText.text = "同意并添加好友";
        deleBtnText.text = "忽略并删除邮件";
    }
    void showAddFriendSuccessMail(){
        mailInfotext.text = mail.senderName + " 同意了你的加好友请求,你可以在好友列表里和他交流了";
        deleBtn.localPosition = new Vector3(0, -161, 0);
        okBtn.gameObject.SetActive(false);
        deleBtn.gameObject.SetActive(true);
        deleBtnText.text = "删除邮件";
    }
}
