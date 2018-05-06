namespace KBEngine
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class Account : KBEngine.AccountBase
    {
		public override void OnReqCreateAvatar(byte arg1)
		{
			Event.fireOut("OnReqCreateAvatar", new object[] { (int)arg1 });
		}

		public override void __init__()
        {
            Event.fireOut("onLoginSuccessfully", new object[] { KBEngineApp.app.entity_uuid, id, this });
        }
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
