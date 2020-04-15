CREATE OR REPLACE VIEW trays_hga AS

SELECT pallets.id as palletId,
    pallets.palletSN,
    pallets.palletOrder,
    hgainfo.hgainfoId,
    hgainfo.serialNumber,
    hgainfo.position,
    hgainfo.iaviResult,
    hgainfo.vmiResult,
    trays.id as trayId,
    trays.traySN,
    trays.isCompleted,
    trays.created_at,
    pallets.lotNumber
FROM hgainfo
    INNER JOIN pallets on pallets.id = hgainfo.palletId
    INNER JOIN trays on trays.id = pallets.trayId
ORDER BY trays.id DESC, hgainfo.hgainfoId ASC;
