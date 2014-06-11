
 INSERT INTO [dbo].[Role] VALUES (2,null,0,'Customer Support'), (4,null,0,'Customer User'), (1,null,0,'General Administrator'),(3,null,0,'Tech Support')

 INSERT INTO [dbo].[RoleActions] ([RoleId] ,[id]) VALUES
 (1,1),(1,2),(1,3),(1,4),(1,5),(1,9),(1,11),(1,12),(2,1),(2,2),(2,5),(2,9),(2,12),(2,14),(3,3),(3,4),(3,11),(3,13),(4,6),(4,8),(4,10)

 INSERT INTO [dbo].[Company] VALUES
(1,null,0,1,'Diebold','District','Area','Site','Device','test',NULL,'test',NULL,'test@test.com',0,0,'Daily'),
(2,null,0,2,'Company2','District','Area','Site','Device','test',NULL,'test',NULL,'test@test.com',0,0,'Daily'),
(3,null,0,3,'Company3','District','Area','Site','Device','test',NULL,'test',NULL,'test@test.com',0,0,'Daily')
 
 INSERT INTO [dbo].[User] ([Id],[DeletedKey],[IsDisabled],[FirstName],[LastName],[Username],[Email],[Phone],[OfficePhone],[Extension],[Mobile],[PreferredContact],[Title],[Text1],[Text2],[TimeZone],[RoleId],[CompanyId])VALUES 
(1,null,0,'General','Administrator','portaladmin','admin@diebold.com',235235,234535,26356,5652,'Mobile','Mr','rrgwsef','sdrgv','UTC',1, 1),
(2,null,0,'Customer','Support','portalcust.support','cust.support@diebold.com',234,235,234,1234,'Mobile','Mr','2qd','sdmko','UTC',2, 1),
(3,null,0,'Technical','Support','portaltech.support','tech.support@diebold.com',96,6,6,687,'Mobile','Mr','fbu','suf','UTC',3, 1),
(4,null,0,'Customer','User','portaluser','user@customer.com',3456,8,334,3453,'Mobile','Mr','dvb','ef','UTC',4, 1)

INSERT INTO [dbo].[CompanyGrouping1Level]VALUES
(1,'District 1',1),(2,'District 2',1),(3,'District 3',1),(4,'District 4',1),(5,'District 5',1),(6,'District 6',2),(7,'District 7',2),(8,'District 8',2),(9,'District 9',3),(10,'District 10',3)

INSERT INTO [dbo].[CompanyGrouping2Level] VALUES
(1,'Area 1',1),(2,'Area 2',1),(3,'Area 3',1),(4,'Area 4',1),(5,'Area 5',1),(6,'Area 6',2),(7,'Area 7',2),(8,'Area 8',2),(9,'Area 9',3),(10,'Area 10',3),(11,'Area 11',6),(12,'Area 12',6),(13,'Area 13',7),(14,'Area 14',8),(15,'Area 15',8)
 
 INSERT INTO [CompanySubscription]([CompanyId],[id])
  VALUES (1,'DaysRecorded'),(1,'IsNotRecording'),(1,'NetworkDown'),(1,'SMART'),(1,'DriveTemperature'),(1,'RaidStatus'),(1,'VideoLoss'),
 		 (2,'DaysRecorded'),(2,'IsNotRecording'),(2,'NetworkDown'),(2,'SMART'),(2,'DriveTemperature'),(2,'RaidStatus'),(2,'VideoLoss'),
		 (3,'DaysRecorded'),(3,'IsNotRecording'),(3,'NetworkDown'),(3,'SMART'),(3,'DriveTemperature'),(3,'RaidStatus'),(3,'VideoLoss')

INSERT INTO [dbo].[Site] VALUES
(1,null,0,1,'Site1','Note','SharepointURL','ParentAssosciation','DieboldName','Address1','Address2','City','State','123456','County','country','ok',1),
(2,null,0,2,'Site2','Note','SharepointURL','ParentAssosciation','DieboldName','Address1','Address2','City','State','123456','County','country','ok',1),
(3,null,0,3,'Site3','Note','SharepointURL','ParentAssosciation','DieboldName','Address1','Address2','City','State','123456','County','country','ok',1),
(4,null,0,4,'Site4','Note','SharepointURL','ParentAssosciation','DieboldName','Address1','Address2','City','State','123456','County','country','ok',1),
(5,null,0,5,'Site5','Note','SharepointURL','ParentAssosciation','DieboldName','Address1','Address2','City','State','123456','County','country','ok',2),
(6,null,0,6,'Site6','Note','SharepointURL','ParentAssosciation','DieboldName','Address1','Address2','City','State','123456','County','country','ok',2),
(7,null,0,7,'Site7','Note','SharepointURL','ParentAssosciation','DieboldName','Address1','Address2','City','State','123456','County','country','ok',2),
(8,null,0,8,'Site8','Note','SharepointURL','ParentAssosciation','DieboldName','Address1','Address2','City','State','123456','County','country','ok',1),
(9,null,0,9,'Site9','Note','SharepointURL','ParentAssosciation','DieboldName','Address1','Address2','City','State','123456','County','country','ok',1)

