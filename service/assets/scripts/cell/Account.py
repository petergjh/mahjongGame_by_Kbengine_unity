import KBEngine
from KBEDebug import *
class Account(KBEngine.Entity):
	def __init__(self):
		KBEngine.Entity.__init__(self)

	def LeaveRoom(self,callerEntityID):
		"""
		离开房间
		"""	
		if callerEntityID!=self.id:
			return
		KBEngine.globalData["Room_%i" % self.spaceID].ReqLeaveRoom(self)

	def changeRoomSeatIndex(self,index):
		self.roomSeatIndex = index

	def playerReadyStateChange(self,state):
		self.isReady = state
		print("cell account")
		print(self.isReady)

	def game_holds_push(self,holds):
		self.holds = holds

	#麻将逻辑通知你有操作了
	def game_action_push(self,actionData):
		self.actionData = actionData
		if actionData.get("pai")!=-1:
			if self.client:
				self.client.has_action()

	def gang_notify_push(self,pai,gangtype):
		if gangtype == "angang":
			for i in range(4):
				self.holds.remove(pai)
			
		elif gangtype == "diangang":
			for i in range(3):
				self.holds.remove(pai)
		elif gangtype == "wangang":
			self.holds.remove(pai)

		self.allClients.onGang(self.id,pai,gangtype)

	#麻将逻辑通知你胡牌成功
	def hu_push(self,isZimo,hupai):
		if iszimo:
			self.holds.remove(hupai)
		self.allClients.onHu(self.id,iszimo,hupai)