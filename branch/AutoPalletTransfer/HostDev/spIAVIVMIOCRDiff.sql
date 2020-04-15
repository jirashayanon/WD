USE [TempPalletTrackingDB]
GO
/****** Object:  StoredProcedure [dbo].[spIAVIVMIOCRDiff]    Script Date: 11/28/2017 14:13:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Prasert
-- Create date: 
-- Description:	
-- =============================================

-- EXEC spIAVIVMIOCRDiff '2017-11-20', '2017-11-28'
-- EXEC spIAVIVMIOCRDiff '2017-10-20 07:00:00', '2017-11-24 07:00:00'

ALTER PROCEDURE [dbo].[spIAVIVMIOCRDiff] 

	@dtStartDate varchar(30),
	@dtEndDate varchar(30)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT  hgainfoId, [LineNo], PalletId, Position, Processdatetime, IaviResult, VmiResult, defect.DefectName, 
			SerialNumber, IaviSerialNumber, TrayId, CreatedAt, UpdatedAt
	FROM
	(
			(
				SELECT hgainfo.Id as hgainfoId, [LineNo], PalletId, Position, Processdatetime, IaviResult, VmiResult, SerialNumber, IaviSerialNumber, TrayId, CreatedAt, UpdatedAt
				FROM hgainfo
			) AS h
	
		inner join defect  on h.VmiResult = defect.Id
	)

	WHERE SerialNumber not like IaviSerialNumber
	AND CreatedAt >= @dtStartDate
	AND UpdatedAt <= @dtEndDate

	order by CreatedAt desc
END
