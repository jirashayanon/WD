import sys
from random import randint
import datetime
import random
import string
from time import gmtime, strftime
import math




sys.stdout = open('iavi.sql', 'w')



def id_generator(size=10, chars=string.ascii_uppercase + string.digits):
    return ''.join(random.choice(chars) for _ in range(size))

def palletIDGenerator(chars=string.ascii_uppercase):
	result=''.join(random.choice(chars) for _ in range(2))
	resultInt=random.sample(range(1, 10000), 1)
	return result+str(resultInt[0])
def chooseRandomDefect():
	#foo = ['Good','Reject','A1','T8','T6','J4P','S2','J7']
	#secure_random = random.SystemRandom()
	#return(secure_random.choice(foo))
	return random.randint(1,9)


print ("DROP TABLE IF EXISTS `aqtraydata`;CREATE TABLE `aqtraydata` (`TrayId` int(11) NOT NULL,`Date` varchar(30) NOT NULL, `TimeStart` varchar(30) NOT NULL,`TimeEnd` varchar(30) NOT NULL,`UsedTime` varchar(30) NOT NULL,`TesterNumber` varchar(30) NOT NULL,`Customer` varchar(30) NOT NULL,`Product` varchar(30) NOT NULL,`User` varchar(30) NOT NULL,`LotNumber` varchar(30) NOT NULL,`DocControl1` varchar(30) NOT NULL,`DocControl2` varchar(30) NOT NULL,`Sus` varchar(30) NOT NULL,`AssyLine` varchar(30) NOT NULL) ENGINE=InnoDB DEFAULT CHARSET=latin1;")


print("CREATE TABLE `hgadefect` (`id` int(11) NOT NULL, `hgainfo_view_id` int(11) NOT NULL,`defectName` int(20) NOT NULL, `coordinateX` double NOT NULL, `coordinateY` double NOT NULL) ENGINE=InnoDB DEFAULT CHARSET=latin1;")

print("CREATE TABLE `hgainfo` ( `hgainfoId` int(11) NOT NULL, `lineNo` varchar(20) NOT NULL,  `palletId` int(20) DEFAULT NULL, `position` int(11) NOT NULL,  `processdatetime` datetime NOT NULL, `iaviResult` varchar(20) NOT NULL, `vmiResult` int(11) NOT NULL COMMENT '1:None, 2:Good, 3:Reject', `serialNumber` varchar(20) NOT NULL, `trayId` int(15) DEFAULT NULL, `created_at` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP, `updated_at` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00' ON UPDATE CURRENT_TIMESTAMP) ENGINE=InnoDB DEFAULT CHARSET=latin1;")
print("CREATE TABLE `hgainfo_view` (  `id` int(11) NOT NULL, `hgainfoId` int(11) NOT NULL,  `viewId` int(11) NOT NULL,  `imagePath` varchar(255) NOT NULL,  `result` int(20) NOT NULL) ENGINE=InnoDB DEFAULT CHARSET=latin1;")

print("CREATE TABLE `pallets` ( `id` int(11) NOT NULL, `palletSN` varchar(20) NOT NULL,`processPalletId` int(11) DEFAULT NULL,`created_at` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,`updated_at` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00' ON UPDATE CURRENT_TIMESTAMP,`isCompleted` int(11) NOT NULL DEFAULT '0') ENGINE=InnoDB DEFAULT CHARSET=latin1;")

print("CREATE TABLE `trays` (`id` int(11) NOT NULL,`traySN` varchar(20) NOT NULL, `AQTrayHeader` varchar(2048) NOT NULL, `isCompleted` int(11) NOT NULL COMMENT '0: incomplete, 1:complete', `created_at` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP, `updated_at` timestamp NOT NULL DEFAULT '0000-00-00 00:00:00' ON UPDATE CURRENT_TIMESTAMP) ENGINE=InnoDB DEFAULT CHARSET=latin1;")

print("CREATE TABLE `view` ( `id` int(11) NOT NULL, `name` varchar(20) NOT NULL) ENGINE=InnoDB DEFAULT CHARSET=latin1;")

print("DROP TABLE IF EXISTS `defect`;CREATE TABLE `defect` ( `id` int(11) NOT NULL,  `defectName` varchar(20) NOT NULL) ENGINE=InnoDB DEFAULT CHARSET=latin1;")

table_name='view';
for i in range(1, 12):
	cameraView = 'Camera'+"{0:0=2d}".format(i)
	#print 'insert into ' + table_name + " ('id', 'name')  values"+'(' + str(i) + ", '" + cameraView + "');" 
	print("INSERT INTO `view` (`id`, `name`)  VALUES(%d, '%s');" %(i, cameraView))



