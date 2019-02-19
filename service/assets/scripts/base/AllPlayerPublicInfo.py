import KBEngine
from KBEDebug import *
from interfaces.BaseGlobalEntity import BaseGlobalEntity

class AllPlayerPublicInfo(BaseGlobalEntity):
	def __init__(self):
		BaseGlobalEntity.__init__(self,"AllPlayerPublicInfo")

	def initOver(self):
		self.allPlayers_dir={}
		self._convert_data_type()
		print(self.allPlayers_dir)

	def _convert_data_type(self):
		for i, info in enumerate(self.allPlayers):
			 p_dbid = info["playerDBID"]
			 info["isOnLine"] = False
			 self.allPlayers_dir[p_dbid] = info
		self.allPlayers=[]

	def onWriteToDB(self,cellData):
		if hasattr(self, 'allPlayers_dir'):
			self.allPlayers = []
			for data in self.allPlayers_dir.values():
				if data:
					self.allPlayers.append(data)

	def upDataInfo(self,ent_base, dbid,isOnLine):
		info = ent_base.GetPlayerInfo()
		print(info)
		if dbid in self.allPlayers_dir:
			self.allPlayers_dir[dbid]["isOnLine"] = isOnLine
			self.allPlayers_dir[dbid]["playerName"] = info["playerName"]
			self.allPlayers_dir[dbid]["playerGold"] =info["playerGold"]
		else:
			_data = {
			"isOnLine" : isOnLine,
			"playerGold" : info["playerGold"],
			"playerName" : info["playerName"],
			"playerDBID" : info["playerDBID"]
			}
			self.allPlayers_dir[dbid] = _data

	def register(self,ent_base, dbid):
		self.upDataInfo(ent_base, dbid,True)
		print(self.allPlayers_dir)

	def deregister(self,ent_base, dbid):
		self.upDataInfo(ent_base, dbid,False)
		print(self.allPlayers_dir)

	def GetPlayersInfo(self,friendList):
		infoList = []
		for i in range(len(_friendList)):
			if _friendList[i] in self.allPlayers_dir:
				infoList.append(self.allPlayers_dir[_friendList[i]])
		return infoList
