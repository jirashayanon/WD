USE [TempPalletTrackingDB]
GO
/****** Object:  StoredProcedure [dbo].[spUpdatePalletTransactionNoLotNoSN]    Script Date: 08/02/2017 10:20:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Prasert
-- Create date: 
-- Description:	
-- =============================================

-- EXEC [spUpdatePalletTransactionNoLotNoSN] 4, 'PT0107', 60, 10	--UNLOAD -> ASLV
-- EXEC [spUpdatePalletTransactionNoLotNoSN] 230, 'PT0001', 20, 30	


ALTER PROCEDURE [dbo].[spUpdatePalletTransactionNoLotNoSN] 
	@transID int,
	@palletID varchar(6),
	@equipmentType tinyint,
	@nextEquipmentType tinyint

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	UPDATE tblPalletTransaction
	SET EquipmentType = @equipmentType,
		NextEquipmentType = @nextEquipmentType

	WHERE PalletID = @palletID AND TransID = @transID

	SELECT @transID, @palletID, @equipmentType, @nextEquipmentType
END