print("INSERT INTO `defect` (`id`, `defectName`) VALUES(1, 'None'),(2, 'Good'),(3, 'A1'),(4, 'T8'),(5, 'T6'),(6, 'J4P'),(7, 'S2'),(8, 'J1O');")


traySNList=[]
tray_num=50
table_name='trays'
for i in range(1,tray_num+1):
	traySerialN=id_generator()
	traySNList.append(traySerialN)	
	#print 'insert into ' , table_name , "('id', 'traySN','AQTrayHeader', 'isCompleted', 'created_at', 'updated_at') values ", "(" , str(i) , ", " ,"'", traySerialN , "'",", 0, ", "'",,"', '", strftime("%Y-%m-%d %H:%M:%S", gmtime()),"') ;"
	print ("INSERT INTO `trays` (`id`, `traySN`, `AQTrayHeader`, `isCompleted`, `created_at`, `updated_at`) VALUES (%d, '%s', 'null', 0, '%s', '%s');" %(i, traySerialN,strftime("%Y-%m-%d %H:%M:%S", gmtime()), strftime("%Y-%m-%d %H:%M:%S", gmtime()) ))

# sys.stdout.close()
# print traySNList
# sys.stdout = open('ivai.sql', 'w')
# sys.stderr.write(traySNList[49])




for i in range(1,tray_num+1):
	print("INSERT INTO `aqtraydata` (`TrayId`, `Date`, `TimeStart`, `TimeEnd`, `UsedTime`, `TesterNumber`, `Customer`, `Product`, `User`, `LotNumber`, `DocControl1`, `DocControl2`, `Sus`, `AssyLine`) VALUES(%d, '3/12/2017', '3:55:29 AM', '3:56:29 AM', '01:00', 'AVI001', 'WD', 'TRAILS_C8_SD', '', 'Y3DRG_CB2', 'PN#70632-15-SBB', 'STR#G24960SN', 'MPT-4G', 'B4A101');" %(i))



table_name='pallets'
palletList=[]
for i in range(1,tray_num*2+1):
	palletSN=palletIDGenerator()
	palletList.append(palletSN)
	print ("INSERT INTO `pallets` (`id`, `palletSN`, `processPalletId`, `created_at`, `updated_at`, `isCompleted`) VALUES(%d, '%s', NULL, '%s', '%s', 0);" %(i,palletSN, strftime("%Y-%m-%d %H:%M:%S", gmtime()), strftime("%Y-%m-%d %H:%M:%S", gmtime()) ));

viewID=1;
table_name='hgainfo_view'
for hgaID in range(1,tray_num*20+1):
	for viewIndex in range(1,11):
		if hgaID==10 or hgaID==20:
			hgaIDCamera=10
		else:
			hgaIDCamera=hgaID%10
		

		path="Camera"+"{0:0=2d}".format(viewIndex)+"/Camera"+"{0:0=2d}".format(viewIndex)+"-"+"{0:0=2d}".format(hgaIDCamera)+".bmp"
		result=chooseRandomDefect()
		#print 'insert into ' + table_name + " ('id', 'hgainfoId', 'viewId', 'imagePath', 'result')  values "+'(' + str(viewID)+", "+str(hgaID) + ", " + str(viewIndex) + ", "+"'"+path+"'"+","+str(result)+");" 
		print ("INSERT INTO `hgainfo_view` (`id`, `hgainfoId`, `viewId`, `imagePath`, `result`) VALUES(%d, %d, %d, '%s', %d);" %(viewID, hgaID, viewIndex, path, result));
		viewID=viewID+1

