# -*- coding: utf-8 -*-
import KBEngine
from KBEDebug import *
class MailSystem:

	def __init__(self):
		pass

	def reqSendMail(self,targetDBID,targetName,mailType,mailInfo):
		mail = {
				"senderDBID": self.databaseID,
				"senderName":self.playerName,
				"targetDBID":targetDBID,
				"targetName":targetName,
				"lookOver":False,
				"mailType":mailType,
				"mailInfo":mailInfo,
				"mailID":0,
			}
		playerlist = [targetDBID]
		infoList = KBEngine.globalData["AllPlayerPublicInfo"].GetPlayersInfo(playerlist)
		if len(infoList)>0 and infoList[0]["isOnLine"]==True:
			infoList[0]["entity"].Receive_message(mail)
		else:
			KBEngine.globalData["OfflineMessage"].send_message(mail)


	def addFriendByMail(self,dbid):
		if dbid not in self.friendsList:
			self.friendsList.append(dbid);

	def Receive_message(self,mail):
		self._lastmailID+=1
		mail["mailID"] = self._lastmailID
		self.mailList.append(mail)
		self.client.addMail(mail)
		if mail["mailType"] == 2:
			self.addFriendByMail(mail["senderDBID"]);
		self.writeToDB()


	def reqLookMail(self,index):
		for mail in self.mailList:
			if mail["mailID"] == index:
				mail["lookOver"] = True;
				break

	def reqDeleMail(self,index):
		rem = -1
		for i in range(len(self.mailList)):
			print(str(self.mailList[i]["mailID"]))
			if self.mailList[i]["mailID"] == index:
				rem = i
				break

		if rem >=0:
			self.mailList.pop(rem)
			self.client.deleMail(index)
		
	def reqAgreeAddFriendByMail(self,index):
		for mail in self.mailList:
			if mail["mailID"] == index and  mail["mailType"]==3:
				self.addFriendByMail(mail["senderDBID"]);
				self.reqSendMail(mail["senderDBID"],mail["senderName"],2,self.playerName)
				self.reqDeleMail(index)
				break
				

	def reqGiveGold(self,dbid,targetName):
		gold = self.cellData["playerGold"]
		if gold>=100:
			gold-=100
			self.cellData["playerGold"] = gold
			self.reqSendMail(dbid,targetName,0,"")
			self.client.callClientMsg("赠送金币成功！")
		else:
			self.client.callClientMsg("金币少于100，赠送失败！")

	def reqGetOtherGiveGold(self,index):
		for mail in self.mailList:
			if mail["mailID"] == index and  mail["mailType"]==0:
				self.cellData["playerGold"]+=100;
				self.reqDeleMail(index)
				self.client.callClientMsg("收到好友赠送的100金币")
				break