INSERT INTO [dbo].Device(Id, Name, ReportBufferSize, ExternalDeviceId, CompanyId, IsDisabled, DeletedKey)
VALUES (1, 'Dvr 1',  1, '15', 1, 0, null),
	   (2, 'Dvr 2',  1, '17', 1, 0, null),
	   (3, 'Gateway 1',  1, '18', 1, 0, null)

INSERT INTO [dbo].Gateway(Id, Protocol, TimeZone, SiteId, MacAddress, EMCId, LastUpdate, LastUsedEMCZoneNumber)
VALUES (3, 1, 'Eastern Standard Time', 1, 'AA:BB:CC:DD:EE:FF', '1111', '2012-05-08', 1)

INSERT INTO [dbo].Dvr(Id, HostName, DeviceKey, TimeZone, ZoneNumber, NumberOfCameras, UserName, [Password], IsInDST, PortA, PortB, GatewayId, DeviceType, PollingFrequency, HealthCheckVersion)
VALUES (1, '10.255.168.67', 'AA:BB:CC:DD:EE:GG-ABCDEFGHIJ', 'Eastern Standard Time', 3, 1, '', '', 0, '', '', 3, 'Costar111', 'OneMinute', 'Version1'),
(2, '10.255.168.68', 'AA:BB:CC:DD:EE:GG-ABC', 'Eastern Standard Time', 3, 1, '', '', 0, '', '', 3, 'Costar111', 'OneMinute', 'Version1')