'''
for i in range (1,tray_num+1):
	for j in range (1,3):
		for k in range(1,11):
'''
defectList=[]
for i in range (1, 50*20+1):
	tray_id=(i-1)/20+1
	pallet_id=(i-1)/10+1
	if i%20==0:
		position=20
	else:
		position=i%20
	defect= chooseRandomDefect()
	secure_random = random.SystemRandom()
	status=int(secure_random.choice(['1','2', '3']))
	statusS=secure_random.choice(['Good','Reject','None', '-'])
	if defect != 2:
		defectList.append((tray_id,defect))
	#print ("INSERT INTO 'hgainfo' ('hgainfoId', 'lineNo', 'palletId', 'position', 'processdatetime', 'defect', 'status', 'statusS', 'serialNumber','trayId', 'created_at', 'updated_at') VALUES (%s, '1', %s, %s, %s, %s, %s, 'None', %s, NULL, %s, %s)," %(str(i), str(pallet_id), str(position), strftime("%Y-%m-%d %H:%M:%S", gmtime()),defect, str(status), str(statusS), traySNList[tray_id-1], str(tray_id), strftime("%Y-%m-%d %H:%M:%S", gmtime()), strftime("%Y-%m-%d %H:%M:%S", gmtime())))

	#print ("INSERT INTO 'hgainfo' ('hgainfoId', 'lineNo', 'palletId', 'position', 'processdatetime', 'defect', 'status', 'vmiResult', 'serialNumber','trayId', 'created_at', 'updated_at') VALUES ("+str(i)+", '1', "+ str(pallet_id)+", "+str(position)+", "+"'"+strftime("%Y-%m-%d %H:%M:%S", gmtime())+"'"+", "+str(defect)+", "+str(status)+", "+"'"+str(statusS)+"'"+", "+"'"+traySNList[tray_id-1]+"'"+", "+str(tray_id)+", "+"'"+strftime("%Y-%m-%d %H:%M:%S", gmtime())+"'"+", "+"'"+strftime("%Y-%m-%d %H:%M:%S", gmtime())+"'"+"); ")
	serial_NUmber=id_generator()
	print ("INSERT INTO `hgainfo` (`hgainfoId`, `lineNo`, `palletId`, `position`, `processdatetime`, `iaviResult`, `vmiResult`, `serialNumber`, `trayId`, `created_at`, `updated_at`) VALUES(%d, '1', %d, %d, '%s', '%s', %d, '%s', %d, '%s', '%s');" %(i, (pallet_id), (position), strftime("%Y-%m-%d %H:%M:%S", gmtime()), statusS, status, serial_NUmber,tray_id,  strftime("%Y-%m-%d %H:%M:%S", gmtime()),  strftime("%Y-%m-%d %H:%M:%S", gmtime())));


defect_id=1
for defectRecord in defectList:
	ghaID=defectRecord[0]
	hgaViewId=hgaID+random.randint(1,11)
	defect=int(defectRecord[1])
	coordX=random.uniform(0, 1)
	coordY=random.uniform(0, 1)
	#print ("INSERT INTO 'hgadefect' ('id', 'hgainfo_view_id', 'defectName', 'coordinateX', 'coordinateY') VALUES (%s, %s, %s, %s, %s);"%(str(defect_id), str(hgaViewId), str(defect), str(coordX), str(coordY)))
	
	print ("INSERT INTO `hgadefect` (`id`, `hgainfo_view_id`, `defectName`, `coordinateX`, `coordinateY`) VALUES(%d, %d, %d, %s, %s);" %(defect_id, hgaViewId, defect, str(coordX), str(coordY)));

	defect_id=defect_id+1




print("ALTER TABLE `aqtraydata`ADD PRIMARY KEY (`TrayId`);")
print("ALTER TABLE `defect`ADD PRIMARY KEY (`id`);")
print("ALTER TABLE `hgadefect`ADD PRIMARY KEY (`id`), ADD KEY `HGAInfoId` (`hgainfo_view_id`);")
print("ALTER TABLE `hgainfo`ADD PRIMARY KEY (`hgainfoId`), ADD KEY `timestamp_index` (`processdatetime`), ADD KEY `PalletId` (`palletId`), ADD KEY `Processdatetime` (`processdatetime`), ADD KEY `trayId` (`trayId`);")
print("ALTER TABLE `hgainfo_view`ADD PRIMARY KEY (`id`),ADD KEY `hgainfoId` (`hgainfoId`,`viewId`),ADD KEY `viewId` (`viewId`);")
print("ALTER TABLE `pallets`ADD PRIMARY KEY (`id`);")
print("ALTER TABLE `trays`  ADD PRIMARY KEY (`id`);")
print("ALTER TABLE `view`ADD PRIMARY KEY (`id`);")

print("ALTER TABLE `defect` MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;ALTER TABLE `hgadefect` MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4;ALTER TABLE `hgainfo`MODIFY `hgainfoId` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=202;ALTER TABLE `hgainfo_view` MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=247;ALTER TABLE `pallets`  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;ALTER TABLE `trays` MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=7;")

print("ALTER TABLE `hgadefect`ADD CONSTRAINT `hgadefect_ibfk_1` FOREIGN KEY (`hgainfo_view_id`) REFERENCES `hgainfo_view` (`id`);")

print("ALTER TABLE `hgainfo`ADD CONSTRAINT `hgainfo_ibfk_1` FOREIGN KEY (`trayId`) REFERENCES `trays` (`id`);")

print("ALTER TABLE `hgainfo_view`ADD CONSTRAINT `hgainfo_view_ibfk_1` FOREIGN KEY (`hgainfoId`) REFERENCES `hgainfo` (`hgainfoId`) ON DELETE CASCADE, ADD CONSTRAINT `hgainfo_view_ibfk_2` FOREIGN KEY (`viewId`) REFERENCES `view` (`id`);")