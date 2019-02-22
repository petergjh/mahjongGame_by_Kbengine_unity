import KBEngine
from KBEDebug import *
from interfaces.BaseGlobalEntity import BaseGlobalEntity

class OfflineMessage(BaseGlobalEntity):
	def __init__(self):
		BaseGlobalEntity.__init__(self,"OfflineMessage")

	def initOver(self):
		self.allMails_dir={}
		self._convert_data_type()
		print(self.allMails_dir)

	def _convert_data_type(self):
		for i, info in enumerate(self.offline_messages):
			p_dbid = info["targetDBID"]
			if p_dbid in  self.allMails_dir.values():
				self.allMails_dir[p_dbid].append(info)
			else:
				self.allMails_dir[p_dbid] = [info]
		self.allPlayers=[]

	def onWriteToDB(self,cellData):
		if hasattr(self, 'allMails_dir'):
			self.offline_messages = []
			for data in self.allMails_dir.values():
				if data:
					self.offline_messages.extend(data)

	

	def register(self,ent_base, dbid):
		if dbid in self.allMails_dir:
			data = self.allMails_dir[dbid]
			for info in range(len(data)):
				ent_base.Receive_message(data[info]);
			self.allMails_dir.pop(dbid)



	def deregister(self,ent_base, dbid):
		pass

	def send_message(self,mail):
		dbid =  mail["targetDBID"];
		if dbid in self.allMails_dir.values():
			self.allMails_dir[dbid].append(mail);
		else:
			self.allMails_dir[dbid] = [mail]
		

