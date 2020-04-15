USE [TempPalletTrackingDB]
GO
/****** Object:  UserDefinedFunction [dbo].[fnGetNextEquipmentType]    Script Date: 08/18/2017 10:47:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Prasert
-- Create date: 
-- Description:	
-- =============================================

--//    //ASLV  , APT   , ILC   , SJB   , AVI   , UNOCR
--//    //10    , 80    , 30    , 40    , 50    , 60
--//    NA      = 0,
--//    ASLV    = 10,
--//    ACAM    = 20,
--//    ILC     = 30,
--//    SJB     = 40,
--//    AVI     = 50,
--//    UNOCR   = 60,
--//    FVMI    = 70,
--//    APT     = 80,

-- SELECT dbo.fnGetNextEquipmentType(40) AS NextEquipmentType


ALTER FUNCTION [dbo].[fnGetNextEquipmentType] 
(
	@equipmentType int
)
RETURNS int
AS
BEGIN
	DECLARE @Result int

	SET @Result =
		(
			CASE 	
				WHEN @equipmentType = 10 THEN 80
				WHEN @equipmentType = 20 THEN 30
				WHEN @equipmentType = 30 THEN 40
				WHEN @equipmentType = 40 THEN 50
				WHEN @equipmentType = 50 THEN 60	
				WHEN @equipmentType = 60 THEN 10			
				WHEN @equipmentType = 80 THEN 30			
				ELSE 0
			END		
		)
			
	RETURN @Result
END
