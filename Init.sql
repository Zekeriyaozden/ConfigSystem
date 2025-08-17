
IF DB_ID('ConfigDb') IS NULL                
BEGIN
  CREATE DATABASE ConfigDb;               
END
GO                                          


USE ConfigDb;                            
GO

IF OBJECT_ID('dbo.[Configuration]','U') IS NULL  
BEGIN
  CREATE TABLE dbo.[Configuration] (
    [Id] INT IDENTITY(1,1) PRIMARY KEY,          
    [Name] NVARCHAR(100) NOT NULL,           
    [Type] NVARCHAR(20) NOT NULL,             
    [Value] NVARCHAR(500) NOT NULL,              
    [IsActive] BIT NOT NULL,                       
    [ApplicationName] NVARCHAR(100) NOT NULL,   
    [LastUpdated] DATETIME2 NOT NULL              
      CONSTRAINT DF_Configuration_LastUpdated      
      DEFAULT SYSUTCDATETIME()
  );

  CREATE UNIQUE INDEX IX_Config_AppName_Name
    ON dbo.[Configuration]([ApplicationName],[Name]);
END
GO

INSERT INTO [dbo].[Configuration] (ApplicationName,Name,Value,Type,IsActive)
VALUES 
('SERVICE-A', 'SiteName',        'service-a.local', 'string', 1),
('SERVICE-A', 'MaxItemCount',    '50',              'int',    1),
('SERVICE-A', 'IsFeatureEnabled','true',            'bool',   1),
('SERVICE-B', 'SiteName',        'service-b.local', 'string', 1),
('SERVICE-B', 'RefreshRate',     '1.5',             'double', 1),
('SERVICE-B', 'IsBetaMode',      'false',           'bool',   1);
