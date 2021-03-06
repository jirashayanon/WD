USE [TempPalletTrackingDB]
GO
/****** Object:  StoredProcedure [dbo].[spReportGetOverUnderRejectSummary]    Script Date: 11/28/2017 14:15:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Nasakol P.
-- Create date: 2017-08-21
-- Description:	Get 5 slider alignment parameters
-- exec [spReportGetOverUnderRejectSummary] '2017-08-15', '2017-08-20', '%'
-- =============================================
ALTER PROCEDURE [dbo].[spReportGetOverUnderRejectSummary]
	-- Add the parameters for the stored procedure here
	@start_date varchar(30),
	@end_date varchar(30),
	@lot varchar(30)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT 
		defectVMI,
		defectIAVI,
		SUM(CASE 
				WHEN defectVMI = 'Good' AND (defectIAVI is not null)
					THEN 1
					ELSE 0
			END) as OverReject,
		SUM(CASE 
				WHEN defectVMI <> 'None' AND defectVMI <> 'Good' AND (defectIAVI is null)
					THEN 1
					ELSE 0
			END) as UnderReject,
		SUM(CASE 
				WHEN defectVMI = 'None' AND (defectIAVI is not null)
					THEN 1
					ELSE 0
			END) as UnknownReject,
		SUM(CASE 
				WHEN (defectVMI <> 'None' AND defectVMI <> 'Good' and defectIAVI is not null) or
					((defectVMI = 'None' or defectVMI = 'Good') and defectIAVI is null)
					THEN 1
					ELSE 0
			END) as MatchAtleast,
		Total
	FROM  (SELECT 
			tray.Id as TrayId,
			tray.TraySN,
			tray.CreatedAt,
			pallet.PalletSN,
			pallet.PalletOrder,
			hgainfo.Position,
			hgainfo.Id,
			hgainfo.SerialNumber,
			defectVMI.defectName as DefectVMI,
			defectIAVI.defectName as DefectIAVI
		FROM tray
			INNER JOIN pallet on pallet.trayId = tray.id
			INNER JOIN hgainfo on hgainfo.palletId = pallet.id
			LEFT JOIN defect defectIAVI on defectIAVI.id = (
				SELECT TOP 1 defect.id
				FROM    hgainfoView
					INNER JOIN hgadefect on hgadefect.HgainfoViewId = hgainfoView.id
					INNER JOIN defect on defect.id = hgadefect.defectId
				WHERE   hgainfoView.hgainfoId = hgainfo.Id
				ORDER BY defect.priority
			)
			INNER JOIN defect defectVMI on defectVMI.id = hgainfo.vmiResult
		WHERE
			tray.CreatedAt >= @start_date AND
			tray.CreatedAt <= @end_date AND
			pallet.LotNumber LIKE @lot AND
			tray.isCompleted = 1
			-- trays.LotNumber = ''
	) t1
	CROSS JOIN
	(
		SELECT COUNT(*) AS Total 
		FROM trays_hga
		WHERE 
			trays_hga.CreatedAt >= @start_date AND
			trays_hga.CreatedAt <= @end_date AND
			trays_hga.LotNumber LIKE @lot AND
			trays_hga.isCompleted = 1
	) t2
	GROUP BY defectVMI, defectIAVI, Total;

END

