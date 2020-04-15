----------------------- =SELECT DIFF TIME -------------------

SELECT t1.*, t3.created_at as finish_at,
    (TIME_TO_SEC(t3.created_at) - TIME_TO_SEC(t1.created_at)) as diff,
    COUNT(hgainfo.hgainfoId),
    aqtraydata.LotNumber
FROM trays t1
    INNER JOIN trays t3 on t3.id = (
        SELECT id
        FROM trays t2
        WHERE t1.id < t2.id
        ORDER BY t2.id 
        LIMIT 1
    )
    INNER JOIN pallets on pallets.trayId = t1.id
    INNER JOIN hgainfo on hgainfo.palletId = pallets.id
    INNER JOIN aqtraydata on aqtraydata.TrayId = t1.id
WHERE t1.id >= 285 and t1.id <= 318
GROUP BY t1.id, t1.traySN, t1.AQTrayHeader, t1.isCompleted, t1.created_at, t1.updated_at,
    aqtraydata.LotNumber





----------------------- =UPH -------------------
SELECT sum(x.diff_created_at), sum(x.diff_updated_at), sum(x.count)
FROM (SELECT t1.id,
    (TIME_TO_SEC(t3.created_at) - TIME_TO_SEC(t1.created_at)) as diff_created_at,
    (TIME_TO_SEC(t3.updated_at) - TIME_TO_SEC(t1.updated_at)) as diff_updated_at,
    COUNT(hgainfo.hgainfoId) as count
FROM trays t1
    INNER JOIN trays t3 on t3.id = (
        SELECT id
        FROM trays t2
        WHERE t1.id < t2.id
        ORDER BY t2.id 
        LIMIT 1
    )
    INNER JOIN pallets on pallets.trayId = t1.id
    INNER JOIN hgainfo on hgainfo.palletId = pallets.id
    INNER JOIN aqtraydata on aqtraydata.TrayId = t1.id
WHERE t1.id >= 285 and t1.id <= 317
GROUP BY t1.id
) x



----------------------- =Yield -------------------
SELECT hgainfo.vmiResult, defect.defectName AS vmiDefectName, hgainfo.iaviResult, COUNT(*) as CountDefect
FROM trays
    INNER JOIN pallets on pallets.trayId = trays.id
    INNER JOIN hgainfo on hgainfo.palletId = pallets.id
    INNER JOIN defect on defect.id = hgainfo.vmiResult
-- WHERE trays.id >= 285 and trays.id <= 318
WHERE trays.isCompleted = 1 and 
    trays.created_at >= '2017-04-07 14:00:00' and trays.created_at < '2017-04-07 18:00:00'
GROUP BY defect.defectName, hgainfo.vmiResult, hgainfo.iaviResult
ORDER BY CountDefect DESC



----------------------- =VOR Performance -------------------
SELECT vmiDefectName, iaviDefectName, COUNT(*) as CountDefect
FROM (
    SELECT hgainfo.hgainfoId, defectVMI.defectName AS vmiDefectName, GROUP_CONCAT(DISTINCT defectIAVI.defectName SEPARATOR ' ') as iaviDefectName
    FROM trays
        INNER JOIN pallets on pallets.trayId = trays.id
        INNER JOIN hgainfo on hgainfo.palletId = pallets.id
        INNER JOIN defect as defectVMI on defectVMI.id = hgainfo.vmiResult
        INNER JOIN hgainfo_view on hgainfo_view.hgainfoId = hgainfo.hgainfoId
        INNER JOIN hgadefect on hgadefect.hgainfo_view_id = hgainfo_view.id
        INNER JOIN defect as defectIAVI on defectIAVI.id = hgadefect.defectId
    -- WHERE trays.id >= 285 and trays.id <= 318
    WHERE trays.isCompleted = 1 and 
        trays.created_at >= '2017-04-07 14:00:00' and trays.created_at < '2017-04-07 18:00:00'
    GROUP BY hgainfoId
) tt
GROUP BY vmiDefectName, iaviDefectName
ORDER BY CountDefect DESC


