USE [TempPalletTrackingDB]
GO
/****** Object:  StoredProcedure [dbo].[spSearchTrayId]    Script Date: 11/28/2017 14:18:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Nasakol P.
-- Create date: 2017-08-17
-- Description:	Use to search tray and hgainfo
-- EXEC [spSearchTrayId] 'AW0180272B'
-- EXEC [spSearchTrayId] '6W4101232X'
-- =============================================
ALTER PROCEDURE [dbo].[spSearchTrayId]
	-- Add the parameters for the stored procedure here
	@trayIdParam varchar(20)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select hgainfo.Id,
		hgainfo.[LineNo],
		p.Id as PalletId,
		hgainfo.Position,
		hgainfo.Processdatetime,
		hgainfo.IaviResult,
		hgainfo.VmiResult,
		hgainfo.SerialNumber,
		hgainfoView.ViewId,
		hgainfoView.ImagePath,
		hgainfoView.Result,
		p.PalletSN,
		p.PalletOrder,
		tt.TrayId,
		tt.TraySN,
		tt.AQTrayHeader,
		tt.IsCompleted
	from (
			SELECT TOP 1 id AS trayId, traySN, AQTrayHeader, isCompleted
			FROM tray
			WHERE tray.traySN = @trayIdParam
			ORDER BY tray.CreatedAt DESC
		) tt
		inner join (
     		SELECT pallet.id, pallet.palletSN, pallet.palletOrder, pallet.trayId
			FROM pallet
		) p on p.trayId = tt.trayId
		inner join hgainfo on hgainfo.palletId = p.id
		left join hgainfoView on hgainfoView.hgainfoId = hgainfo.Id
		left join [view] on [view].id = hgainfoView.viewId
	WHERE [view].isEnabled = 1 or [view].isEnabled is null
	ORDER BY p.palletOrder, hgainfo.position, hgainfoView.viewId
END

