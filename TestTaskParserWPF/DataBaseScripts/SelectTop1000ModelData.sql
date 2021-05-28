/****** Script for SelectTopNRows command from SSMS  ******/
USE CarParsing
SELECT TOP (1000) [MODELID]
      ,[MODELCODE]
      ,[MODELNAME]
      ,[MODELDATERANGE]
      ,[MODELPICKINGCODE]
  FROM [CarParsing].[dbo].[ModelData]