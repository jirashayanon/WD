USE [TempPalletTrackingDB]
GO
/****** Object:  StoredProcedure [dbo].[spSearchOCR]    Script Date: 11/28/2017 14:17:06 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Prasert
-- Create date: 
-- Description:	
-- =============================================

-- EXEC spSearchOCR 'Y9HDS0'

ALTER PROCEDURE [dbo].[spSearchOCR] 
	@hgaSN varchar(10)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select * from(
	(
		select hgainfo.SerialNumber as VMISerialNumber, hgainfo.IaviSerialNumber as IAVISerialNumber, hgainfo.PalletId, pallet.Id, hgainfo.Position, pallet.PalletSN, pallet.Processdatetime, 
		pallet.Product, pallet.LotNumber, pallet.DocControl1, pallet.DocControl2, pallet.Sus, pallet.AssyLine,
		pallet.TrayId, pallet.PalletOrder
		from hgainfo
		inner join pallet on hgainfo.PalletId = pallet.Id
	) as p

	inner join tray  on p.TrayId = tray.Id)

	where p.VMISerialNumber LIKE '%' + @hgaSN + '%' OR p.IAVISerialNumber LIKE '%' + @hgaSN + '%'
	
	order by Processdatetime desc
END
