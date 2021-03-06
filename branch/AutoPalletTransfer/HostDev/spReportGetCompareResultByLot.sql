USE [TempPalletTrackingDB]
GO
/****** Object:  StoredProcedure [dbo].[spReportGetCompareResultByLot]    Script Date: 11/28/2017 14:15:34 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Nasakol P.
-- Create date: 2017-08-21
-- Description:	Compare result between VMI and IAVI each view
-- exec spReportGetCompareResultByLot '2017-08-15', '2017-08-20', '%'
-- =============================================
ALTER PROCEDURE [dbo].[spReportGetCompareResultByLot]
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
	SELECT pp.id as PalletId, pp.PalletSN, pp.CreatedAt,
		hgainfo.Id as HgainfoId, 
		hgainfo.SerialNumber, 
		hgainfo.Position, 
		defectVMI.defectName as VmiResult,
		
		CASE 
			WHEN hgainfo.iaviResult = 'REJECT'
				THEN 
					COALESCE(
						STUFF((SELECT DISTINCT ',' + defectName 
							FROM hgainfoView hview1
								LEFT JOIN hgadefect on hgadefect.HgainfoViewId = hview1.id
								LEFT JOIN defect on defect.id = hgadefect.defectId
							WHERE hview1.id = hgainfoView.Id
							FOR XML PATH ('')), 1, 1, '')
					, '')
				ELSE hgainfo.iaviResult	 -- Good or NOHGA
		END as IaviResult,
		
		[view].Name as ViewName,
		'http://172.16.52.236/iavi-report/getImage.php?path=' + hgainfoView.imagePath as ImagePath,
		tray.TraySN,
		tray.IsCompleted,
		aqtraydata.LotNumber
	FROM hgainfoView
		LEFT JOIN hgadefect on hgadefect.HgainfoViewId = hgainfoView.id
		LEFT JOIN defect defectIAVI on defectIAVI.id = hgadefect.defectId
		INNER JOIN hgainfo on hgainfo.Id = hgainfoView.hgainfoId
		INNER JOIN defect defectVMI on defectVMI.id = hgainfo.vmiResult
		inner join pallet pp on pp.id=hgainfo.palletId
		LEFT JOIN tray on tray.id = pp.trayId
		LEFT JOIN aqtraydata on aqtraydata.TrayId = tray.id
		INNER JOIN [view] on [view].id = hgainfoView.viewId
	WHERE 
		-- view.name = 'TopABS'
		-- (view.name = 'FortyFive' or view.name = 'TopPZT' or view.name = 'TopAlignment')
		pp.CreatedAt >= @start_date
		and pp.CreatedAt <= @end_date
		-- and defectIAVI.defectname = 'A1'
		and (aqtraydata.LotNumber LIKE @lot)
	GROUP BY pp.id, pp.palletSN, pp.CreatedAt,
		hgainfo.Id, hgainfo.serialNumber, hgainfo.position, vmiResult, defectVMI.defectName,
		hgainfo.iaviResult,
		tray.TraySN,
		tray.IsCompleted,
		aqtraydata.LotNumber,
		hgainfoView.Id,
		[view].name, imagePath
	ORDER BY hgainfo.Id
END

