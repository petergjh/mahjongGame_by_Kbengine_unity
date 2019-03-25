# -*- coding: utf-8 -*-
import KBEngine
from KBEDebug import *
AUTO_SAVE =1
class BaseGlobalEntity(KBEngine.Entity):

	def __init__(self,className):
		KBEngine.Entity.__init__(self)
		KBEngine.globalData[className] = self 
		self.initOver()
		self.addTimer(15,15,AUTO_SAVE)

	def initOver(self):
		 pass

	def getScriptName(self):
		return self.__class__.__name__

	def onTimer(self,tid,userArg):
		if AUTO_SAVE == userArg:
			self.writeToDB(self._onWriteToBD,True)  

	def _onWriteToBD(self,isSuccess,entity):
		pass