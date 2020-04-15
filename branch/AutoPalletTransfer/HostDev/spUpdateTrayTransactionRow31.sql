USE [TempPalletTrackingDB]
GO
/****** Object:  StoredProcedure [dbo].[spUpdateTrayTransactionRow31]    Script Date: 08/02/2017 10:23:40 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Prasert
-- Create date: 
-- Description:	
-- =============================================

-- EXEC [spUpdateTrayTransactionRow31] 2, 'AW0162120F', 
-- 'PT0003'

ALTER PROCEDURE [dbo].[spUpdateTrayTransactionRow31] 

	@trayTransID int,
	@trayID varchar(10),	
	@palletIDRow31 varchar(6)
			
AS
BEGIN
	SET NOCOUNT ON;
	
	UPDATE tblTrayTransaction
	SET 
    PalletID_Row31 = @palletIDRow31
							
	WHERE TrayTransID = @trayTransID AND TrayID = @trayID
	
	SELECT @trayTransID, @trayID, @palletIDRow31

END
