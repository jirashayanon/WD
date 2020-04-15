SELECT 
    defectVMI,
    defectIAVI,
    SUM(IF(defectVMI = 'Good' AND (defectIAVI is not null), 1, 0)) as OverReject,
    SUM(IF(defectVMI <> 'None' AND defectVMI <> 'Good' AND (defectIAVI is null), 1, 0)) as UnderReject,
    SUM(IF(defectVMI = 'None' AND (defectIAVI is not null), 1, 0)) as UnknownReject,
    SUM(IF( (defectVMI <> 'None' AND defectVMI <> 'Good' and defectIAVI is not null) or
        ((defectVMI = 'None' or defectVMI = 'Good') and defectIAVI is null), 1, 0)) as MatchAtleast,
    Total
FROM  (SELECT 
        trays.id,
        trays.traySN,
        trays.created_at,
        pallets.palletSN,
        pallets.palletOrder,
        hgainfo.position,
        hgainfo.hgainfoId,
        hgainfo.serialNumber,
        defectVMI.defectName as defectVMI,
        defectIAVI.defectName as defectIAVI
    FROM trays
        INNER JOIN pallets on pallets.trayId = trays.id
        INNER JOIN hgainfo on hgainfo.palletId = pallets.id
        LEFT JOIN defect defectIAVI on defectIAVI.id = (
            SELECT  defect.id
            FROM    hgainfo_view
                INNER JOIN hgadefect on hgadefect.hgainfo_view_id = hgainfo_view.id
                INNER JOIN defect on defect.id = hgadefect.defectId
            WHERE   hgainfo_view.hgainfoId = hgainfo.hgainfoId
            ORDER BY defect.priority
            LIMIT 1
        )
        INNER JOIN defect defectVMI on defectVMI.id = hgainfo.vmiResult
    WHERE
        trays.created_at >= start_date AND
        trays.created_at <= end_date AND
        pallets.LotNumber LIKE lot AND
        trays.isCompleted = 1
        -- trays.LotNumber = ''
) t1
CROSS JOIN
(
    SELECT COUNT(*) AS Total 
    FROM trays_hga
    WHERE 
        trays_hga.created_at >= start_date AND
        trays_hga.created_at <= end_date AND
        trays_hga.LotNumber LIKE lot AND
        trays_hga.isCompleted = 1
) t2
GROUP BY defectVMI, defectIAVI;
