USE [TempPalletTrackingDB]
GO
/****** Object:  StoredProcedure [dbo].[spUpdatePalletTransactionNoLotNoSNILC]    Script Date: 08/02/2017 10:20:11 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Prasert
-- Create date: 
-- Description:	
-- =============================================

-- EXEC [spUpdatePalletTransactionNoLotNoSNILC] 407, 'PT0095', 30, 40, 0, 0.1, 0	--ILC -> SJB


ALTER PROCEDURE [dbo].[spUpdatePalletTransactionNoLotNoSNILC] 
	@transID int,
	@palletID varchar(6),
	@equipmentType tinyint,
	@nextEquipmentType tinyint,
	@ilcUVPower int,
	@ilcCureTime float,
	@ilcCureZone int

AS
BEGIN

	SET NOCOUNT ON;

	UPDATE tblPalletTransaction
	SET EquipmentType = @equipmentType,
		NextEquipmentType = @nextEquipmentType,
		ILCUVPower = @ilcUVPower,
		ILCCureTime = @ilcCureTime,
		ILCCureZone = @ilcCureZone

	WHERE PalletID = @palletID AND TransID = @transID

	SELECT @transID, @palletID, @equipmentType, @nextEquipmentType, @ilcUVPower, @ilcCureTime, @ilcCureZone
END
