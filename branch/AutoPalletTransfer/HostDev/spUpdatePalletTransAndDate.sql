USE [TempPalletTrackingDB]
GO
/****** Object:  StoredProcedure [dbo].[spUpdatePalletTransAndDate]    Script Date: 08/02/2017 10:22:14 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Prasert
-- Create date: 
-- Description:	
-- =============================================

-- EXEC [spUpdatePalletTransAndDate] 3, 'PT0001', 20, 30, '05-03-2017 16:40:00 PM', 'WYKU7_AB2',
-- 'WYKU871H','A1,WO',
-- 'WYKU871C','A1,WO',
-- 'WYKU7692','A1,WO',
-- 'WYKU769X','A1,WO',
-- 'WYKU769W','A1,WO',
-- 'WYKU769V','A1,WO',
-- 'WYKU769R','A1,WO',
-- 'WYKU769P','A1,WO',
-- 'WYKU771P','A1,WO',
-- 'WYKU771K','A1,WO'


-- EXEC [spUpdatePalletTransAndDate] 4, 'PT0001', 20, 30, '05-03-2017 16:42:00 PM', 'WYKU7_A',
-- 'WYKU871H','',
-- 'WYKU871C','',
-- 'WYKU7692','',
-- 'WYKU769X','',
-- 'WYKU769W','',
-- 'WYKU769V','',
-- 'WYKU769R','',
-- 'WYKU769P','',
-- 'WYKU771P','',
-- 'WYKU771K',''


ALTER PROCEDURE [dbo].[spUpdatePalletTransAndDate] 
	@transID int,
	@palletID varchar(6),
	@equipmentType tinyint,
	@nextEquipmentType tinyint,
	@createdDateTime DATETIME,
	@lotNumber varchar(20),
	
	@hgaSN1 varchar(10),
	@hgaDefect1 varchar(20),
	@hgaSN2 varchar(10),
	@hgaDefect2 varchar(20),
	@hgaSN3 varchar(10),
	@hgaDefect3 varchar(20),
	@hgaSN4 varchar(10),
	@hgaDefect4 varchar(20),
	@hgaSN5 varchar(10),
	@hgaDefect5 varchar(20),				
	
	@hgaSN6 varchar(10),
	@hgaDefect6 varchar(20),
	@hgaSN7 varchar(10),
	@hgaDefect7 varchar(20),
	@hgaSN8 varchar(10),
	@hgaDefect8 varchar(20),
	@hgaSN9 varchar(10),
	@hgaDefect9 varchar(20),
	@hgaSN10 varchar(10),
	@hgaDefect10 varchar(20)		
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE tblPalletTransaction
	SET EquipmentType = @equipmentType,
		NextEquipmentType = @nextEquipmentType,
		CreatedDateTime = @createdDateTime,
		LotNumber = @lotNumber,
		HGASN_1 = @hgaSN1,
		HGADefect_1 = @hgaDefect1,
		HGASN_2 = @hgaSN2,
		HGADefect_2 = @hgaDefect2,
		HGASN_3 = @hgaSN3,
		HGADefect_3 = @hgaDefect3,		
		HGASN_4 = @hgaSN4,
		HGADefect_4 = @hgaDefect4,		
		HGASN_5 = @hgaSN5,
		HGADefect_5 = @hgaDefect5,		
		HGASN_6 = @hgaSN6,
		HGADefect_6 = @hgaDefect6,		
		HGASN_7 = @hgaSN7,
		HGADefect_7 = @hgaDefect7,		
		HGASN_8 = @hgaSN8,
		HGADefect_8 = @hgaDefect8,		
		HGASN_9 = @hgaSN9,
		HGADefect_9 = @hgaDefect9,
		HGASN_10 = @hgaSN10,
		HGADefect_10 = @hgaDefect10	
						
	WHERE PalletID = @palletID AND TransID = @transID

	SELECT @transID, @palletID, @equipmentType, @nextEquipmentType, @createdDateTime, @lotNumber, 
	@hgaSN1, @hgaDefect1, 
	@hgaSN2, @hgaDefect2, 
	@hgaSN3, @hgaDefect3, 
	@hgaSN4, @hgaDefect4, 
	@hgaSN5, @hgaDefect5, 
	@hgaSN6, @hgaDefect6, 
	@hgaSN7, @hgaDefect7, 
	@hgaSN8, @hgaDefect8, 
	@hgaSN9, @hgaDefect9, 
	@hgaSN10, @hgaDefect10	
END
