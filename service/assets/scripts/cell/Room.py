import KBEngine
from KBEDebug import *
class Room(KBEngine.Entity):
	def __init__(self):
		KBEngine.Entity.__init__(self)
		KBEngine.addSpaceGeometryMapping(self.spaceID,None,"spaces/mjRoom")
		KBEngine.globalData["Room_%i" % self.spaceID] = self
		self.roomInfo = roomInfo(self.roomKey,self.playerMaxCount)
		self.game = None
		self.clearPublicRoomInfo()
		

	def enterRoom(self,EntityCall):
		for i in range(len(self.roomInfo.seats)):
			seat = self.roomInfo.seats[i];
			if seat.userId == 0:
				seat.userId = EntityCall.id;
				seat.entity = EntityCall
				seat.score = 1000   #带入分数
				print("玩家进来了---"+str(seat.userId)+" 座位号为 "+str(i))
				EntityCall.changeRoomSeatIndex(i)
				self.base.CanEnterRoom(EntityCall)
				EntityCall.enterRoomSuccess(self.roomKey)
				return

	def changeRoomSuccess(self,entityID):
		self.roomInfo.clearDataByEntityID(entityID)


	def ReqLeaveRoom(self,EntityCall):
		#通知玩家base销毁cell
		EntityCall.base.onLeaveRoom()
		#让base向大厅要人
		self.base.leaveRoom(EntityCall.id)
		#清除该玩家坐过的椅子数据
		self.roomInfo.clearDataByEntityID(EntityCall.id)

	def setPublicRoomInfo(self):
		playerList = []
		for i in range(self.playerMaxCount):
			seatData =self.game.gameSeats[i]
			d={
				"userId":seatData.userId
				}
			playerList.append(d)
		data = {
			"state" :self.game.state,
			"playerInfo":playerList,
			}
		self.public_roomInfo = data

	##清空游戏共享数据
	def clearPublicRoomInfo(self):
		playerList = []
		for i in range(self.playerMaxCount):
			d={
				"userId":0
				}
			playerList.append(d)
		data = {
			"state" :"idel",
			"playerInfo":playerList,
			}
		self.public_roomInfo = data

	def reqGetRoomInfo(self,callerEntityID):
		for i in range(len(self.roomInfo.seats)):
			seat = self.roomInfo.seats[i];
			if seat.userId ==callerEntityID:
				if(seat.entity.client):
					seat.entity.client.onGetRoomInfo(self.public_roomInfo)
	
					
	def reqChangeReadyState(self,callerEntityID,STATE):
		print(callerEntityID)
		for i in range(len(self.roomInfo.seats)):
			seat = self.roomInfo.seats[i];
			if seat.userId ==callerEntityID:
				seat.ready = not STATE
				seat.entity.cell.playerReadyStateChange(seat.ready)
				print(seat.ready)
#----------------------------------------------------------------------------
#麻将信息类
class MJData:
	def __init__(self,roomInfo,maxPlayerCount):
		self.state = "idle"
		self.seatList = roomInfo.seats


#房间信息
class roomInfo:
	def __init__(self,roomKey,maxPlayerCount):
		self.id = roomKey
		self.seats = []
		for i in range(maxPlayerCount):
			seat = seat_roomInfo(i)
			self.seats.append(seat)

	def clearData(self):
		for i in range(len(self.seats)):
			self.clearDataBySeat(i,False)

	def clearDataBySeat(self,index,isOut = True):
		s = self.seats[index]
		if isOut:
			s.userId = 0
			s.entity = None
		s.ready = False
		s.score = 0
		s.seatIndex = index

	def clearDataByEntityID(self,entityID,isOut = True):
		for i in range(len(self.seats)):
			if self.seats[i].userId == entityID:
				self.clearDataBySeat(i,isOut)
				break



#椅子信息
class seat_roomInfo:
	def __init__(self,seatIndex):
		self.userId = 0
		self.entity = None
		self.score = 0
		self.ready = False
		self.seatIndex = seatIndex