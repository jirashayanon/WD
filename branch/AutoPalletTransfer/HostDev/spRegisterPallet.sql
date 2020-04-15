USE [TempPalletTrackingDB]
GO
/****** Object:  StoredProcedure [dbo].[spRegisterPallet]    Script Date: 08/02/2017 10:18:04 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Prasert
-- Create date: 
-- Description:	
-- =============================================

-- EXEC [spRegisterPallet] 'PT0001'

ALTER PROCEDURE [dbo].[spRegisterPallet] 
	-- Add the parameters for the stored procedure here
	@palletID varchar(6)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    INSERT INTO tblRegisteredPallet(PalletID)
	SELECT @palletID
END
