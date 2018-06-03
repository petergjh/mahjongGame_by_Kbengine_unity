import KBEngine
from KBEDebug import *
class Room(KBEngine.Entity):
	def __init__(self):
		KBEngine.Entity.__init__(self)
		KBEngine.addSpaceGeometryMapping(self.spaceID,None,"spaces/mjRoom")
		KBEngine.globalData["Room_%i" % self.spaceID] = self
		self.roomInfo = roomInfo(self.roomKey,self.playerMaxCount)

	def enterRoom(self,EntityCall):
		for i in range(len(self.roomInfo.seats)):
			seat = self.roomInfo.seats[i];
			if seat.userId == 0:
				seat.userId = EntityCall.id;
				seat.score = 1000   #带入分数
				self.base.CanEnterRoom(EntityCall)
				return

	
	def ReqLeaveRoom(self,EntityCall):
		#通知玩家base销毁cell
		EntityCall.base.onLeaveRoom()
		#让base向大厅要人
		self.base.leaveRoom(EntityCall.id)
		#清除该玩家坐过的椅子数据
		self.roomInfo.clearDataByEntityID(EntityCall.id)



#----------------------------------------------------------------------------
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
		self.score = 0
		self.ready = False
		self.seatIndex = seatIndex