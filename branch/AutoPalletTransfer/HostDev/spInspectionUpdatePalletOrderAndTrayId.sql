USE [TempPalletTrackingDB]
GO
/****** Object:  StoredProcedure [dbo].[spInspectionUpdatePalletOrderAndTrayId]    Script Date: 11/28/2017 14:14:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Nasakol P.
-- Create date: 2017-08-17
-- Description:	Update palletOrder and trayId on latest pallet.
-- =============================================
ALTER PROCEDURE [dbo].[spInspectionUpdatePalletOrderAndTrayId]
	-- Add the parameters for the stored procedure here
	@palletSN varchar(20),
	@palletOrder int,
	@trayId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	UPDATE p1
    SET p1.palletOrder = @palletOrder, p1.trayId = @trayId
	FROM pallet p1
        INNER JOIN (
            SELECT TOP 1 *
            FROM pallet
            WHERE palletSN = @palletSN
            ORDER BY processdatetime DESC, id DESC
        ) p2 on p1.id = p2.id
    WHERE p1.trayId is null;
END

