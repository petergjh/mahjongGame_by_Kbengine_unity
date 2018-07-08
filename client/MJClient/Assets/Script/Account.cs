namespace KBEngine
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Account : KBEngine.AccountBase
    {
		public override void onGetRoomInfo(ROOM_PUBLIC_INFO arg1)
		{
			
		}

		public override void OnReqCreateAvatar(byte arg1)
		{
			Event.fireOut("OnReqCreateAvatar", new object[] { (int)arg1 });
		}

		public override void playerLevelRoom()
		{
			Event.fireOut("playerLevelRoom");
		}

		public override void __init__()
        {
            Event.fireOut("onLoginSuccessfully", new object[] { KBEngineApp.app.entity_uuid, id, this });
        }
		public override void onIsReadyChanged(byte old)
		{
			byte v = this.isReady;
			MonoBehaviour. print(v+"");
			Event.fireOut("set_isReady", new object[] { this, (int)v });
		}
	}
}
