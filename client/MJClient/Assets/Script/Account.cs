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

		public override void game_begin_push()
		{
			MonoBehaviour.print("game_begin_push()");
			Event.fireOut("gameStart");
		}

		public override void upDataClientRoomInfo(ROOM_PUBLIC_INFO arg1)
		{
			MonoBehaviour.print("upDataClientRoomInfo(ROOM_PUBLIC_INFO arg1)");
			MonoBehaviour.print(arg1.state);
			MonoBehaviour.print(arg1.playerInfo[0].holdsCount+"");
			MonoBehaviour.print(arg1.button);
			MonoBehaviour.print(arg1.turn);
		}

		public override void game_chupai_push()
		{
			MonoBehaviour.print(playerName+"--到你出牌了");
		}

		public override void game_mopai_push(sbyte arg1)
		{
			MonoBehaviour.print(playerName + "--摸到一张牌 --"+ arg1);
		}

		public override void has_action()
		{
			MonoBehaviour.print(playerName + "--有操作了");
		}

		public override void onGang(uint arg1, sbyte arg2, string arg3)
		{
			MonoBehaviour.print("杠成功 userID=="+arg1 +"  牌--"+arg2+" 类型=="+ arg3 );
		}

		public override void onHu(uint arg1, byte arg2, sbyte arg3)
		{
			MonoBehaviour.print("胡成功 userID==" + arg1 + "  是否自摸--" + arg2 + " 牌==" + arg3);
		}

		public override void onPlayCard(uint arg1, sbyte arg2)
		{
			MonoBehaviour.print(" userID==" + arg1 + "  打出一张牌--" + arg2 );
		}

		public override void onPlayCardOver(uint arg1, sbyte arg2)
		{
			MonoBehaviour.print(" userID==" + arg1 + "  打出成功，已经加入到该玩家的打出列表，牌为--" + arg2);
		}

		public override void peng_notify_push(uint arg1,sbyte arg2)
		{
			MonoBehaviour.print(" userID==" + arg1 + "  碰牌成功，牌为--" + arg2);
		}

		public override void otherPlayerMopai(uint arg1)
		{
			MonoBehaviour.print(" userID==" + arg1 + "  摸到一张牌");
		}
		public void showActionData() {
			
			MonoBehaviour.print("是否有杠=="+actionData.gang);
			if (actionData.gang == 1) {
				MonoBehaviour.print("杠牌列表==");
				foreach (var pai in actionData.gangpai) {
					MonoBehaviour.print("==" + pai);
				}
			}
			MonoBehaviour.print("是否有碰==" + actionData.peng);
			MonoBehaviour.print("是否有胡==" + actionData.hu);
			

		}

		public override void hasTing(TING_PAI_LIST arg1)
		{
			MonoBehaviour.print("有听牌信息了");
			showTingPaiInfo();
		}
		public void showTingPaiInfo() {
			foreach (TING_PAI_DIC td in TingPaiList) {
				MonoBehaviour.print("----------------------  ");
				MonoBehaviour.print("打掉它就可以胡了---  "+ td.nousepai);
				MonoBehaviour.print("正在听的牌 ---  " + td.pai);
				MonoBehaviour.print("番数 ---  " + td.fan);
			}
		}
	}
}
