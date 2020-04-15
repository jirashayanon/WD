USE [TempPalletTrackingDB]
GO
/****** Object:  StoredProcedure [dbo].[spUpdateTrayTransactionRow22]    Script Date: 08/02/2017 10:23:32 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Prasert
-- Create date: 
-- Description:	
-- =============================================

-- EXEC [spUpdateTrayTransactionRow22] 2, 'AW0162120F', 
-- 'PT0003'

ALTER PROCEDURE [dbo].[spUpdateTrayTransactionRow22] 

	@trayTransID int,
	@trayID varchar(10),	
	@palletIDRow22 varchar(6)
			
AS
BEGIN
	SET NOCOUNT ON;
	
	UPDATE tblTrayTransaction
	SET 
    PalletID_Row22 = @palletIDRow22
							
	WHERE TrayTransID = @trayTransID AND TrayID = @trayID
	
	SELECT @trayTransID, @trayID, @palletIDRow22

END
