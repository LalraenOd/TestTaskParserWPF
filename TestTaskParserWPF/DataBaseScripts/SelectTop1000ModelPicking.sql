/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) [PICKINGID]
      ,[MODELCODE]
      ,[DATE]
      ,[EQUIPMENT]
      ,[ATM,MTM]
      ,[BACK DOOR]
      ,[BODY]
      ,[BODY 1]
      ,[BODY 2]
      ,[BODY SHAPE]
      ,[BUILDING CONDITION]
      ,[CAB]
      ,[CATEGORY]
      ,[COOLER]
      ,[DECK]
      ,[DECK(MATERIAL)]
      ,[DECK,CAB]
      ,[DESTINATION]
      ,[DESTINATION 1]
      ,[DESTINATION 2]
      ,[DRIVER S LICENSE]
      ,[DRIVER S POSITION]
      ,[EMISSION REGULATION]
      ,[ENGINE 1]
      ,[ENGINE 2]
      ,[FUEL INDUCTION]
      ,[GEAR SHIFT TYPE]
      ,[GRADE]
      ,[GRADE MARK]
      ,[INCOMPLETE VEHICLES]
      ,[LOADING CAPACITY]
      ,[MODEL MARK]
      ,[NO.OF DOORS]
      ,[PRODUCT]
      ,[REAR SUSPENTION]
      ,[REAR TIRE]
      ,[ROLL BAR]
      ,[ROOF]
      ,[SEAT TYPE]
      ,[SEATING CAPACITY]
      ,[SIDE WINDOW]
      ,[TOP,BACK DOOR]
      ,[TRANSMISSION MODEL]
      ,[TRUCK OR VAN]
      ,[VEHICLE MODEL]
  FROM [CarParsing].[dbo].[ModelPicking]