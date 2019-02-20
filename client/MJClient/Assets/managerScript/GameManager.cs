using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using KBEngine;
public class GameManager : MonoBehaviour
{
    private static GameObject obj = null;
    private static GameManager instance = null;
    public delegate void showWaitingCB();
    GameObject waitingOBJ, messageBoxOBJ;
    int waitingShowCbTime;
    showWaitingCB SWCB;
    public static bool isInGame = false;
    public static List<Int32> pendingEnterEntityIDs = new List<Int32>();
    public Entity roomEntity;
    public string curSceneName = "Login";
    public static GameManager GetInstance()
    {
        if (instance == null)
        {
            obj = new GameObject();
            obj.name = "GameManager";
            instance = obj.AddComponent(typeof(GameManager)) as GameManager;
        }
        return instance;
    }
    // Use this for initialization
    void Awake()
    {

        DontDestroyOnLoad(gameObject);
		installEvents();


	}
	void installEvents()
	{
		KBEngine.Event.registerOut("addSpaceGeometryMapping", this, "addSpaceGeometryMapping");
		KBEngine.Event.registerOut("onEnterWorld", this, "onEnterWorld");
		KBEngine.Event.registerOut("onLeaveWorld", this, "onLeaveWorld");
		KBEngine.Event.registerOut("callClientMsg", this, "callClientMsg");
		
	}
	public void callClientMsg(string msg) {
		GameManager.GetInstance().showMessagePanel(msg);
	}
	public void onEnterWorld(KBEngine.Entity entity)
	{
		if (MJpanel.instance == null||MJpanel.roomIsIn == false)
		{
			pendingEnterEntityIDs.Add(entity.id);
		}
		else {
			MJpanel.instance.EnterWorld(entity);
		}
	}
	public void onLeaveWorld(KBEngine.Entity entity) {
		if (MJpanel.instance!=null &&  MJpanel.roomIsIn )
		{
			MJpanel.instance.LeaveWorld(entity);
		}
		else {
			if (pendingEnterEntityIDs.Contains(entity.id)) {
				pendingEnterEntityIDs.Remove(entity.id);
			}

		}	

	}
	public bool gameSceneLoadeOver() {
		//无论如何先让房间先进场
		bool hasRoom = false;
		List<int> newEntityIDS = new List<int>();
		newEntityIDS.Add(0);
		foreach (Int32 id in pendingEnterEntityIDs)
		{
			KBEngine.Entity entity = KBEngineApp.app.findEntity(id);
			if (entity.className == "Room")
			{
				newEntityIDS[0] = id;
				hasRoom = true;
			}
			else {
				newEntityIDS.Add(id);
			}
			
		}
		if (!hasRoom)
			return false;

		pendingEnterEntityIDs = newEntityIDS;
		foreach (Int32 id in pendingEnterEntityIDs)
		{
			print(id);
			KBEngine.Entity entity = KBEngineApp.app.findEntity(id);
			if (entity != null)
			{
				MJpanel.instance.EnterWorld(entity);
			}
		}
		pendingEnterEntityIDs.Clear();

		return true;
	}
	void OnDestroy()
    {
		
	}
	public void addSpaceGeometryMapping(string path)
	{
		print(path);
		switch (path) {
			case "spaces/mjRoom":
				UnityEngine.SceneManagement.SceneManager.LoadSceneAsync("gameScene");
				break;
		}
	}
	// 显示等待界面
	public void showWaiting(string showInfoMessage = "", int showTime = 30, showWaitingCB cb = null)
    {
        //closeWaitingPanel();
        print(showInfoMessage);
        if (waitingOBJ != null && waitingShowCbTime != 0)
        {
            timerManager.GetInstance().cancelTimer(waitingShowCbTime);
        }
        else
        {
            waitingOBJ = LoadPanel("Prefabs/waitingPanel");
        }
        waitingShowCbTime = timerManager.GetInstance().addTimer(showTime, waitingShowTimeOut);
        SWCB = null;
        SWCB = cb;
        if (showInfoMessage != "")
        {
            waitingOBJ.SendMessage("ShowInfo", showInfoMessage);
        }
    }
    public void closeWaitingPanel()
    {
        if (waitingOBJ != null)
        {
            if (waitingShowCbTime != 0)
            {
                timerManager.GetInstance().cancelTimer(waitingShowCbTime);
            }
            DestroyObject(waitingOBJ);

        }
    }
    void waitingShowTimeOut(int id, object obj)
    {
        if (waitingShowCbTime == id)
        {
            closeWaitingPanel();
            if (SWCB != null)
            {
                SWCB();
            }
        }
    }
    //弹窗
    public void showMessagePanel(string Contents)
    {
        ShowMessageBox(Contents, "提示", "确定", "");
    }
    public void showMessagePanel(string Contents, MessagePanel.cancelCB cb = null)
    {
        print(Contents);
        ShowMessageBox(Contents, "提示", "确定", "", cb);
    }
    public void showMessagePanel(string Contents, string title = "提示", string cancelText = "关闭", string OKText = "", MessagePanel.cancelCB cb1 = null, MessagePanel.okCB cb = null)
    {
        ShowMessageBox(Contents, title, cancelText, OKText, cb1, cb);
    }
    void ShowMessageBox(string Contents, string title = "提示", string cancelText = "关闭", string OKText = "", MessagePanel.cancelCB cb1 = null, MessagePanel.okCB cb = null)
    {
        if (messageBoxOBJ != null)
            Destroy(messageBoxOBJ);

        messageBoxOBJ = LoadPanel("Prefabs/MessagePanel");
        messageBoxOBJ.GetComponent<MessagePanel>()._showMessagePanel(Contents, title, cancelText, OKText, cb1, cb);
    }
    //加载panel
    public GameObject LoadPanel(string path, Transform parent = null)
    {
        if (parent == null)
        {
            parent = GameObject.Find("Canvas/UI").transform;
        }
        GameObject go = Resources.Load(path) as GameObject;
        go = Instantiate(go);
        go.transform.SetParent(parent, false);
        return go;
    }
}