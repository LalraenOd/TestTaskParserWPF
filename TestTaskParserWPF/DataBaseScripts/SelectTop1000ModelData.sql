/****** Script for SelectTopNRows command from SSMS  ******/
USE CarParcing
SELECT TOP (1000) [MODELID]
      ,[MODELCODE]
      ,[MODELNAME]
      ,[MODELDATERANGE]
      ,[MODELPICKINGCODE]
  FROM [CarParcing].[dbo].[ModelData]