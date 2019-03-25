# -*- coding: utf-8 -*-
import KBEngine
from KBEDebug import *
import Functor
from interfaces.MailSystem import MailSystem

MAIN_STATE_IDEL = 1
MAIN_STATE_MATCH = 2
MAIN_STATE_INGAME = 3
class Account(KBEngine.Proxy,MailSystem):
	def __init__(self):
		KBEngine.Proxy.__init__(self)
		MailSystem.__init__(self)
		self.MainState = MAIN_STATE_IDEL
		self.roomKey = 0;
		self.playerName = self.cellData["playerName"]
		self.playerGold = self.cellData["playerGold"]
	def reqAddFriend(self,dbid):
		if dbid not in self.friendsList:
			self.friendsList.append(dbid);
			self.client.callClientMsg("添加成功")
		else:
			self.client.callClientMsg("你们已经是好友了")
	def GetPlayerInfo(self):
		_data={
			"playerName":self.playerName,
			"playerDBID":self.databaseID,
			"playerGold":self.playerGold
			}
		return _data;
	def onTimer(self, id, userArg):
		"""
		KBEngine method.
		使用addTimer后， 当时间到达则该接口被调用
		@param id		: addTimer 的返回值ID
		@param userArg	: addTimer 最后一个参数所给入的数据
		"""
		DEBUG_MSG(id, userArg)
		
		
	def onClientEnabled(self):
		"""
		KBEngine method.
		该entity被正式激活为可使用， 此时entity已经建立了client对应实体， 可以在此创建它的
		cell部分。
		"""
		INFO_MSG("account[%i] entities enable. entityCall:%s" % (self.id, self.client))
		KBEngine.globalData["AllPlayerPublicInfo"].register(self, self.databaseID)
		KBEngine.globalData["OfflineMessage"].register(self, self.databaseID)
		#self.initFriendsList()
		
	def initFriendsList(self):
		infoList = KBEngine.globalData["AllPlayerPublicInfo"].GetPlayersInfo(self.friendsList)
		if self.client:
			self.client.initFriendsListOK(infoList)

	def reqFriendsList(self):
		self.initFriendsList()

	def onLogOnAttempt(self, ip, port, password):
		"""
		KBEngine method.
		客户端登陆失败时会回调到这里
		"""
		INFO_MSG(ip, port, password)
		return KBEngine.LOG_ON_ACCEPT
		
	def onClientDeath(self):
		"""
		KBEngine method.
		客户端对应实体已经销毁
		"""
		DEBUG_MSG("Account[%i].onClientDeath:" % self.id)
		KBEngine.globalData["AllPlayerPublicInfo"].deregister(self, self.databaseID)
		#self.destroy()

	def reqCreateAvatar(self,name):
		#客户端请求创建一个角色
		#0：表示请求成功
		#1：已经有相同名字的玩家
		#2：创建失败！已经有一个角色了
		if self.isNewPlayer == 0:
			self.client.OnReqCreateAvatar(2)
			return
		
		porps={
			"playerName":name,
		}
		nameEntity = KBEngine.createEntityLocally("CheckName",porps)
		if nameEntity:
			nameEntity.writeToDB(Functor.Functor(self._OnNameSave,name))

	def _OnNameSave(self,name,success,avatar):
		if self.isDestroyed:
			if avatar:
				avatar.destroy()
			return
		if success:
			self.isNewPlayer = 0
			self.playerName_base = name;
			self.playerID_base = self.databaseID+10000
			self.playerName = name
			self.cellData["playerName"] = name
			self.cellData["playerID"] = self.playerID_base
			self.writeToDB()
			KBEngine.globalData["AllPlayerPublicInfo"].register(self, self.databaseID)
			if self.client:
				self.client.OnReqCreateAvatar(0)
		else:
			self.client.OnReqCreateAvatar(1)

	def onLeaveRoom(self):
		self.destroyCellEntity()

	def EnterMatchesMatch(self):
		if self.MainState != MAIN_STATE_IDEL:
			return
		self.MainState = MAIN_STATE_MATCH
		KBEngine.globalData["Halls"].EnterMatchesMatch(self)

	def createCell(self,roomCell):
		self.createCellEntity(roomCell)

	def onLoseCell(self):
		self.MainState = MAIN_STATE_IDEL
		self.inRoom = False
		self.playerGold = self.cellData["playerGold"]
		if self.client:
			self.client.playerLevelRoom()

	def enterRoomSuccess(self,roomKey):
		self.roomKey = roomKey
		self.inRoom = True

	def reqChangeRoom(self):
		KBEngine.globalData["Halls"].changeRoom(self,self.roomKey)

	#房间通知玩家换房间
	def OnTeleport(self,space):
		print("开始换房间，当前房间号---"+str(self.roomKey))
		self.teleport(space)
	#换房间成功回调
	def onTeleportSuccess(self):
		print("换房间成功，当前房间号---"+str(self.roomKey))

	def changeRoomSeatIndex(self,index):
		if self.cell:
			self.cell.changeRoomSeatIndex(index);
		else:
			self.cellData["roomSeatIndex"] = index