USE [TempPalletTrackingDB]
GO
/****** Object:  StoredProcedure [dbo].[spSearchPalletsByTray]    Script Date: 11/28/2017 14:17:57 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Prasert
-- Create date: 
-- Description:	
-- =============================================

-- exec spSearchPalletsByTray '6W4083057D'

ALTER PROCEDURE [dbo].[spSearchPalletsByTray] 
	@AqtrayBarcode varchar(10) 
AS
BEGIN

	SET NOCOUNT ON;
	
	select pallet.Id, pallet.PalletSN, pallet.PalletOrder, pallet.TrayId, tray.TraySN, pallet.Processdatetime, pallet.IsCompleted, pallet.PalletPath,
	pallet.CreatedAt, pallet.LotNumber from pallet
	inner join tray on pallet.TrayId = tray.Id		
	where tray.TraySN = @AqtrayBarcode

	order by pallet.Processdatetime desc
END
