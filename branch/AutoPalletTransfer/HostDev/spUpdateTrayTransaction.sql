USE [TempPalletTrackingDB]
GO
/****** Object:  StoredProcedure [dbo].[spUpdateTrayTransaction]    Script Date: 08/02/2017 10:22:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Prasert
-- Create date: 
-- Description:	
-- =============================================

-- EXEC [spUpdateTrayTransaction] 1, 'AW0162120F', 
-- 'PT0001', '', 
-- 'PT0002', '',
-- '', '', 
-- 0,
-- '05-03-2017 16:40:00 PM'

ALTER PROCEDURE [dbo].[spUpdateTrayTransaction] 

	@trayTransID int,
	@trayID varchar(10),
	
	@palletIDRow11 varchar(6),
	@palletIDRow12 varchar(6),				
	
	@palletIDRow21 varchar(6),
	@palletIDRow22 varchar(6),
	
	@palletIDRow31 varchar(6),
	@palletIDRow32 varchar(6),	
		
	@completedTray bit,
	@createdDateTime DATETIME
			
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE tblTrayTransaction
	SET TrayID = @trayID, 
    PalletID_Row11 = @palletIDRow11, 
    PalletID_Row12 = @palletIDRow12, 
    PalletID_Row21 = @palletIDRow21, 
    PalletID_Row22 = @palletIDRow22, 
    PalletID_Row31 = @palletIDRow31, 
    PalletID_Row32 = @palletIDRow32,         
    CompletedTray = @completedTray,
    CreatedDateTime = @createdDateTime						
							
	WHERE TrayTransID = @trayTransID
	
	SELECT @trayTransID, @trayID, 
	@palletIDRow11, @palletIDRow12,
	@palletIDRow21, @palletIDRow22,
	@palletIDRow31, @palletIDRow32,
	@completedTray,
	@createdDateTime

END
