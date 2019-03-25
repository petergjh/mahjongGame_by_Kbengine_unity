using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KBEngine;
using UnityEngine.UI;
using DG.Tweening;
public class MailPanel : MonoBehaviour {
    public static MailPanel instance;
    Transform root,returnBtn,Content,mailBase,mailInfoPanel;
	Account account;
	float posY;
	// Use this for initialization
	void Start () {
        instance =  this;
		root = transform.Find ("root");
		returnBtn = root.Find ("return");
		Content = root.Find ("Scroll View/Viewport/Content");
		mailBase = root.Find ("MailBase");
        mailInfoPanel = transform.Find("mailInfoPanel");
		EventInterface.AddOnEvent (returnBtn, Click);
		account = (Account)KBEngineApp.app.player ();
		root.DOScale (Vector3.one, 0.5f).SetEase (Ease.OutBack);
		posY = mailBase.GetComponent<RectTransform> ().rect.height;
		initMailList ();
	}
    void deleAllMail(){
        for (int i =0; i < Content.childCount; i++)
        {
            DestroyObject(Content.GetChild(i).gameObject);
        }
    }
	public void initMailList(){
        deleAllMail();
		Content.GetComponent<RectTransform> ().sizeDelta = new Vector2 (0,posY*account.mailList.Count);
		if (account.mailList.Count == 0) {
            root.Find("NoMail").gameObject.SetActive(true);
			return;
		}
		for (int i = 0; i < account.mailList.Count; i++) {
			MAIL mail = account.mailList [i];
            Transform tr = Instantiate (mailBase);
			string info = "";
			if (mail.mailType == 1) {
                info = mail.senderName + " 发了一封邮件给你，请尽快查收吧";
			} else if (mail.mailType == 3) {
                info = mail.senderName + " 想要添加你为好友，请尽快处理吧";
            }else if (mail.mailType == 2) {
                info = mail.senderName + "同意了你的加好友请求";
            }   			
			tr.Find ("info").GetComponent<Text> ().text = info;
			tr.Find ("look").GetComponent<Text> ().text = (mail.lookOver == 0 ? "未读" : "已读");
			tr.Find ("index").GetComponent<Text> ().text = i + "";
			tr.SetParent (Content, false);
			tr.gameObject.SetActive (true);
            EventInterface.AddOnEvent(tr,ClickMail);
		}
	}
   
	// Update is called once per frame
	void Click (Transform tr) {
		if (tr == returnBtn) {
			root.DOScale (Vector3.zero, 0.5f).SetEase (Ease.InBack).OnKill (() => {
				DestroyObject (gameObject);
			});
		}
	}
    void ClickMail(Transform tr)
    {
        tr.Find("look").GetComponent<Text>().text = "已读";
        int index = int.Parse(tr.Find("index").GetComponent<Text>().text);
        MAIL mail = account.mailList[index];
       
		if(mail.lookOver==0)
			account.baseEntityCall.reqLookMail(mail.mailID);

		account.mailList[index].lookOver = 1;
		hallPanel.instance.showMailInfo(account);
		showMailInfoPanel(mail);

    }
    void showMailInfoPanel(MAIL mail){
        GameObject go = GameManager.GetInstance().LoadPanel("Prefabs/mailInfoPanel");
        go.GetComponent<mailInfoPanel>().showMailInfoPanel(mail);
		root.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack).OnKill(() => {
			DestroyObject(gameObject);
		});
	}
}
