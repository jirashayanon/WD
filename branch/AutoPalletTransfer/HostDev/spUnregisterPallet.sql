USE [TempPalletTrackingDB]
GO
/****** Object:  StoredProcedure [dbo].[spUnregisterPallet]    Script Date: 08/02/2017 10:18:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Prasert
-- Create date: 
-- Description:	
-- =============================================

-- EXEC [spUnregisterPallet] 'PT0001'

ALTER PROCEDURE [dbo].[spUnregisterPallet] 
	-- Add the parameters for the stored procedure here
	@palletID varchar(6)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    DELETE FROM dbo.tblRegisteredPallet WHERE PalletID = (SELECT @palletID)
END
