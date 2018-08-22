using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KBEngine;
public class test : MonoBehaviour {
	public InputField input;
	public Transform sendPaiBtn,showActionDataBtn,pengBtn;
	// Use this for initialization
	void Start () {
		EventInterface.AddOnEvent(sendPaiBtn, Click);
		EventInterface.AddOnEvent(showActionDataBtn, Click);
		EventInterface.AddOnEvent(pengBtn, Click);
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
		} else if (tr == pengBtn) {
			rm.cellEntityCall.reqPeng();
		}
		
		
	}
}
