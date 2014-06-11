USE [DATABASENAME]

 INSERT INTO [dbo].[Role] ([Id],[DeletedKey],[IsDisabled],[Name]) VALUES (2,null,0,'Customer Support'), (4,null,0,'Customer User'), (1,null,0,'General Administrator'),(3,null,0,'Tech Support')

 INSERT INTO [dbo].[RoleActions] ([RoleId] ,[id]) VALUES
 (1,1),(1,2),(1,3),(1,4),(1,5),(1,9),(1,11),(1,12),(2,1),(2,2),(2,5),(2,9),(2,12),(2,14),(3,3),(3,4),(3,11),(3,13),(4,6),(4,8),(4,10)

 INSERT INTO [dbo].[Company] VALUES
(1,null,0,1,'Diebold','District','Area','Site','Device','test',NULL,'test',NULL,'test@test.com',0,0,'Daily')
 
 INSERT INTO [dbo].[User] ([Id],[DeletedKey],[IsDisabled],[FirstName],[LastName],[Username],[Email],[Phone],[OfficePhone],[Extension],[Mobile],[PreferredContact],[Title],[Text1],[Text2],[TimeZone],[RoleId],[CompanyId])VALUES 
(1,null,0,'General','Administrator','portaladmin','admin@diebold.com',235235,234535,26356,5652,'Mobile','Mr','rrgwsef','sdrgv','UTC',1, 1),
(2,null,0,'Customer','Support','portalcust.support','cust.support@diebold.com',234,235,234,1234,'Mobile','Mr','2qd','sdmko','UTC',2, 1),
(3,null,0,'Technical','Support','portaltech.support','tech.support@diebold.com',96,6,6,687,'Mobile','Mr','fbu','suf','UTC',3, 1),
(4,null,0,'Customer','User','portaluser','user@customer.com',3456,8,334,3453,'Mobile','Mr','dvb','ef','UTC',4, 1)

INSERT INTO [dbo].[CompanySubscription] ([CompanyId],[id]) VALUES
(1, 'DaysRecorded'),(1, 'IsNotRecording'),(1, 'NetworkDown'),(1, 'SMART'),(1, 'DriveTemperature'),(1, 'RaidStatus'),(1, 'VideoLoss')

INSERT INTO [dbo].[CompanyGrouping1Level]VALUES
(1,'District 1',1),(2,'District 2',1),(3,'District 3',1)

INSERT INTO [dbo].[CompanyGrouping2Level] VALUES
(1,'Area 1',1),(2,'Area 2',1),(3,'Area 3',1)
 
INSERT INTO [dbo].[Site] VALUES
(1,null,0,1,'Enterprise Security','Note','SharepointURL','ParentAssosciation','DieboldName','Address1','Address2','City','State','123456','County','country','ok',1)

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