----------------------- =VOR Performance update -------------------
SELECT vmiDefectName, iaviDefectName, COUNT(*) as CountDefect
FROM (
    SELECT hgainfo.hgainfoId, defectVMI.defectName AS vmiDefectName, 
        IF(hgainfo.iaviResult='REJECT', GROUP_CONCAT(DISTINCT coalesce(defectIAVI.defectName, '') SEPARATOR ' '), hgainfo.iaviResult) as iaviDefectName
    FROM trays
        INNER JOIN pallets on pallets.trayId = trays.id
        INNER JOIN hgainfo on hgainfo.palletId = pallets.id
        INNER JOIN defect as defectVMI on defectVMI.id = hgainfo.vmiResult
        LEFT JOIN hgainfo_view on hgainfo_view.hgainfoId = hgainfo.hgainfoId
        LEFT JOIN hgadefect on hgadefect.hgainfo_view_id = hgainfo_view.id
        LEFT JOIN defect as defectIAVI on defectIAVI.id = hgadefect.defectId
    WHERE trays.id >= 285 and trays.id <= 318
    -- WHERE trays.isCompleted = 1 and 
       -- trays.created_at >= '2017-04-07 14:00:00' and trays.created_at < '2017-04-07 18:00:00'
    GROUP BY hgainfoId
) tt
GROUP BY vmiDefectName, iaviDefectName
ORDER BY CountDefect DESC







    SELECT *
    FROM trays
        INNER JOIN pallets on pallets.trayId = trays.id
        INNER JOIN hgainfo on hgainfo.palletId = pallets.id
        INNER JOIN defect as defectVMI on defectVMI.id = hgainfo.vmiResult
        LEFT JOIN hgainfo_view on hgainfo_view.hgainfoId = hgainfo.hgainfoId
        LEFT JOIN hgadefect on hgadefect.hgainfo_view_id = hgainfo_view.id
        LEFT JOIN defect as defectIAVI on defectIAVI.id = hgadefect.defectId
    WHERE trays.id >= 285 and trays.id <= 318 and hgainfo.hgainfoId=27853
    -- WHERE trays.isCompleted = 1 and 
       -- trays.created_at >= '2017-04-07 14:00:00' and trays.created_at < '2017-04-07 18:00:00'
    GROUP BY hgainfo.hgainfoId

select *
from hgainfo
    LEFT JOIN hgainfo_view on hgainfo_view.hgainfoId = hgainfo.hgainfoId
    LEFT JOIN hgadefect on hgadefect.hgainfo_view_id = hgainfo_view.id
WHERE hgainfo.hgainfoId = 268


----------------------- =VOR Performance update 3 -------------------
SELECT vmiDefectName, iaviDefectName, hgainfoId, hgainfo_view_id
FROM (
    SELECT hgainfo.hgainfoId, defectVMI.defectName AS vmiDefectName, 
        IF(hgainfo.iaviResult='REJECT', GROUP_CONCAT(DISTINCT defectIAVI.defectName SEPARATOR ' '), hgainfo.iaviResult) as iaviDefectName,
        hgainfo_view.id as hgainfo_view_id
    FROM trays
        INNER JOIN pallets on pallets.trayId = trays.id
        INNER JOIN hgainfo on hgainfo.palletId = pallets.id
        INNER JOIN defect as defectVMI on defectVMI.id = hgainfo.vmiResult
        LEFT JOIN hgainfo_view on hgainfo_view.hgainfoId = hgainfo.hgainfoId
        LEFT JOIN hgadefect on hgadefect.hgainfo_view_id = hgainfo_view.id
        LEFT JOIN defect as defectIAVI on defectIAVI.id = hgadefect.defectId
    WHERE trays.id >= 285 and trays.id <= 318
    -- WHERE trays.isCompleted = 1 and 
       -- trays.created_at >= '2017-04-07 14:00:00' and trays.created_at < '2017-04-07 18:00:00'
    GROUP BY hgainfo_view.id
) tt
WHERE iaviDefectName is null




