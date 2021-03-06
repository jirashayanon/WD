USE [TempPalletTrackingDB]
GO
/****** Object:  StoredProcedure [dbo].[spSearchAQTrayBarcode]    Script Date: 11/28/2017 14:16:33 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Prasert
-- Create date: 
-- Description:	
-- =============================================

-- EXEC spSearchAQTrayBarcode '6W4068367T'


ALTER PROCEDURE [dbo].[spSearchAQTrayBarcode] 
	-- Add the parameters for the stored procedure here
	@aqtrayBarcode varchar(10)

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
		select tray.TraySN, tray.IsCompleted, tray.CreatedAt, tray.UpdatedAt, 
		aqtraydata.TimeStart, aqtraydata.TimeEnd, aqtraydata.TesterNumber, aqtraydata.Customer,
		aqtraydata.Product, aqtraydata.[User], aqtraydata.LotNumber,
		aqtraydata.DocControl1, aqtraydata.DocControl2, aqtraydata.Sus, aqtraydata.AssyLine

		from tray
		inner join aqtraydata on tray.Id = aqtraydata.TrayId
		
		where tray.TraySN = @aqtrayBarcode
		
		order by tray.CreatedAt desc
END
