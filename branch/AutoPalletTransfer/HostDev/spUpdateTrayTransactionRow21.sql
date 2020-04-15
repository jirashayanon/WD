USE [TempPalletTrackingDB]
GO
/****** Object:  StoredProcedure [dbo].[spUpdateTrayTransactionRow21]    Script Date: 08/02/2017 10:23:23 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Prasert
-- Create date: 
-- Description:	
-- =============================================

-- EXEC [spUpdateTrayTransactionRow21] 2, 'AW0162120F', 
-- 'PT0003'

ALTER PROCEDURE [dbo].[spUpdateTrayTransactionRow21] 

	@trayTransID int,
	@trayID varchar(10),	
	@palletIDRow21 varchar(6)
			
AS
BEGIN
	SET NOCOUNT ON;
	
	UPDATE tblTrayTransaction
	SET 
    PalletID_Row21 = @palletIDRow21
							
	WHERE TrayTransID = @trayTransID AND TrayID = @trayID
	
	SELECT @trayTransID, @trayID, @palletIDRow21

END
