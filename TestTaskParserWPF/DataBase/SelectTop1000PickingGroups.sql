/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) [PICKINGGROUPID]
      ,[PICKINGID]
      ,[PICKINGGROUPNAME]
  FROM [CarDetails].[dbo].[PickingGroups]