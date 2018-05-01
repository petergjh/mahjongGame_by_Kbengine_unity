using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class MessagePanel : MonoBehaviour {
    Text titleText, ContentsText;
    public delegate void okCB();
    public delegate void cancelCB();
    Transform root,cancelBtn, OKBtn,closeBtn;
    okCB okcb;
    cancelCB _cancelCB;
    bool isClose;
    // Use this for initialization
    void Awake () {
		DontDestroyOnLoad (gameObject);
        root = transform.Find("root");
        root.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        cancelBtn = root.Find("cancelBtn");
        OKBtn = root.Find("OKBtn");
        closeBtn = root.Find("closeBtn");
        EventInterface.AddOnEvent(cancelBtn, Click);
        EventInterface.AddOnEvent(OKBtn, Click);
        EventInterface.AddOnEvent(closeBtn, Click);
        titleText = root.Find("titleText").GetComponent<Text>();
        ContentsText = root.Find("ContentsText").GetComponent<Text>();
    }
    public void _showMessagePanel( string Contents, string title = "提示", string cancelText = "关闭", string OKText="", cancelCB _cancelcb = null, okCB cb=null)
    {
        okcb = null;
        _cancelCB = null;
        _cancelCB = _cancelcb;
        okcb = cb;
        cancelBtn.Find("Text").GetComponent<Text>().text = cancelText;
        titleText.text = title;
        ContentsText.text = Contents;
        if (OKText != "") {
            OKBtn.gameObject.SetActive(true);
            cancelBtn.localPosition = new Vector3(-149, -97, 0);
            OKBtn.localPosition = new Vector3(149, -97, 0);
            OKBtn.Find("Text").GetComponent<Text>().text = OKText;
        }

    }
    void close() {
        isClose = true;
        root.DOScale(0, 0.3f).SetEase(Ease.InBack).OnKill(() =>
        {
            DestroyObject(gameObject);
        });
    }
	void Click(Transform tr) {
        if (isClose)
            return;
        audioManager.GetInstance().PlaySoundEffect("Music/buttonClick");
        if (tr == cancelBtn) {
            if (_cancelCB != null)
            {
                _cancelCB();
            }
        } else if (tr == OKBtn) {
            if (okcb != null) {
                okcb();
            }
        }
        close();

    }
}
