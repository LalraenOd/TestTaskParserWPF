/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) [PICKINGSUBGROUPID]
      ,[PICKINGSUBGROUPNAME]
      ,[PICKINGGROUPNAME]
  FROM [CarParsing].[dbo].[PickingSubGroups]