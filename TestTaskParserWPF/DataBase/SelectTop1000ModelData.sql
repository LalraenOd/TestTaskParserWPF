/****** Script for SelectTopNRows command from SSMS  ******/
USE CarDetails
SELECT TOP (1000) [MODELID]
      ,[MODELCODE]
      ,[MODELNAME]
      ,[MODELDATERANGE]
      ,[MODELPICKINGCODE]
  FROM [CarDetails].[dbo].[ModelData]