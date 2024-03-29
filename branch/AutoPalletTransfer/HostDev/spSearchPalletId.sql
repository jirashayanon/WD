USE [TempPalletTrackingDB]
GO
/****** Object:  StoredProcedure [dbo].[spSearchPalletId]    Script Date: 11/28/2017 14:17:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		Nasakol P.
-- Create date: 2017-08-16
-- Description:	Use to search pallet and hgainfo
-- =============================================
ALTER PROCEDURE [dbo].[spSearchPalletId]
	-- Add the parameters for the stored procedure here
	@palletIdParam varchar(20)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
    select hgainfo.*,
		hgainfoView.viewId,
		hgainfoView.imagePath,
		hgainfoView.result,
		pp.palletSN,
		pp.isCompleted
	from hgainfo
		left join hgainfoView on hgainfoView.hgainfoId = hgainfo.Id
		inner join (
			SELECT TOP 1 *
			FROM pallet
			WHERE pallet.palletSN = @palletIdParam
			ORDER BY pallet.CreatedAt DESC
		) pp on pp.id=hgainfo.palletId
END

