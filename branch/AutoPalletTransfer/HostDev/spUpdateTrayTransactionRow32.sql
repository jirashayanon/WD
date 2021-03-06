USE [TempPalletTrackingDB]
GO
/****** Object:  StoredProcedure [dbo].[spUpdateTrayTransactionRow32]    Script Date: 08/02/2017 10:23:49 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Prasert
-- Create date: 
-- Description:	
-- =============================================

-- EXEC [spUpdateTrayTransactionRow32] 2, 'AW0162120F', 
-- 'PT0003'

ALTER PROCEDURE [dbo].[spUpdateTrayTransactionRow32] 

	@trayTransID int,
	@trayID varchar(10),	
	@palletIDRow32 varchar(6)
			
AS
BEGIN
	SET NOCOUNT ON;
	
	UPDATE tblTrayTransaction
	SET 
    PalletID_Row32 = @palletIDRow32
							
	WHERE TrayTransID = @trayTransID AND TrayID = @trayID
	
	SELECT @trayTransID, @trayID, @palletIDRow32

END
