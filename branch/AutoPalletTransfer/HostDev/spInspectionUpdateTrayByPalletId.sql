USE [TempPalletTrackingDB]
GO
/****** Object:  StoredProcedure [dbo].[spInspectionUpdateTrayByPalletId]    Script Date: 11/28/2017 14:14:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Nasakol P.
-- Create date: 2017-08-17
-- Description:	Map pallet and tray, using the latest pallet S/N
-- =============================================
ALTER PROCEDURE [dbo].[spInspectionUpdateTrayByPalletId]
	-- Add the parameters for the stored procedure here
	@palletSN varchar(20),
	@trayId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE hgainfo
    SET trayId = @trayId
    WHERE palletId = (SELECT TOP 1 pallet.id 
		FROM pallet 
		WHERE pallet.palletSN = @palletSN 
		ORDER BY pallet.id DESC)
END

