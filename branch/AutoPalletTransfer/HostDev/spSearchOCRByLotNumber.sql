USE [TempPalletTrackingDB]
GO
/****** Object:  StoredProcedure [dbo].[spSearchOCRByLotNumber]    Script Date: 11/28/2017 14:17:24 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Prasert
-- Create date: 
-- Description:	
-- =============================================

-- EXEC spSearchOCRByLotNumber 'Y8C2A_34'

ALTER PROCEDURE [dbo].[spSearchOCRByLotNumber] 
	@lotNumber varchar(10)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	select * from(
	(
		select hgainfo.SerialNumber, hgainfo.PalletId, pallet.Id, hgainfo.Position, pallet.PalletSN, pallet.Processdatetime, 
		pallet.Product, pallet.LotNumber, pallet.DocControl1, pallet.DocControl2, pallet.Sus, pallet.AssyLine,
		pallet.TrayId, pallet.PalletOrder
		from hgainfo
		inner join pallet on hgainfo.PalletId = pallet.Id
	) as p

	inner join tray  on p.TrayId = tray.Id)

	where LotNumber = @lotNumber
	
	order by Processdatetime desc
END