----------------------- =Search by defect -------------------
SELECT *
FROM hgainfo_view
    INNER JOIN hgadefect on hgadefect.hgainfo_view_id = hgainfo_view.id
    INNER JOIN defect on defect.id = hgadefect.defectId
    INNER JOIN hgainfo on hgainfo.hgainfoId = hgainfo_view.hgainfoId
    INNER JOIN pallets on pallets.id = hgainfo.palletId
    INNER JOIN trays on trays.id = pallets.trayId
WHERE defect.defectName = "OCR"
ORDER BY hgadefect.id DESC





----------------------- =check by lot -------------------
SET @LotNumberVar = "WYL4A_DB2";

SELECT COUNT(hgainfo.hgainfoId)
FROM trays
    INNER JOIN pallets on pallets.trayId = trays.id
    INNER JOIN hgainfo on hgainfo.palletId = pallets.id
    INNER JOIN aqtraydata on aqtraydata.TrayId = trays.id
WHERE trays.isCompleted = 1 and trays.created_at >= '2017-04-24'
    and LotNumber=@LotNumberVar
    and (hgainfo.vmiResult=2 or (hgainfo.vmiResult=1 and hgainfo.iaviResult="GOOD"));
    
SELECT COUNT(hgainfo.hgainfoId)
FROM trays
    INNER JOIN pallets on pallets.trayId = trays.id
    INNER JOIN hgainfo on hgainfo.palletId = pallets.id
    INNER JOIN aqtraydata on aqtraydata.TrayId = trays.id
WHERE trays.isCompleted = 1 and trays.created_at >= '2017-04-24'
    and LotNumber=@LotNumberVar
    AND hgainfo.iaviResult <> "NOHGA"

-----
SET @LotNumberVar = "WYKUA_CB2";

SELECT LotNumber, COUNT(hgainfo.hgainfoId)
FROM trays
    INNER JOIN pallets on pallets.trayId = trays.id
    INNER JOIN hgainfo on hgainfo.palletId = pallets.id
    INNER JOIN aqtraydata on aqtraydata.TrayId = trays.id
WHERE trays.isCompleted = 1 and trays.created_at >= '2017-04-24'
    and (hgainfo.vmiResult=2 or (hgainfo.vmiResult=1 and hgainfo.iaviResult="GOOD"))
GROUP BY LotNumber;
    
SELECT LotNumber, COUNT(hgainfo.hgainfoId)
FROM trays
    INNER JOIN pallets on pallets.trayId = trays.id
    INNER JOIN hgainfo on hgainfo.palletId = pallets.id
    INNER JOIN aqtraydata on aqtraydata.TrayId = trays.id
WHERE trays.isCompleted = 1 and trays.created_at >= '2017-04-24'
    AND hgainfo.iaviResult <> "NOHGA"
GROUP BY LotNumber
----------------------

SELECT LotNumber, 
    SUM(IF(hgainfo.vmiResult=2 or (hgainfo.vmiResult=1 and hgainfo.iaviResult="GOOD"), 1, 0)) as GOOD,
    SUM(IF(hgainfo.iaviResult <> "NOHGA", 1, 0)) as HAV_HGA,
    COUNT(hgainfo.hgainfoId) as ALL_
FROM trays
    INNER JOIN pallets on pallets.trayId = trays.id
    INNER JOIN hgainfo on hgainfo.palletId = pallets.id
    INNER JOIN aqtraydata on aqtraydata.TrayId = trays.id
WHERE trays.isCompleted = 1 and trays.created_at >= '2017-04-24'
GROUP BY LotNumber;


