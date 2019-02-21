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

	def Receive_message(self,mail):
		self._lastmailID+=1
		mail["mailID"] = self._lastmailID
		self.mailList.append(mail)

	def reqLookMail(self,index):
		for mail in self.mailList:
			if mail["mailID"] == index:
				mail["mailID"] = True;
				break

	def reqDeleMail(self,index):
		for mail in self.mailList:
			if mail["mailID"] == index:
				self.mailList.remove(mail)
				break

