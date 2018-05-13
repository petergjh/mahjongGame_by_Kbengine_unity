import KBEngine
from KBEDebug import *

FEN_PEI_TIMER = 1
class Halls(KBEngine.Entity):
	def __init__(self):
		KBEngine.Entity.__init__(self)
		KBEngine.globalData["Halls"] = self
		self.waitingEnterPlayerEntitys = []
		self.fen_pei_timer == 0

	def EnterMatchesMatch(self,entityCall):
		if entityCall in self.waitingEnterPlayerEntitys:
			return
		self.waitingEnterPlayerEntitys.append(entityCall)
		if self.fen_pei_timer==0:
			self.fen_pei_timer = self.addTimer(0,0.5,FEN_PEI_TIMER)

	def CreatPrivRoom(self,entityCall):
		pass

	def joinRoom(self,entityCall,roomId):
		pass

	def fen_pei(self):
		if len(self.waitingEnterPlayerEntitys) == 0 and self.fen_pei_timer != 0:
			self.delTimer(self.fen_pei_timer)
			self.fen_pei_timer = 0
			return

		#处理玩家加入匹配房间

	def onTimer(self, timerHandle, userData):
		if userData == FEN_PEI_TIMER:
			self.fen_pei()