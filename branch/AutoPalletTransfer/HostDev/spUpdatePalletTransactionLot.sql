USE [TempPalletTrackingDB]
GO
/****** Object:  StoredProcedure [dbo].[spUpdatePalletTransactionLot]    Script Date: 08/02/2017 10:19:50 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Prasert
-- Create date: 
-- Description:	
-- =============================================

-- EXEC [spUpdatePalletTransactionLot] 1, 'PT0001', 80, 30, 'WRXGC_A', 'APT001'
-- EXEC [spUpdatePalletTransactionLot] 3, 'PT0071', 10, 20, 'WR19X_AB2', ''

ALTER PROCEDURE [dbo].[spUpdatePalletTransactionLot] 
	@transID int,
	@palletID varchar(6),
	@equipmentType tinyint,
	@nextEquipmentType tinyint,
	@lotNumber varchar(20),
	@acamID varchar(6)
AS
BEGIN
	SET NOCOUNT ON;

	UPDATE tblPalletTransaction
	SET EquipmentType = @equipmentType,
		NextEquipmentType = @nextEquipmentType,
		LotNumber = @lotNumber,
		ACAMID = @acamID
						
	WHERE PalletID = @palletID AND TransID = @transID

	SELECT @transID, @palletID, @equipmentType, @nextEquipmentType, @lotNumber, @acamID
END
