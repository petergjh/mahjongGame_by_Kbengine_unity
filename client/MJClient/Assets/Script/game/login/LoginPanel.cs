using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using KBEngine;
public class LoginPanel : MonoBehaviour {
    InputField AccountNameInput, PassWordInput;
    Transform loginBtn;
    // Use this for initialization
    void Start () {
        AccountNameInput = transform.Find("AccountNameInput").GetComponent<InputField>();
        PassWordInput = transform.Find("PassWordInput").GetComponent<InputField>();
        loginBtn = transform.Find("loginBtn");
        EventInterface.AddOnEvent(loginBtn,Click);
        installEvents();
    }
    void installEvents()
    {
        KBEngine.Event.registerOut("onLoginFailed", this, "onLoginFailed");
        KBEngine.Event.registerOut("onVersionNotMatch", this, "onVersionNotMatch");
        KBEngine.Event.registerOut("onScriptVersionNotMatch", this, "onScriptVersionNotMatch");
        KBEngine.Event.registerOut("onLoginGatewayFailed", this, "onLoginGatewayFailed");
        KBEngine.Event.registerOut("onLoginSuccessfully", this, "onLoginSuccessfully");
		KBEngine.Event.registerOut("OnReqCreateAvatar", this, "OnReqCreateAvatar");
		KBEngine.Event.registerOut("login_baseapp", this, "login_baseapp");
        KBEngine.Event.registerOut("Loginapp_importClientMessages", this, "Loginapp_importClientMessages");
        KBEngine.Event.registerOut("Baseapp_importClientMessages", this, "Baseapp_importClientMessages");
        KBEngine.Event.registerOut("Baseapp_importClientEntityDef", this, "Baseapp_importClientEntityDef");
        KBEngine.Event.registerOut("onLoginBaseappFailed", this, "onLoginBaseappFailed");
    }
    void OnDestroy()
    {
        KBEngine.Event.deregisterOut(this);
    }
    public void onLoginFailed(UInt16 failedcode)
    {
        GameManager.GetInstance().closeWaitingPanel();
        if (failedcode == 6)
        {
            GameManager.GetInstance().showMessagePanel("账号或" + KBEngineApp.app.serverErr(failedcode) + "!");
            print("账号或" + KBEngineApp.app.serverErr(failedcode) + "!");
        }
        else
        {
            GameManager.GetInstance().showMessagePanel("登陆失败！" + KBEngineApp.app.serverErr(failedcode) + "!");
            print("登陆失败！" + KBEngineApp.app.serverErr(failedcode) + "!");
        }
    }
    public void onCreateAccountResult(UInt16 retcode, byte[] datas)
    {
        GameManager.GetInstance().closeWaitingPanel();
        if (retcode != 0)
        {
            GameManager.GetInstance().showMessagePanel(KBEngineApp.app.serverErr(retcode) + "!");
            print(KBEngineApp.app.serverErr(retcode) + "!");
            //注册失败后，把输入框重置下，并返回登陆界面
            return;
        }
        GameManager.GetInstance().showMessagePanel("注册成功！");
        print("注册成功！");

    }
    public void onVersionNotMatch(string verInfo, string serVerInfo)
    {
        GameManager.GetInstance().closeWaitingPanel();
        GameManager.GetInstance().showMessagePanel("与服务端版本(" + serVerInfo + ")不匹配, 当前版本(" + verInfo + ")!");
        print("与服务端版本(" + serVerInfo + ")不匹配, 当前版本(" + verInfo + ")!");
    }

    public void onScriptVersionNotMatch(string verInfo, string serVerInfo)
    {
        GameManager.GetInstance().closeWaitingPanel();
        GameManager.GetInstance().showMessagePanel("与服务端脚本版本(" + serVerInfo + ")不匹配, 当前版本(" + verInfo + ")!");
        print("与服务端脚本版本(" + serVerInfo + ")不匹配, 当前版本(" + verInfo + ")!");
    }

    public void onLoginGatewayFailed(UInt16 failedcode)
    {
        GameManager.GetInstance().closeWaitingPanel();
        GameManager.GetInstance().showMessagePanel("登陆网关服务器失败");
        print("登陆网关服务器失败, 错误:" + KBEngineApp.app.serverErr(failedcode) + "!");
    }

    public void login_baseapp()
    {
        GameManager.GetInstance().showWaiting("请求连接到网关服务器...");
        print("请求连接到网关服务器...");
    }
        //自定义方法
    public void onLoginSuccessfully(UInt64 rndUUID, Int32 eid, KBEngine.Account accountEntity)
    {
        
        GameManager.GetInstance().showWaiting("登陆成功!", 1);
		print("isNewPlayer ="+accountEntity.isNewPlayer);
		print("playerName_base=" + accountEntity.playerName_base);
		print("playerID_base=" + accountEntity.playerID_base);
		//if (accountEntity.isNewPlayer == 1) {
			accountEntity.baseEntityCall.reqCreateAvatar("撒旦");
		//}
	}
	public void OnReqCreateAvatar(int code) {
		print("OnReqCreateAvatar "+code);
		if (code == 0) {
			Account ac = (Account)KBEngineApp.app.player();
			print("isNewPlayer=" + ac.isNewPlayer);
			print("playerName_base=" + ac.playerName_base);
			print("playerID_base=" + ac.playerID_base);
		}else if(code == 1){
			print("和服务器上的玩家重名了");
		}
		else if (code == 2)
		{
			print("你已经有一个角色了  不需要在创建了");
		}
	}


	public void Loginapp_importClientMessages()
    {
        GameManager.GetInstance().showWaiting("请求建立登录通信协议...");
        print("请求建立登录通信协议...");
    }

    public void Baseapp_importClientMessages()
    {
        GameManager.GetInstance().showWaiting("请求建立网关通信协议...");
        print("请求建立网关通信协议...");
    }

    public void Baseapp_importClientEntityDef()
    {
        GameManager.GetInstance().showWaiting("请求导入脚本...");
        print("请求导入脚本...");
    }
    public void onLoginBaseappFailed(UInt16 failedcode)
    {
        if (failedcode == 8)
        {
            GameManager.GetInstance().closeWaitingPanel();
            GameManager.GetInstance().showMessagePanel(KBEngineApp.app.serverErr(failedcode) + "!");
            print(KBEngineApp.app.serverErr(failedcode) + "!");
        }
    }
    // Update is called once per frame
    void Click(Transform tr) {
        if (tr == loginBtn) {
            login();
        }
	}
    //登录账号
    void login()
    {
        if (AccountNameInput.text == "" || PassWordInput.text == "")
        {
            GameManager.GetInstance().showMessagePanel("账号或密码不能为空！", () =>
            {
                AccountNameInput.text = "";
                PassWordInput.text = "";
            });
            return;
        }
        //发起登录消息
        GameManager.GetInstance().showWaiting("正在连接中....", 10, () => {
            GameManager.GetInstance().showMessagePanel("连接超时！");
        });
        KBEngine.Event.fireIn("login", AccountNameInput.text, PassWordInput.text, System.Text.Encoding.UTF8.GetBytes(Application.platform.ToString()));
    }
}
