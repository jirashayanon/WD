USE [TempPalletTrackingDB]
GO
/****** Object:  StoredProcedure [dbo].[spGetDefectsOnTray]    Script Date: 11/28/2017 14:12:09 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Nasakol P.
-- Create date: 2017-08-16
-- Description:	Get all defects for each hga on tray.
--				Save to defectList for multipleGood in S/W 
-- exec [spGetDefectsOnTray] 'AW0266846M'
-- =============================================
ALTER PROCEDURE [dbo].[spGetDefectsOnTray]
	-- Add the parameters for the stored procedure here
	@trayIdParam varchar(20)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT hgainfo.Id,
		hgainfo.[LineNo],
		hgainfo.PalletId,
		hgainfo.Position,
		p.PalletOrder,
		hgainfo.IaviResult,
		hgainfo.VmiResult,
		hgainfo.SerialNumber,
		p.PalletSN,
		p.Processdatetime,
		p.TrayId,
		tt.TraySN,
		
		COALESCE(
			STUFF((SELECT ',' + defectName 
				FROM hgainfoView
					LEFT JOIN hgadefect on hgadefect.HgainfoViewId = hgainfoView.id
					LEFT JOIN defect on defect.id = hgadefect.defectId
				WHERE hgainfoView.hgainfoId = hgainfo.Id
				FOR XML PATH ('')), 1, 1, '')
		, '') As DefectString
		
		-- coalesce(GROUP_CONCAT(DISTINCT defect.defectName SEPARATOR ','), '') as defectString2
	FROM (
			SELECT TOP 1 id AS trayId, traySN, AQTrayHeader, isCompleted
			FROM tray
			WHERE tray.traySN = @trayIdParam
			ORDER BY tray.CreatedAt DESC
		) tt
		INNER JOIN pallet p on p.trayId = tt.trayId
		INNER JOIN hgainfo on hgainfo.palletId = p.id
		-- LEFT JOIN hgainfoView on hgainfoView.hgainfoId = hgainfo.Id
		-- LEFT JOIN hgadefect on hgadefect.HgainfoViewId = hgainfoView.id
		-- LEFT JOIN defect on defect.id = hgadefect.defectId
	GROUP BY hgainfo.Id,
		hgainfo.[lineNo],
		hgainfo.palletId,
		hgainfo.position,
		p.palletOrder,
		hgainfo.iaviResult,
		hgainfo.vmiResult,
		hgainfo.serialNumber,
		p.palletSN,
		p.Processdatetime,
		p.trayId,
		tt.traySN
	ORDER BY p.palletOrder, hgainfo.position
END

