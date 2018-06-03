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