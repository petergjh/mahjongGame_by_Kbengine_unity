import KBEngine
from KBEDebug import *
import Functor
import random

ROOM_MAX_PLAYER = 2
FEN_PEI_TIMER = 1
class Halls(KBEngine.Entity):
	def __init__(self):
		KBEngine.Entity.__init__(self)
		KBEngine.globalData["Halls"] = self
		self.waitingEnterPlayerEntitys = []
		self.fen_pei_timer = 0
		self.NeedPlayerRoomEntity = {}
		self.allRoomEntityList = {}
		print("有好友列表个数---"+str(len(self.friendsList))+"个")

	def EnterMatchesMatch(self,entityCall):
		if entityCall in self.waitingEnterPlayerEntitys:
			print("已经在匹配队列中了。。。。")
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
		playerCount = len(self.waitingEnterPlayerEntitys)
		if playerCount>0:
			deleRoomList = []
			print("123123123------")
			for roomId,entity in self.NeedPlayerRoomEntity.items():
				print(len(self.NeedPlayerRoomEntity))
				freeSet = entity.NeedPlayersCount()
				if freeSet>0:
					if playerCount < freeSet:
						for i in range(playerCount):
							entity.enterRoom(self.waitingEnterPlayerEntitys.pop(0))
							playerCount-=1
					else:
						for i in range(freeSet):
							entity.enterRoom(self.waitingEnterPlayerEntitys.pop(0))
							playerCount-=1
						deleRoomList.append(roomId)

					if playerCount == 0:
						return


			print("结束后长度")
			print(len(self.NeedPlayerRoomEntity))
			for i in range(len(deleRoomList)):
				self.NeedPlayerRoomEntity.pop(deleRoomList[i])

			if playerCount>0:
				#房间都检查一遍了还是没有把玩家分配完就创建房间
				if playerCount>ROOM_MAX_PLAYER:
					for i in range(playerCount%ROOM_MAX_PLAYER):
						self._creatRoom(ROOM_MAX_PLAYER)
						playerCount-=ROOM_MAX_PLAYER
				
				if playerCount>0:
					self._creatRoom(playerCount)
					playerCount-=playerCount


	def _creatRoom(self,PlayerCount):
		EntityList = []
		for i in range(PlayerCount):
			EntityList.append(self.waitingEnterPlayerEntitys.pop(0))
		self._creatRoomEntity(EntityList)

	def _creatRoomEntity(self,entityList,roomType=0):
		roomId = self.generateRoomId()
		if self.allRoomEntityList.get(roomId,None) !=None:
			self._creatRoomEntity(entityList,roomType)

		KBEngine.createEntityAnywhere("Room",
								{
									"roomKey":roomId,
									"RoomType":roomType,
									"EnterPlayerList":entityList,
									"playerMaxCount":ROOM_MAX_PLAYER},
									Functor.Functor(self._CreatRoomCB,roomId)
									)

	
	def _CreatRoomCB(self,roomId,entityCall):
		self.allRoomEntityList[roomId] = entityCall

	def generateRoomId(self):
		roomid = ""
		roomId_1 = random.randint(1,9)
		roomid=str(roomId_1)
		for num in range(0,5):
			roomId_n = random.randint(0,9)
			roomid = roomid+str(roomId_n)
		return int(roomid)

	def onTimer(self, timerHandle, userData):
		if userData == FEN_PEI_TIMER:
			self.fen_pei()

	#房间通知大厅，需要玩家来填满
	def roomNeedPlayer(self,entityCall,roomId):
		self.NeedPlayerRoomEntity[roomId] = entityCall


	#房间通知大厅，已经满员了
	def roomIsFull(self,entityCall,roomId):
		if self.NeedPlayerRoomEntity.get(roomId,None)!= None:
			self.NeedPlayerRoomEntity.pop(roomId)


	def changeRoom(self,entityMailbox,curRoomId):
		isEnterNewRoom = False
		if len(self.NeedPlayerRoomEntity)>0:
			for roomId in list(self.NeedPlayerRoomEntity):
				if roomId != curRoomId:
					self.NeedPlayerRoomEntity[roomId].enterRoom(entityMailbox)
					isEnterNewRoom = True

		if isEnterNewRoom==False:
			EntityList = [entityMailbox]
			self._creatRoomEntity(EntityList)
			isEnterNewRoom = True

		if isEnterNewRoom:
			#交换房间成功，清除上一个房间交换者的数据
			self.allRoomEntityList[curRoomId].changeRoomSuccess(entityMailbox)
		else:
			print("没有多余的房间了")	
