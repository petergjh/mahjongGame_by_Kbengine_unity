import KBEngine
from KBEDebug import *
import random

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
				"userId":seatData.userId,
				"holdsCount":len(seatData.holds)
				}
			playerList.append(d)
		data = {
			"state" :self.game.state,
			"playerInfo":playerList,
			"button":self.game.button,
			"turn": self.game.turn,
			}
		self.public_roomInfo = data

	##清空游戏共享数据
	def clearPublicRoomInfo(self):
		playerList = []
		for i in range(self.playerMaxCount):
			d={
				"userId":0,
				"holdsCount":0,
				}
			playerList.append(d)
		data = {
			"state" :"idel",
			"playerInfo":playerList,
			"button":-1,
			"turn": -1,
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
				break
		for i in range(len(self.roomInfo.seats)):
			seat = self.roomInfo.seats[i];
			if seat.ready  == False:
				return

		self.begin()

	def begin(self):
		print("全部就位---开始处理！");
		self.clearPublicRoomInfo()
		self.game = MJData(self.roomInfo,self.playerMaxCount)
		self.shuffle(self.game)
		self.deal(self.game)
		self.numOfMJ = len(self.game.mahjongs) - self.game.currentIndex;
		self.cur_turn = self.game.button
		self.game.state = "playing";
		self.setPublicRoomInfo()
		seats = self.roomInfo.seats
		for i in range(len(seats)):
			if seats[i].entity.client:
				seats[i].entity.cell.game_holds_push(self.game.gameSeats[i].holds)
				#seats[i].entity.client.upDataClientRoomInfo(self.public_roomInfo)
				seats[i].entity.client.game_begin_push()

		print("游戏开始！");
		##游戏开始检测
		self.gameStartCheck(self.game)

	def gameStartCheck(self,game):
		for i in range(len(game.gameSeats)):
			gs = game.gameSeats[i]
			#检查是否有听牌
			self.checkCanTingPai(game,gs)

	#检查是否有听牌
	def checkCanTingPai(self,game,seatData):
		seatData.tingMap = {}
		#检查手上的牌是不是已打缺，如果未打缺，则不进行判定
		for i in range(len(seatData.holds)):
			pai = seatData.holds[i]
			if self.getMJType(pai) == seatData.que:
				return

		#检查是否是七对 前提是没有碰，也没有杠 ，即手上拥有13张牌
		if len(seatData.holds) == 13:
			pairCount = 0;
			danPai = -1;
			for k in seatData.countMap:
				c = seatData.countMap.get(k)
				if c == 2 or c == 3:
					pairCount+=1
				elif c == 4:
					pairCount+=2
				
				if c == 1 or c == 3:
					#如果已经有单牌了，表示不止一张单牌，并没有下叫。直接闪
					if danPai>=0:
						break

					danPai = k

			if pairCount == 6:
				#七对只能和一张，就是手上那张单牌
				seatData.tingMap[danPai] = {
					"pattern":"7pairs",
					"fan":"2",
					}

		#对对胡检测 由于四川麻将没有吃，所以只需要检查手上的牌
		#对对胡叫牌有两种情况
		#1、N坎 + 1张单牌
		#2、N-1坎 + 两对牌
		singleCount = 0
		pairCount = 0
		arr = []
		for k in seatData.countMap:
			c = seatData.countMap.get(k)
			if c == 1:
				singleCount+=1
				arr.append(k)
			elif c ==2:
				pairCount+=1
				arr.append(k)
			elif c ==4:
				singleCount+=1
				pairCount+=2

		if (pairCount == 0 and singleCount ==1) or (pairCount == 2 and singleCount == 0):
			for i in range(len(arr)):
				p = arr[i];
				if seatData.tingMap.get(p,None) == None:
					seatData.tingMap[p] ={
						"pattern":"duidui",
						"fan":"1"
					}

		#检查是不是平胡
		#检查筒
		if seatData.que !=0:
			self.checkTingPai(seatData,0,9)
		elif seatData.que !=1:
			self.checkTingPai(seatData,9,18)
		elif seatData.que !=2:
			self.checkTingPai(seatData,18,27)

	#检测听牌
	def checkTingPai(self,seatData,begin,end):
		for i in range(begin,end):
			if seatData.tingMap.get(i,None) !=None:
				continue
			#将牌加入到计数中
			old = seatData.countMap.get(i,None)
			if old == None:
				seatData.countMap[i] = 1
			else:
				seatData.countMap[i] += 1

			seatData.holds.append(i)
			#逐个判定手上的牌
			ret = self.checkCanHu(seatData)
			if ret:
				#平胡 0番
				seatData.tingMap[i] = {
					"pattern":"normal",
					"fan":"0"
				}
			#搞完以后，撤消刚刚加的牌
			if old==None:
				del seatData.countMap[i]
			else:
				seatData.countMap[i] = old
			seatData.holds.pop()

	##逐个判定手上的牌是否能让胡
	def checkCanHu(self,seatData):
		#如果当前牌大于等于２，则将它选为将牌
		for k in seatData.countMap:
			c = seatData.countMap.get(k)
			if c == None:
				c = 0
			if c <2:
				continue
			seatData.countMap[k] -=2
			#逐个判定剩下的牌是否满足　３Ｎ规则,一个牌会有以下几种情况
			#1、0张，则不做任何处理
			#2、2张，则只可能是与其它牌形成匹配关系
			#3、3张，则可能是单张形成 A-2,A-1,A  A-1,A,A+1  A,A+1,A+2，也可能是直接成为一坎
			#4、4张，则只可能是一坎+单张
			ret = self.checkSingle(seatData)
			seatData.countMap[k] +=2
			if ret:
				return True


	#检查是不是单牌
	def checkSingle(self,seatData):
		holds = seatData.holds;
		selected = -1;
		c = 0
		for i in range(len(holds)):
			pai = holds[i]
			c = seatData.countMap.get(pai)
			if c !=0:
				selected = pai
				break
		#如果没有找到剩余牌，则表示匹配成功了
		if selected == -1:
			return True
		#否则，进行匹配
		if c == 3:
			#直接作为一坎
			seatData.countMap[selected] = 0;
			ret = self.checkSingle(seatData);
			#立即恢复对数据的修改
			seatData.countMap[selected] = c;
			if ret ==True:
				return True

		elif c == 4:
			#直接作为一坎
			seatData.countMap[selected] = 1;
			ret = self.checkSingle(seatData)
			#立即恢复对数据的修改
			seatData.countMap[selected] = c;
			#如果作为一坎能够把牌匹配完，直接返回TRUE。
			if ret == True:
				return True

		#按单牌处理
		return self.matchSingle(seatData,selected)

	#匹配单牌
	def matchSingle(self,seatData,selected):
		#分开匹配 A-2,A-1,A
		matched = True
		v = selected % 9
		if v<2:
			matched = False
		else:
			for i in range(3):
				t = selected - 2 + i
				cc = seatData.countMap.get(t)
				if cc == None or cc == 0:
					matched = False
					break
		
		#匹配成功，扣除相应数值
		if matched:
			seatData.countMap[selected - 2] -=1
			seatData.countMap[selected - 1] -=1
			seatData.countMap[selected] -=1
			ret = self.checkSingle(seatData);
			seatData.countMap[selected - 2] +=1
			seatData.countMap[selected - 1] +=1
			seatData.countMap[selected] +=1
			if ret == True:
				return True

		#分开匹配 A-1,A,A + 1
		matched = True
		if v<1 or v>7:
			matched = False
		else:
			for i in range(3):
				t = selected - 1 +i
				cc = seatData.countMap.get(t)
				if cc == None:
					matched = False
					break
				if cc == 0:
					matched = False
					break
		#匹配成功，扣除相应数值
		if matched:
			seatData.countMap[selected-1] -=1
			seatData.countMap[selected] -=1
			seatData.countMap[selected + 1] -=1
			ret = self.checkSingle(seatData)
			seatData.countMap[selected - 1] +=1
			seatData.countMap[selected] +=1
			seatData.countMap[selected + 1] +=1
			if ret == True:
				return True

		#分开匹配 A,A+1,A + 2
		matched = True
		if v>6:
			matched = False
		else:
			for i in range(3):
				t = selected +i
				cc = seatData.countMap.get(t)
				if cc ==None:
					matched = False
					break
				if cc == 0:
					matched = False
					break
		#匹配成功，扣除相应数值
		if matched:
			seatData.countMap[selected] -=1
			seatData.countMap[selected + 1] -=1
			seatData.countMap[selected + 2] -=1
			ret = self.checkSingle(seatData);
			seatData.countMap[selected] +=1
			seatData.countMap[selected + 1] +=1
			seatData.countMap[selected + 2] +=1
			if ret == True:
				return True

		return False

	#获取麻将花色
	def getMJType(self,pai):
		if pai >= 0 and pai < 9:
			#筒
			return 0;
		elif pai >= 9 and pai < 18:
			#条
			return 1;    
		elif pai >= 18 and pai < 27:
			#万
			return 2;

	#洗牌
	def shuffle(self,game):
		mahjongs = game.mahjongs
		#筒 (0 ~ 8 表示筒子)
		for i in range(9):
			for c in range(4):
				mahjongs.append(i)
		#条 9 ~ 17表示条子
		# for i in range(9,18):
		# 	for c in range(4):
		# 		mahjongs.append(i)


		#万 18 ~ 26表示万
		for i in range(18,27):
			for c in range(4):
				mahjongs.append(i)

		random.shuffle(mahjongs)		#随机打乱牌  洗牌

	#发牌
	def deal(self,game):
		game.currentIndex = 0	#强制清0
		#每人13张 一共 13*人数  庄家多一张 
		seatIndex = game.button;
		allFPCount = 13*self.playerMaxCount;
		for i in range(allFPCount):
			self.mopai(game,seatIndex);
			seatIndex +=1
			seatIndex = seatIndex%self.playerMaxCount;

		#庄家多摸最后一张
		self.mopai(game,game.button)
		#当前轮设置为庄家
		game.turn = game.button


	#摸牌
	def mopai(self,game,seatIndex):
		pai = game.mahjongs[game.currentIndex]
		game.gameSeats[seatIndex].holds.append(pai)
		game.currentIndex +=1
		data = game.gameSeats[seatIndex];
		#统计牌的数目
		c = data.countMap.get(pai,None)
		if c== None:
			c=0
		data.countMap[pai] = c + 1;



#----------------------------------------------------------------------------
#麻将信息类
class MJData:
	def __init__(self,roomInfo,maxPlayerCount):
		self.state = "idle"
		self.seatList = roomInfo.seats
		self.mahjongs = []
		self.currentIndex = 0  #当前发的牌在所有牌中的索引
		self.button = 1 #庄家位置
		self.turn = 0  #记录该谁出牌
		self.chuPai = -1
		self.gameSeats = []
		for i in range(maxPlayerCount):	
			seat = seatData(self,i,self.seatList[i].userId)
			self.gameSeats.append(seat)


#所有玩家的牌类信息
class seatData:
	def __init__(self,game,index,userId):
		self.game = game   #游戏对象
		self.seatIndex = index   #玩家座位索引
		self.userId = userId		#玩家id
		self.holds = []  #持有的牌
		#self.folds = []  #打出的牌
		self.tingMap = {} #玩家手上的牌的数目，用于快速判定碰杠
		self.countMap = {}  #玩家手上的牌的数目，用于快速判定碰杠
		self.que = -1 #缺一门
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