INSERT INTO [dbo].[AlarmConfiguration] VALUES
(1, 'DaysRecorded', 'Costar111', 'Equals', 'Warning', null, '0', 0, 0, 0, 0, 'Integer'),
(2, 'IsNotRecording', 'Costar111', 'Equals', 'Warning', null, 'true', 0, 0, 0, 0, 'Boolean'),
(3, 'NetworkDown', 'Costar111', 'Equals', 'Warning', null, 'true', 0, 0, 0, 0, 'Boolean'),
(4, 'VideoLoss', 'Costar111', 'Equals', 'Warning', null, 'true', 0, 0, 0, 0, 'Object'),
(5, 'DaysRecorded', 'ipConfigure530', 'Equals', 'Warning', null, '0', 0, 0, 0, 0, 'Integer'),
(6, 'IsNotRecording', 'ipConfigure530', 'Equals', 'Warning', null, 'true', 0, 0, 0, 0, 'Boolean'),
(7, 'NetworkDown', 'ipConfigure530', 'Equals', 'Warning', null, 'true', 0, 0, 0, 0, 'Boolean'),
(8, 'SMART', 'ipConfigure530', 'NotEquals', 'Warning', null, 'passed', 0, 0, 0, 0, 'Boolean'),
(9, 'DriveTemperature', 'ipConfigure530', 'Equals', 'Warning', null, '0', 0, 0, 0, 0, 'Integer'),
(10, 'RaidStatus', 'ipConfigure530', 'NotEquals', 'Warning', null, 'clean', 0, 0, 0, 0, 'Boolean'),
(11, 'VideoLoss', 'ipConfigure530', 'Equals', 'Warning', null, 'true', 0, 0, 0, 0, 'Object'),
(12, 'DaysRecorded', 'VerintEdgeVr200', 'Equals', 'Warning', null, '0', 0, 0, 0, 0, 'Integer'),
(13, 'IsNotRecording', 'VerintEdgeVr200', 'Equals', 'Warning', null, 'true', 0, 0, 0, 0, 'Boolean'),
(14, 'NetworkDown', 'VerintEdgeVr200', 'Equals', 'Warning', null, 'true', 0, 0, 0, 0, 'Boolean'),
(15, 'SMART', 'VerintEdgeVr200', 'NotEquals', 'Warning', null, 'passed', 0, 0, 0, 0, 'Boolean'),
(16, 'DriveTemperature', 'VerintEdgeVr200', 'Equals', 'Warning', null, '0', 0, 0, 0, 0, 'Integer'),
(17, 'RaidStatus', 'VerintEdgeVr200', 'NotEquals', 'Warning', null, 'clean', 0, 0, 0, 0, 'Boolean'),
(18, 'VideoLoss', 'VerintEdgeVr200', 'Equals', 'Warning', null, 'true', 0, 0, 0, 0, 'Object')
-- Alarms for Device 1
,
(22, 'DaysRecorded', 'Costar111', 'Equals', 'Warning', 1, '0', 0, 0, 0, 0, 'Integer'),
(23, 'IsNotRecording', 'Costar111', 'Equals', 'Warning', 1, 'true', 0, 0, 0, 0, 'Boolean'),
(24, 'NetworkDown', 'Costar111', 'Equals', 'Warning', 1, 'true', 0, 0, 0, 0, 'Boolean'),
(25, 'SMART', 'Costar111', 'NotEquals', 'Warning', 1, 'passed', 0, 0, 0, 0, 'Boolean'),
(26, 'DriveTemperature', 'Costar111', 'Equals', 'Warning', 1, '0', 0, 0, 0, 0, 'Integer'),
(27, 'RaidStatus', 'Costar111', 'NotEquals', 'Warning', 1, 'clean', 0, 0, 0, 0, 'Boolean'),
(28, 'VideoLoss', 'Costar111', 'Equals', 'Warning', 1, 'true', 0, 0, 0, 0, 'Object'),
-- Alarms for Device 2
(29, 'DaysRecorded', 'Costar111', 'Equals', 'Warning', 2, '0', 0, 0, 0, 0, 'Integer'),
(30, 'IsNotRecording', 'Costar111', 'Equals', 'Warning', 2, 'true', 0, 0, 0, 0, 'Boolean'),
(31, 'NetworkDown', 'Costar111', 'Equals', 'Warning', 2, 'true', 0, 0, 0, 0, 'Boolean'),
(32, 'SMART', 'Costar111', 'NotEquals', 'Warning', 2, 'passed', 0, 0, 0, 0, 'Boolean'),
(33, 'DriveTemperature', 'Costar111', 'Equals', 'Warning', 2, '0', 0, 0, 0, 0, 'Integer'),
(34, 'RaidStatus', 'Costar111', 'NotEquals', 'Warning', 2, 'clean', 0, 0, 0, 0, 'Boolean'),
(35, 'VideoLoss', 'Costar111', 'Equals', 'Warning', 2, 'true', 0, 0, 0, 0, 'Object')

GO

-- Create Alert Status for Devices 1 and 2
DECLARE @STATID INT; 
DECLARE @DEVICEID INT;
DECLARE @ALARMCONFIGURATIONID INT;
DECLARE @DEVICETYPE NVARCHAR(32);

SET @STATID = 1;

DECLARE DEVICECURSOR CURSOR FOR
	SELECT ID, DEVICETYPE FROM DVR;
OPEN DEVICECURSOR;

FETCH NEXT FROM DEVICECURSOR INTO @DEVICEID, @DEVICETYPE
WHILE @@FETCH_STATUS = 0
	BEGIN
	
		DECLARE ALARMCURSOR CURSOR FOR
		SELECT ID FROM ALARMCONFIGURATION WHERE DEVICEID =@DEVICEID AND DEVICETYPE = @DEVICETYPE
		OPEN ALARMCURSOR;
		
		FETCH NEXT FROM ALARMCURSOR INTO @ALARMCONFIGURATIONID;
		WHILE @@FETCH_STATUS = 0
		BEGIN
			INSERT INTO AlertStatus(Id, DeviceId, AlarmConfigurationId, AlertCount, IsAcknowledged, IsOk, FirstAlertTimeStamp, LastAlertTimeStamp)
			VALUES
			(
				@STATID,
				@DEVICEID,
				@ALARMCONFIGURATIONID,
				0,
				1,
				1,
				NULL, 
				NULL)
		
			SET @STATID = @STATID + 1;
		
			FETCH NEXT FROM ALARMCURSOR INTO @ALARMCONFIGURATIONID;
		END;
		
		CLOSE ALARMCURSOR;
		DEALLOCATE ALARMCURSOR;	

		FETCH NEXT FROM DEVICECURSOR INTO @DEVICEID, @DEVICETYPE
	END;

CLOSE DEVICECURSOR;
DEALLOCATE DEVICECURSOR;
GO