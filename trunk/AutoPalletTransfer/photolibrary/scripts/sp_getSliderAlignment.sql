SELECT pallets.id as palletId,
    pallets.palletSN,
    pallets.processdatetime,
    pallets.LotNumber,
    hgainfo.hgainfoId,
    hgainfo.serialNumber,
    hgainfo.position,
    IF(hgainfo.iaviResult='REJECT', GROUP_CONCAT(DISTINCT coalesce(defectIAVI.defectName, '') order by defectIAVI.defectName SEPARATOR ' '), hgainfo.iaviResult) as iaviResult,
    hgainfo.A_Location,
    hgainfo.B_Location,
    hgainfo.Skew,
    hgainfo.Pitch_Offset,
    hgainfo.Roll_Offset
FROM hgainfo
    INNER JOIN pallets on pallets.id = hgainfo.palletId
    INNER JOIN hgainfo_view on hgainfo_view.hgainfoId = hgainfo.hgainfoId
    LEFT JOIN hgadefect on hgadefect.hgainfo_view_id = hgainfo_view.id
    LEFT JOIN defect defectIAVI on defectIAVI.id = hgadefect.defectId
WHERE
    pallets.created_at >= start_date AND
    pallets.created_at <= end_date AND
    pallets.LotNumber LIKE lot 
GROUP BY
    pallets.id,
    pallets.palletSN,
    pallets.processdatetime,
    pallets.LotNumber,
    hgainfo.hgainfoId,
    hgainfo.serialNumber,
    hgainfo.position,
    hgainfo.A_Location,
    hgainfo.B_Location,
    hgainfo.Skew,
    hgainfo.Pitch_Offset,
    hgainfo.Roll_Offset
    