USE [TempPalletTrackingDB]
GO
/****** Object:  StoredProcedure [dbo].[spUpdateTrayCompleted]    Script Date: 08/02/2017 10:22:42 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Prasert
-- Create date: 
-- Description:	
-- =============================================

-- EXEC [spUpdateTrayCompleted] 2, 'AW0162120F', 0

ALTER PROCEDURE [dbo].[spUpdateTrayCompleted] 

	@trayTransID int,
	@trayID varchar(10),	
	@completedTray bit
			
AS
BEGIN
	SET NOCOUNT ON;
	
	UPDATE tblTrayTransaction
	SET 
    CompletedTray = @completedTray
							
	WHERE TrayTransID = @trayTransID AND TrayID = @trayID
	
	SELECT @trayTransID, @trayID, @completedTray

END
