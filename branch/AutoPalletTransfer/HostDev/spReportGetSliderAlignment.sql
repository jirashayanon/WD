USE [TempPalletTrackingDB]
GO
/****** Object:  StoredProcedure [dbo].[spReportGetSliderAlignment]    Script Date: 11/28/2017 14:16:10 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Nasakol P.
-- Create date: 2017-08-21
-- Description:	Get 5 slider alignment parameters
-- exec spReportGetSliderAlignment '2017-08-15', '2017-08-20', '%'
-- =============================================
ALTER PROCEDURE [dbo].[spReportGetSliderAlignment]
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
    SELECT pallet.id as PalletId,
		pallet.PalletSN,
		pallet.Processdatetime,
		pallet.LotNumber,
		hgainfo.Id,
		hgainfo.SerialNumber,
		hgainfo.Position,
		
		CASE 
			WHEN hgainfo.iaviResult = 'REJECT'
				THEN 
					COALESCE(
						STUFF((SELECT DISTINCT ',' + defectName 
							FROM hgainfoView
								LEFT JOIN hgadefect on hgadefect.HgainfoViewId = hgainfoView.id
								LEFT JOIN defect on defect.id = hgadefect.defectId
							WHERE hgainfoView.hgainfoId = hgainfo.Id
							FOR XML PATH ('')), 1, 1, '')
					, '')
				ELSE hgainfo.iaviResult	 -- Good or NOHGA
		END as iaviResult,
             
		-- IF(hgainfo.iaviResult='REJECT', GROUP_CONCAT(DISTINCT coalesce(defectIAVI.defectName, '') order by defectIAVI.defectName SEPARATOR ' '), hgainfo.iaviResult) as iaviResult,
		hgainfo.A_Location,
		hgainfo.B_Location,
		hgainfo.Skew,
		hgainfo.PitchOffset,
		hgainfo.RollOffset
	FROM hgainfo
		INNER JOIN pallet on pallet.id = hgainfo.palletId
	WHERE
		pallet.CreatedAt >= @start_date AND
		pallet.CreatedAt <= @end_date AND
		pallet.LotNumber LIKE @lot 
	GROUP BY
		pallet.id,
		pallet.palletSN,
		pallet.processdatetime,
		pallet.LotNumber,
		hgainfo.Id,
		hgainfo.serialNumber,
		hgainfo.position,
		hgainfo.iaviResult,
		hgainfo.A_Location,
		hgainfo.B_Location,
		hgainfo.Skew,
		hgainfo.PitchOffset,
		hgainfo.RollOffset
	    
END

