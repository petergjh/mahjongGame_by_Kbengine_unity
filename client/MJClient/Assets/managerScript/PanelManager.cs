using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using DG.Tweening;
public class PanelManager : MonoBehaviour {
	private static GameObject obj = null;
	private Transform pageRoot;//界面存放节点
	private static PanelManager instance = null;
 	public	List<GameObject> panelList= new List<GameObject>();
	public GameObject waitingPanel = null;
	public bool readResource;
	GameObject panelShowRoot;
	public static PanelManager GetInstance()
	{
		if (instance == null)
		{
			obj = new GameObject();
			obj.name = "PanelManager";
			instance = obj.AddComponent(typeof(PanelManager)) as PanelManager;
		}
		return instance;
	}
	
	void Awake()
	{
		//panelShowRoot = GameObject.Find ("Canvas_scene/PanelRoot");
		pageRoot = GameObject.Find("Canvas/Root").transform;
	}
	// Use this for initialization
	void Start () {
		
	}
	public  Transform GetPanel(string name){
		GameObject tr = findChild (name);
		if (tr == null) {
			//print ("没有发现对象----"+name);
			return null;
		} else
			return tr.transform;
//		Transform tr1;
//		if (tr == null) {
//			if (panelShowRoot == null) {
//				panelShowRoot = GameObject.Find ("Canvas_scene/PanelRoot");
//			}if (panelShowRoot == null) {
//				print ("没有找到panelShowRoot");
//				return null;
//			}				
//			tr1 = panelShowRoot.transform.Find (name);
//			if (tr1 != null) {
//				return tr1;
//			}
//			return null;
//		} else {
//			return tr.transform;
//		}
	}
	// Update is called once per frame
	void Update () {
	
	}
	GameObject findChild(string name)
	{
		if (pageRoot == null || !pageRoot.gameObject.activeSelf)
			pageRoot = GameObject.Find("Canvas/Root").transform;

		int childs1 = pageRoot.childCount;
		if (childs1 > 0) {
			for (int i = 0; i < childs1; i++) {
				if (pageRoot.GetChild (i).name == name) {
					//Transform tr = pageRoot.GetChild (i);
//					tr.SetParent (this.transform,false);
					return pageRoot.GetChild (i).gameObject;
				}
					
			}
		}
		int childs = this.transform.childCount;
		if (childs > 0) {
			for (int i = 0; i < childs; i++) {
				if (this.transform.GetChild (i).name == name)
					return this.transform.GetChild (i).gameObject;
			}
		}
		return null;
	}
	//获得主场景移动的目标点
	Vector3 GetTargetPosition(Transform tr){
		int BtnArea=0;
		int x = 1136;
		int y = 640;
		Vector3 vec = Vector3.zero;
		Vector2 trPO = tr.localPosition;
		if (trPO.x > -568 && trPO.x < (-568 + (x / 3)) && trPO.y > (320 - (y / 3)) && trPO.y < 320)
			BtnArea = 1;
		else if (trPO.x > (-568 + (x / 3)) && trPO.x < (-568 + 2 * (x / 3)) && trPO.y > (320 - (y / 3)) && trPO.y < (y / 2)) {
			BtnArea = 2;
		} else if (trPO.x > (-568 + 2 * (x / 3)) && trPO.x < ((x / 2)) && trPO.y > (320 - (y / 3)) && trPO.y < (y / 2)) {
			BtnArea = 3;
		} else if (trPO.x > (-(x / 2)) && trPO.x < (-568 + (x / 3)) && trPO.y > (320 - 2*(y / 3)) && trPO.y < (320 - (y / 3))) {
			BtnArea = 4;
		}else if (trPO.x > (-568 + (x / 3)) && trPO.x < (-568 + 2 * (x / 3)) && trPO.y > (320 - 2*(y / 3)) && trPO.y < (320 - (y / 3))) {
			BtnArea = 5;
		} else if (trPO.x > (-568 + 2 * (x / 3)) && trPO.x < ((x / 2)) && trPO.y > (320 - 2*(y / 3)) && trPO.y < (320 - (y / 3))) {
			BtnArea = 6;
		}else if (trPO.x > (-(x / 2)) && trPO.x < (-568 + (x / 3)) && trPO.y > (-320) && trPO.y < (320 - 2*(y / 3))) {
			BtnArea =7;
		}else if (trPO.x > (-568 + (x / 3)) && trPO.x < (-568 + 2 * (x / 3)) && trPO.y > (-320) && trPO.y < (320 - 2*(y / 3))) {
			BtnArea = 8;
		} else if (trPO.x > (-568 + 2 * (x / 3)) && trPO.x < ((x / 2)) && trPO.y > (-320) && trPO.y < (320 - 2*(y / 3))) {
			BtnArea = 9;
		}
		switch (BtnArea) {
		case 1:
			vec = new Vector3 (568, -320, 0);
			break;
		case 2:
			vec = new Vector3 (0, -320, 0);
			break;
		case 3:
			vec = new Vector3 (-568, -320, 0);
			break;
		case 4:
			vec = new Vector3 (568, 0, 0);
			break;
		case 5:
			vec = new Vector3 (0, 0, 0);
			break;
		case 6:
			vec = new Vector3 (-568, 0, 0);
			break;
		case 7:
			vec = new Vector3 (568, 320, 0);
			break;
		case 8:
			vec = new Vector3 (0, 320, 0);
			break;
		case 9:
			vec = new Vector3 (-568, 320, 0);
			break;
		}
		return vec;
	}

