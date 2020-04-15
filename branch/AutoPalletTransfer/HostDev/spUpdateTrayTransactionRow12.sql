USE [TempPalletTrackingDB]
GO
/****** Object:  StoredProcedure [dbo].[spUpdateTrayTransactionRow12]    Script Date: 08/02/2017 10:23:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Prasert
-- Create date: 
-- Description:	
-- =============================================

-- EXEC [spUpdateTrayTransactionRow12] 2, 'AW0162120F', 
-- 'PT0003'

ALTER PROCEDURE [dbo].[spUpdateTrayTransactionRow12] 

	@trayTransID int,
	@trayID varchar(10),	
	@palletIDRow12 varchar(6)
			
AS
BEGIN
	SET NOCOUNT ON;
	
	UPDATE tblTrayTransaction
	SET 
    PalletID_Row12 = @palletIDRow12
							
	WHERE TrayTransID = @trayTransID AND TrayID = @trayID
	
	SELECT @trayTransID, @trayID, @palletIDRow12

END
