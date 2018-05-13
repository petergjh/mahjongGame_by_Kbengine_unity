import KBEngine
from KBEDebug import *

class Room(KBEngine.Entity):
	def __init__(self):
		KBEngine.Entity.__init__(self)
		self.createCellEntityInNewSpace(None)
		self.roomKey = self.cellData["roomKey"]
		self.MaxPlayerCount = self.cellData["playerMaxCount"]
		self.RoomType = self.cellData["RoomType"]

	def NeedPlayersCount(self):
		if self.isDestroyed:
			return 0

		return self.MaxPlayerCount - len(self.EnterPlayerList)

	def enterRoom(self,entityCall):
		if entityCall not in self.EnterPlayerList:
			self.EnterPlayerList.append(entityCall)
		if len(self.EnterPlayerList) == self.MaxPlayerCount and self.RoomType==0:
			KBEngine.globalData["Halls"].roomIsFull(self,self.roomKey)

		if self.cell is not None:
			#向cell投送玩家
			#self.cell.
			pass