	//加载界面ui
	public  Object Load(string path,string name,bool HeidLastPanel = true)
	{
		int count = panelList.Count;
		if (string.IsNullOrEmpty(path+name))
			return null;
		GameObject realgo=findChild(name);
		if (realgo == null) {
			realgo = Resources.Load (path + name) as GameObject;
			realgo = GameObject.Instantiate (realgo);
		}
		if (pageRoot == null || !pageRoot.gameObject.activeSelf)
			pageRoot = GameObject.Find("Canvas_scene/PanelRoot").transform;
		realgo.transform.SetParent(this.transform, false);
		realgo.transform.SetParent(pageRoot, false);
		realgo.name = name;
		//显示动画？
//		if (count > 0) {
//			//至少有1个界面，要打开一个
//			GameObject NowHeldPanel = panelList[count-1];
//			if (realgo.name.Contains ("guizi") && NowHeldPanel.name.Contains ("scene") && realgo.GetComponent<guiziContl> ().openAnim) {
//				realgo.gameObject.SetActive (false);
//				Vector3 targetPosition = GetTargetPosition (NowHeldPanel.GetComponent<scenePanel_common> ().NewClickBtn);
//				audioManager.GetInstance ().PlaySoundEffect("music/qianj");
//				//如果有关闭效果时显示
//				NowHeldPanel.GetComponent<scenePanel_common> ().openMask ();
//				Image im = NowHeldPanel.GetComponent<Image> ();
//				if (im != null) {
//					NowHeldPanel.transform.DOScale (new Vector3 (2, 2, 2), GameManager.sceneAnimTime);//.OnKill (() => {
//						realgo.gameObject.SetActive (true);
//					//});
//					NowHeldPanel.transform.DOLocalMove (targetPosition, GameManager.sceneAnimTime);
//					//im.DOColor (new Color32 (77, 77, 77, 255), GameManager.sceneAnimTime);
//					//im.DOFade (GameManager.sceneAnimTime, GameManager.sceneAnimTime);
//				}
//			} else {
				realgo.gameObject.SetActive (false);
				realgo.gameObject.SetActive (true);
//			}
//		}
		if (HeidLastPanel && (count) > 0) {
			panelList [count - 1].transform.SetParent (transform, false);
		}
		//存入ui列表
		panelList.Add (realgo);
//		if (realgo.GetComponent<scenePanel_common> () != null) {
//			GameManager.cur_scene = realgo.GetComponent<scenePanel_common> ().Cur_scene;
//			print (GameManager.cur_scene);
//		}
		return realgo;
	}
	void openAniOver(Transform tr){
		tr.gameObject.SetActive (true);
	}
	//加载界面ui
	public Object LoadOther(string path,Transform parent)
	{
		if (string.IsNullOrEmpty(path))
			return null;
		GameObject realgo=findChild(path);
		if (realgo == null) {
			realgo =Resources.Load(path) as GameObject;
			realgo = GameObject.Instantiate(realgo);
		}
		realgo.transform.SetParent(parent, false);
		realgo.gameObject.SetActive (true);
		realgo.name = name;
		return realgo;	
	}
	public void lastPanel(bool showInit=false,bool dele=false)
	{
		int count = panelList.Count;
		string str;
		GameObject NowShowPanel,closePanel;
		if ( count > 1) {
			str = panelList [count - 1].name;
			closePanel = panelList [count - 1];
			NowShowPanel = panelList [count - 2];
//			if (closePanel.name.Contains ("guizi") && NowShowPanel.name.Contains ("scene") && closePanel.GetComponent<guiziContl> ().openAnim) {
//				//如果有关闭效果时显示
//				audioManager.GetInstance ().PlaySoundEffect ("music/qianj");
//				Image im = NowShowPanel.GetComponent<Image> ();
//				if (im != null) {
//					NowShowPanel.transform.DOScale (new Vector3 (1, 1, 1), GameManager.sceneAnimTime).OnKill(() => {
//						closePanelMethod (count,dele);
//					});
//					NowShowPanel.transform.DOLocalMove (Vector3.zero, GameManager.sceneAnimTime);
//					//im.DOFade (1, GameManager.sceneAnimTime);
//					//im.DOColor (new Color (1, 1, 1, 255), GameManager.sceneAnimTime);
//				}
//			} else {
			if(showInit)
				NowShowPanel.gameObject.SetActive(false);
			NowShowPanel.gameObject.SetActive(true);
			NowShowPanel.transform.SetParent (pageRoot,false);
			closePanelMethod (count,dele);
			//}

		}
	}
	public void heidPanel(bool dele=false){
		int count = panelList.Count;
		string str;
		closePanelMethod (count);
	}
	void closePanelMethod(int count ,bool dele=false){
		if (dele) {
			DestroyObject (panelList [count - 1]);
		} else {
			panelList [count - 1].transform.SetParent (this.transform, false);
			panelList [count - 1].gameObject.SetActive (false);
		}
		panelList.Remove (panelList[count - 1]);
	}
	public void showWaiting(int second,Transform parent){
		if (waitingPanel != null) {
			Destroy (waitingPanel);
			waitingPanel = null;
		}
		GameObject realgo = Resources.Load ("Prefabs/public/waiting")as GameObject;
		realgo = GameObject.Instantiate(realgo);
		realgo.transform.SetParent (parent, false);
		realgo.gameObject.SetActive (true);
		waitingPanel = realgo;
		if (second != 0) {
			StartCoroutine (closeCoro(second));
		} 
	}
	public void closeWaiting(){
		if (waitingPanel != null) {
			Destroy (waitingPanel);
			waitingPanel = null;
		}
		//GameManager.GetInstance().closeWaiting_ ();
	}
	IEnumerator closeCoro(int second ){
		yield return new WaitForSeconds (second);
		closeWaiting ();
	}
}