----- use value in row to be column, like pivot function... 
----- but mysql need to use prepared statement
-- http://stackoverflow.com/questions/12004603/mysql-pivot-row-into-dynamic-number-of-columns
SET @@group_concat_max_len = 32000;
SET @sql = NULL;
SELECT
  GROUP_CONCAT(DISTINCT
    CONCAT(
      'count(case when defectName = ''',
      defectName,
      ''' then 1 end) AS ', defectName
    )
  ) INTO @sql
from defect;

SET @sql = CONCAT('SELECT tt.hgainfoId, ', @sql, ' 
    from (
        SELECT hgainfo.hgainfoId, defectName
        from hgainfo
            INNER JOIN hgainfo_view on hgainfo_view.hgainfoId = hgainfo.hgainfoId
            LEFT JOIN hgadefect on hgadefect.hgainfo_view_id = hgainfo_view.id
            INNER JOIN defect on defect.id = hgadefect.defectId
        group by hgainfo.hgainfoId
    ) tt
    GROUP BY hgainfoId');

SELECT @sql;
PREPARE stmt FROM @sql;
EXECUTE stmt;
DEALLOCATE PREPARE stmt;



----------------------- =get hga by view -------------------
SET @start_date = '2017-05-30 14:35:00';
SET @end_date = '2017-05-30 15:30:00';

SELECT DISTINCT pp.id as PalletId, pp.palletSN, pp.created_at,
    hgainfo.hgainfoId, hgainfo.serialNumber, hgainfo.position, defectVMI.defectName as vmiResult,
    IF(hgainfo.iaviResult='REJECT', GROUP_CONCAT(DISTINCT coalesce(defectIAVI.defectName, '') SEPARATOR ' '), hgainfo.iaviResult) as iaviResult,
    view.name,
    concat('\\\\172.16.53.13\\Images\\', hgainfo_view.imagePath) as imagePath
FROM hgainfo_view
    LEFT JOIN hgadefect on hgadefect.hgainfo_view_id = hgainfo_view.id
    LEFT JOIN defect defectIAVI on defectIAVI.id = hgadefect.defectId
    INNER JOIN hgainfo on hgainfo.hgainfoId = hgainfo_view.hgainfoId
    INNER JOIN defect defectVMI on defectVMI.id = hgainfo.vmiResult
    inner join (
        SELECT *
        FROM pallets
        WHERE created_at >= @start_date and created_at <= @end_date and
            pallets.id = (SELECT max(id) from pallets p1 where p1.palletSN = pallets.palletSN and created_at >= @start_date and created_at <= @end_date)
        ORDER BY pallets.created_at DESC
    ) pp on pp.id=hgainfo.palletId
    -- INNER JOIN trays on trays.id = pallets.trayId
    INNER JOIN view on view.id = hgainfo_view.viewId
WHERE 
    -- view.name = 'TopABS'
    view.name = 'TopPZT'
    -- (view.name = 'FortyFive' or view.name = 'TopPZT' or view.name = 'TopAlignment')
    and pp.created_at >= @start_date
    and pp.created_at <= @end_date
    -- and defectIAVI.defectname = 'A1'
GROUP BY PalletId, pp.palletSN, pp.created_at,
    hgainfo.hgainfoId, hgainfo.serialNumber, hgainfo.position, vmiResult, defectVMI.defectName,
    view.name, imagePath
ORDER BY hgainfo.hgainfoId




----------------------- =Over/Under reject -------------------
SET @start_date = '2017-07-24';
SET @end_date = '2017-07-25';
SET @lot = '%';

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
        trays.created_at >= @start_date AND
        trays.created_at <= @end_date AND
        pallets.LotNumber LIKE @lot AND
        trays.isCompleted = 1
        -- trays.LotNumber = ''
) t1
CROSS JOIN
(
    SELECT COUNT(*) AS Total 
    FROM trays_hga
    WHERE 
        trays_hga.created_at >= @start_date AND
        trays_hga.created_at <= @end_date AND
        trays_hga.LotNumber LIKE @lot AND
        trays_hga.isCompleted = 1
) t2
GROUP BY defectVMI, defectIAVI;





----------------------- =get slider alignment parameter -------------------
SET @start_date = '2017-07-26';
SET @end_date = '2017-07-27';
SET @lot = '%';

SELECT pallets.id as palletId,
    pallets.palletSN,
    pallets.processdatetime,
    pallets.LotNumber,
    hgainfo.serialNumber,
    hgainfo.position,
    hgainfo.A_Location,
    hgainfo.B_Location,
    hgainfo.Skew,
    hgainfo.Pitch_Offset,
    hgainfo.Roll_Offset
FROM hgainfo
    INNER JOIN pallets on pallets.id = hgainfo.palletId
WHERE
    pallets.created_at >= @start_date AND
    pallets.created_at <= @end_date AND
    pallets.LotNumber LIKE @lot 
