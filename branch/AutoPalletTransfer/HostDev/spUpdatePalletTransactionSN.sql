USE [TempPalletTrackingDB]
GO
/****** Object:  StoredProcedure [dbo].[spUpdatePalletTransactionSN]    Script Date: 08/02/2017 10:21:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Prasert
-- Create date: 
-- Description:	
-- =============================================

-- EXEC [spUpdatePalletTransactionSN] 4, 'PT0001', 20, 30,
-- 'WYKU871H','J4B',
-- 'WYKU871C','J4B',
-- 'WYKU7692','J4B',
-- 'WYKU769X','J4B',
-- 'WYKU769W','J4B',
-- 'WYKU769V','J4B',
-- 'WYKU769R','J4B',
-- 'WYKU769P','J4B',
-- 'WYKU771P','J4B',
-- 'WYKU771K','J4B'


ALTER PROCEDURE [dbo].[spUpdatePalletTransactionSN] 
	@transID int,
	@palletID varchar(6),
	@equipmentType tinyint,
	@nextEquipmentType tinyint,
	
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

	SELECT @transID, @palletID, @equipmentType, @nextEquipmentType, 
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
