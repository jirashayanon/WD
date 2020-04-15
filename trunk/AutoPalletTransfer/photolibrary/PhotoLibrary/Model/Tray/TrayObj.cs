using System;
using System.IO;
using System.Text;
using System.Xml;

public class TrayObj
{
    /*1*/
    private const string CONST_DATE = "DATE=";
    /*2*/
    private const string CONST_TIME_START = "TIME START=";
    /*3*/
    private const string CONST_TIME_END = "TIME END=";
    /*4*/
    private const string CONST_USED_TIME = "USED TIME=";
    /*5*/
    private const string CONST_TRAY_ID = "TRAYID=";

    /*6*/
    private const string CONST_PALLET_ID1 = "PALLETID1=";
    /*7*/
    private const string CONST_PALLET_ID2 = "PALLETID2=";
    /*8*/
    private const string CONST_PALLET_ID3 = "PALLETID3=";
    /*9*/
    private const string CONST_PALLET_ID4 = "PALLETID4=";
    /*10*/
    private const string CONST_PALLET_ID5 = "PALLETID5=";
    /*11*/
    private const string CONST_PALLET_ID6 = "PALLETID6=";

    private const string CONST_DATETIME_FORMAT = "M/d/yyyy h:mm:ss tt";

    private DateTime _dtDate = new DateTime();
    public string Date
    {
        get { return _dtDate.ToString("dd/mm/yyyy"); }
        set { _dtDate = DateTime.ParseExact(value, "dd/mm/yyyy", System.Globalization.CultureInfo.InvariantCulture); }
    }

    private DateTime _dtTimeStart = new DateTime();
    public string TimeStart
    {
        get { return _dtTimeStart.ToString(CONST_DATETIME_FORMAT); }
        set
        {
            try
            {
                _dtTimeStart = DateTime.Parse(value);
            }
            catch (Exception ex)
            {
            }
        }
    }

    public void SetTimeStart(DateTime dtStart)
    {
        _dtTimeStart = dtStart;
    }


    private DateTime _dtTimeEnd = new DateTime();
    public string TimeEnd
    {
        get { return _dtTimeEnd.ToString(CONST_DATETIME_FORMAT); }
        set
        {
            try
            {
                _dtTimeEnd = DateTime.Parse(value);
            }
            catch (Exception ex)
            {
            }
        }
    }

    public void SetTimeEnd(DateTime dtEnd)
    {
        _dtTimeEnd = dtEnd;
    }


    private string _strUsedTime = string.Empty;
    public string UsedTime
    {
        get
        {
            TimeSpan duration = (DateTime.Parse(_dtTimeEnd.ToString(CONST_DATETIME_FORMAT))).Subtract(DateTime.Parse(_dtTimeStart.ToString(CONST_DATETIME_FORMAT)));
            _strUsedTime = duration.ToString(@"mm\:ss");

            return _strUsedTime;
        }

        set
        {
            TimeSpan duration = (DateTime.Parse(_dtTimeEnd.ToString(CONST_DATETIME_FORMAT))).Subtract(DateTime.Parse(_dtTimeStart.ToString(CONST_DATETIME_FORMAT)));
            _strUsedTime = duration.ToString(@"mm\:ss");
        }
    }

    private string _strTrayID = string.Empty;
    public string TrayID
    {
        get { return _strTrayID; }
        set { _strTrayID = value; }
    }

    private string _strPallet1 = string.Empty;
    public string Pallet1
    {
        get { return _strPallet1; }
        set { _strPallet1 = value; }
    }

    private string _strPallet2 = string.Empty;
    public string Pallet2
    {
        get { return _strPallet2; }
        set { _strPallet2 = value; }
    }

    private string _strPallet3 = string.Empty;
    public string Pallet3
    {
        get { return _strPallet3; }
        set { _strPallet3 = value; }
    }

    private string _strPallet4 = string.Empty;
    public string Pallet4
    {
        get { return _strPallet4; }
        set { _strPallet4 = value; }
    }

    private string _strPallet5 = string.Empty;
    public string Pallet5
    {
        get { return _strPallet5; }
        set { _strPallet5 = value; }
    }

    private string _strPallet6 = string.Empty;
    public string Pallet6
    {
        get { return _strPallet6; }
        set { _strPallet6 = value; }
    }



    #region HGAN
    private string _strHGAN1 = string.Empty;
    public string HGAN1
    {
        get { return _strHGAN1; }
        set { _strHGAN1 = value; }

    }

    private string _strHGAN2 = string.Empty;
    public string HGAN2
    {
        get { return _strHGAN2; }
        set { _strHGAN2 = value; }

    }

    private string _strHGAN3 = string.Empty;
    public string HGAN3
    {
        get { return _strHGAN3; }
        set { _strHGAN3 = value; }

    }

    private string _strHGAN4 = string.Empty;
    public string HGAN4
    {
        get { return _strHGAN4; }
        set { _strHGAN4 = value; }

    }

    private string _strHGAN5 = string.Empty;
    public string HGAN5
    {
        get { return _strHGAN5; }
        set { _strHGAN5 = value; }

    }

    private string _strHGAN6 = string.Empty;
    public string HGAN6
    {
        get { return _strHGAN6; }
        set { _strHGAN6 = value; }

    }

    private string _strHGAN7 = string.Empty;
    public string HGAN7
    {
        get { return _strHGAN7; }
        set { _strHGAN7 = value; }

    }

    private string _strHGAN8 = string.Empty;
    public string HGAN8
    {
        get { return _strHGAN8; }
        set { _strHGAN8 = value; }

    }

    private string _strHGAN9 = string.Empty;
    public string HGAN9
    {
        get { return _strHGAN9; }
        set { _strHGAN9 = value; }

    }

    private string _strHGAN10 = string.Empty;
    public string HGAN10
    {
        get { return _strHGAN10; }
        set { _strHGAN10 = value; }

    }

    private string _strHGAN11 = string.Empty;
    public string HGAN11
    {
        get { return _strHGAN11; }
        set { _strHGAN11 = value; }

    }

    private string _strHGAN12 = string.Empty;
    public string HGAN12
    {
        get { return _strHGAN12; }
        set { _strHGAN12 = value; }

    }

    private string _strHGAN13 = string.Empty;
    public string HGAN13
    {
        get { return _strHGAN13; }
        set { _strHGAN13 = value; }

    }

    private string _strHGAN14 = string.Empty;
    public string HGAN14
    {
        get { return _strHGAN14; }
        set { _strHGAN14 = value; }

    }

    private string _strHGAN15 = string.Empty;
    public string HGAN15
    {
        get { return _strHGAN15; }
        set { _strHGAN15 = value; }

    }

    private string _strHGAN16 = string.Empty;
    public string HGAN16
    {
        get { return _strHGAN16; }
        set { _strHGAN16 = value; }

    }

    private string _strHGAN17 = string.Empty;
    public string HGAN17
    {
        get { return _strHGAN17; }
        set { _strHGAN17 = value; }

    }

    private string _strHGAN18 = string.Empty;
    public string HGAN18
    {
        get { return _strHGAN18; }
        set { _strHGAN18 = value; }

    }

    private string _strHGAN19 = string.Empty;
    public string HGAN19
    {
        get { return _strHGAN19; }
        set { _strHGAN19 = value; }

    }

    private string _strHGAN20 = string.Empty;
    public string HGAN20
    {
        get { return _strHGAN20; }
        set { _strHGAN20 = value; }

    }

    private string _strHGAN21 = string.Empty;
    public string HGAN21
    {
        get { return _strHGAN21; }
        set { _strHGAN21 = value; }

    }

    private string _strHGAN22 = string.Empty;
    public string HGAN22
    {
        get { return _strHGAN22; }
        set { _strHGAN22 = value; }

    }

    private string _strHGAN23 = string.Empty;
    public string HGAN23
    {
        get { return _strHGAN23; }
        set { _strHGAN23 = value; }

    }

    private string _strHGAN24 = string.Empty;
    public string HGAN24
    {
        get { return _strHGAN24; }
        set { _strHGAN24 = value; }

    }

    private string _strHGAN25 = string.Empty;
    public string HGAN25
    {
        get { return _strHGAN25; }
        set { _strHGAN25 = value; }

    }

    private string _strHGAN26 = string.Empty;
    public string HGAN26
    {
        get { return _strHGAN26; }
        set { _strHGAN26 = value; }

    }

    private string _strHGAN27 = string.Empty;
    public string HGAN27
    {
        get { return _strHGAN27; }
        set { _strHGAN27 = value; }

    }

    private string _strHGAN28 = string.Empty;
    public string HGAN28
    {
        get { return _strHGAN28; }
        set { _strHGAN28 = value; }

    }

    private string _strHGAN29 = string.Empty;
    public string HGAN29
    {
        get { return _strHGAN29; }
        set { _strHGAN29 = value; }

    }

    private string _strHGAN30 = string.Empty;
    public string HGAN30
    {
        get { return _strHGAN30; }
        set { _strHGAN30 = value; }

    }

    private string _strHGAN31 = string.Empty;
    public string HGAN31
    {
        get { return _strHGAN31; }
        set { _strHGAN31 = value; }

    }

    private string _strHGAN32 = string.Empty;
    public string HGAN32
    {
        get { return _strHGAN32; }
        set { _strHGAN32 = value; }

    }

    private string _strHGAN33 = string.Empty;
    public string HGAN33
    {
        get { return _strHGAN33; }
        set { _strHGAN33 = value; }

    }

    private string _strHGAN34 = string.Empty;
    public string HGAN34
    {
        get { return _strHGAN34; }
        set { _strHGAN34 = value; }

    }

    private string _strHGAN35 = string.Empty;
    public string HGAN35
    {
        get { return _strHGAN35; }
        set { _strHGAN35 = value; }

    }

    private string _strHGAN36 = string.Empty;
    public string HGAN36
    {
        get { return _strHGAN36; }
        set { _strHGAN36 = value; }

    }

    private string _strHGAN37 = string.Empty;
    public string HGAN37
    {
        get { return _strHGAN37; }
        set { _strHGAN37 = value; }

    }

    private string _strHGAN38 = string.Empty;
    public string HGAN38
    {
        get { return _strHGAN38; }
        set { _strHGAN38 = value; }

    }

    private string _strHGAN39 = string.Empty;
    public string HGAN39
    {
        get { return _strHGAN39; }
        set { _strHGAN39 = value; }

    }

    private string _strHGAN40 = string.Empty;
    public string HGAN40
    {
        get { return _strHGAN40; }
        set { _strHGAN40 = value; }

    }

    private string _strHGAN41 = string.Empty;
    public string HGAN41
    {
        get { return _strHGAN41; }
        set { _strHGAN41 = value; }

    }

    private string _strHGAN42 = string.Empty;
    public string HGAN42
    {
        get { return _strHGAN42; }
        set { _strHGAN42 = value; }

    }

    private string _strHGAN43 = string.Empty;
    public string HGAN43
    {
        get { return _strHGAN43; }
        set { _strHGAN43 = value; }

    }

    private string _strHGAN44 = string.Empty;
    public string HGAN44
    {
        get { return _strHGAN44; }
        set { _strHGAN44 = value; }

    }

    private string _strHGAN45 = string.Empty;
    public string HGAN45
    {
        get { return _strHGAN45; }
        set { _strHGAN45 = value; }

    }

    private string _strHGAN46 = string.Empty;
    public string HGAN46
    {
        get { return _strHGAN46; }
        set { _strHGAN46 = value; }

    }

    private string _strHGAN47 = string.Empty;
    public string HGAN47
    {
        get { return _strHGAN47; }
        set { _strHGAN47 = value; }

    }

    private string _strHGAN48 = string.Empty;
    public string HGAN48
    {
        get { return _strHGAN48; }
        set { _strHGAN48 = value; }

    }

    private string _strHGAN49 = string.Empty;
    public string HGAN49
    {
        get { return _strHGAN49; }
        set { _strHGAN49 = value; }

    }

    private string _strHGAN50 = string.Empty;
    public string HGAN50
    {
        get { return _strHGAN50; }
        set { _strHGAN50 = value; }

    }

    private string _strHGAN51 = string.Empty;
    public string HGAN51
    {
        get { return _strHGAN51; }
        set { _strHGAN51 = value; }

    }

    private string _strHGAN52 = string.Empty;
    public string HGAN52
    {
        get { return _strHGAN52; }
        set { _strHGAN52 = value; }

    }

    private string _strHGAN53 = string.Empty;
    public string HGAN53
    {
        get { return _strHGAN53; }
        set { _strHGAN53 = value; }

    }

    private string _strHGAN54 = string.Empty;
    public string HGAN54
    {
        get { return _strHGAN54; }
        set { _strHGAN54 = value; }

    }

    private string _strHGAN55 = string.Empty;
    public string HGAN55
    {
        get { return _strHGAN55; }
        set { _strHGAN55 = value; }

    }

    private string _strHGAN56 = string.Empty;
    public string HGAN56
    {
        get { return _strHGAN56; }
        set { _strHGAN56 = value; }

    }

    private string _strHGAN57 = string.Empty;
    public string HGAN57
    {
        get { return _strHGAN57; }
        set { _strHGAN57 = value; }

    }

    private string _strHGAN58 = string.Empty;
    public string HGAN58
    {
        get { return _strHGAN58; }
        set { _strHGAN58 = value; }

    }

    private string _strHGAN59 = string.Empty;
    public string HGAN59
    {
        get { return _strHGAN59; }
        set { _strHGAN59 = value; }

    }

    private string _strHGAN60 = string.Empty;
    public string HGAN60
    {
        get { return _strHGAN60; }
        set { _strHGAN60 = value; }

    }

    #endregion



    #region DefectN
    //comma separated string format
    private string _strDefectN1 = string.Empty;
    public string DefectN1
    {
        get { return _strDefectN1; }
        set { _strDefectN1 = value; }

    }

    private string _strDefectN2 = string.Empty;
    public string DefectN2
    {
        get { return _strDefectN2; }
        set { _strDefectN2 = value; }

    }

    private string _strDefectN3 = string.Empty;
    public string DefectN3
    {
        get { return _strDefectN3; }
        set { _strDefectN3 = value; }

    }

    private string _strDefectN4 = string.Empty;
    public string DefectN4
    {
        get { return _strDefectN4; }
        set { _strDefectN4 = value; }

    }

    private string _strDefectN5 = string.Empty;
    public string DefectN5
    {
        get { return _strDefectN5; }
        set { _strDefectN5 = value; }

    }

    private string _strDefectN6 = string.Empty;
    public string DefectN6
    {
        get { return _strDefectN6; }
        set { _strDefectN6 = value; }

    }

    private string _strDefectN7 = string.Empty;
    public string DefectN7
    {
        get { return _strDefectN7; }
        set { _strDefectN7 = value; }

    }

    private string _strDefectN8 = string.Empty;
    public string DefectN8
    {
        get { return _strDefectN8; }
        set { _strDefectN8 = value; }

    }

    private string _strDefectN9 = string.Empty;
    public string DefectN9
    {
        get { return _strDefectN9; }
        set { _strDefectN9 = value; }

    }

    private string _strDefectN10 = string.Empty;
    public string DefectN10
    {
        get { return _strDefectN10; }
        set { _strDefectN10 = value; }

    }

    private string _strDefectN11 = string.Empty;
    public string DefectN11
    {
        get { return _strDefectN11; }
        set { _strDefectN11 = value; }

    }

    private string _strDefectN12 = string.Empty;
    public string DefectN12
    {
        get { return _strDefectN12; }
        set { _strDefectN12 = value; }

    }

    private string _strDefectN13 = string.Empty;
    public string DefectN13
    {
        get { return _strDefectN13; }
        set { _strDefectN13 = value; }

    }

    private string _strDefectN14 = string.Empty;
    public string DefectN14
    {
        get { return _strDefectN14; }
        set { _strDefectN14 = value; }

    }

    private string _strDefectN15 = string.Empty;
    public string DefectN15
    {
        get { return _strDefectN15; }
        set { _strDefectN15 = value; }

    }

    private string _strDefectN16 = string.Empty;
    public string DefectN16
    {
        get { return _strDefectN16; }
        set { _strDefectN16 = value; }

    }

    private string _strDefectN17 = string.Empty;
    public string DefectN17
    {
        get { return _strDefectN17; }
        set { _strDefectN17 = value; }

    }

    private string _strDefectN18 = string.Empty;
    public string DefectN18
    {
        get { return _strDefectN18; }
        set { _strDefectN18 = value; }

    }

    private string _strDefectN19 = string.Empty;
    public string DefectN19
    {
        get { return _strDefectN19; }
        set { _strDefectN19 = value; }

    }

    private string _strDefectN20 = string.Empty;
    public string DefectN20
    {
        get { return _strDefectN20; }
        set { _strDefectN20 = value; }

    }

    private string _strDefectN21 = string.Empty;
    public string DefectN21
    {
        get { return _strDefectN21; }
        set { _strDefectN21 = value; }

    }

    private string _strDefectN22 = string.Empty;
    public string DefectN22
    {
        get { return _strDefectN22; }
        set { _strDefectN22 = value; }

    }

    private string _strDefectN23 = string.Empty;
    public string DefectN23
    {
        get { return _strDefectN23; }
        set { _strDefectN23 = value; }

    }

    private string _strDefectN24 = string.Empty;
    public string DefectN24
    {
        get { return _strDefectN24; }
        set { _strDefectN24 = value; }

    }

    private string _strDefectN25 = string.Empty;
    public string DefectN25
    {
        get { return _strDefectN25; }
        set { _strDefectN25 = value; }

    }

    private string _strDefectN26 = string.Empty;
    public string DefectN26
    {
        get { return _strDefectN26; }
        set { _strDefectN26 = value; }

    }

    private string _strDefectN27 = string.Empty;
    public string DefectN27
    {
        get { return _strDefectN27; }
        set { _strDefectN27 = value; }

    }

    private string _strDefectN28 = string.Empty;
    public string DefectN28
    {
        get { return _strDefectN28; }
        set { _strDefectN28 = value; }

    }

    private string _strDefectN29 = string.Empty;
    public string DefectN29
    {
        get { return _strDefectN29; }
        set { _strDefectN29 = value; }

    }

    private string _strDefectN30 = string.Empty;
    public string DefectN30
    {
        get { return _strDefectN30; }
        set { _strDefectN30 = value; }

    }

    private string _strDefectN31 = string.Empty;
    public string DefectN31
    {
        get { return _strDefectN31; }
        set { _strDefectN31 = value; }

    }

    private string _strDefectN32 = string.Empty;
    public string DefectN32
    {
        get { return _strDefectN32; }
        set { _strDefectN32 = value; }

    }

    private string _strDefectN33 = string.Empty;
    public string DefectN33
    {
        get { return _strDefectN33; }
        set { _strDefectN33 = value; }

    }

    private string _strDefectN34 = string.Empty;
    public string DefectN34
    {
        get { return _strDefectN34; }
        set { _strDefectN34 = value; }

    }

    private string _strDefectN35 = string.Empty;
    public string DefectN35
    {
        get { return _strDefectN35; }
        set { _strDefectN35 = value; }

    }

    private string _strDefectN36 = string.Empty;
    public string DefectN36
    {
        get { return _strDefectN36; }
        set { _strDefectN36 = value; }

    }

    private string _strDefectN37 = string.Empty;
    public string DefectN37
    {
        get { return _strDefectN37; }
        set { _strDefectN37 = value; }

    }

    private string _strDefectN38 = string.Empty;
    public string DefectN38
    {
        get { return _strDefectN38; }
        set { _strDefectN38 = value; }

    }

    private string _strDefectN39 = string.Empty;
    public string DefectN39
    {
        get { return _strDefectN39; }
        set { _strDefectN39 = value; }

    }

    private string _strDefectN40 = string.Empty;
    public string DefectN40
    {
        get { return _strDefectN40; }
        set { _strDefectN40 = value; }

    }

    private string _strDefectN41 = string.Empty;
    public string DefectN41
    {
        get { return _strDefectN41; }
        set { _strDefectN41 = value; }

    }

    private string _strDefectN42 = string.Empty;
    public string DefectN42
    {
        get { return _strDefectN42; }
        set { _strDefectN42 = value; }

    }

    private string _strDefectN43 = string.Empty;
    public string DefectN43
    {
        get { return _strDefectN43; }
        set { _strDefectN43 = value; }

    }

    private string _strDefectN44 = string.Empty;
    public string DefectN44
    {
        get { return _strDefectN44; }
        set { _strDefectN44 = value; }

    }

    private string _strDefectN45 = string.Empty;
    public string DefectN45
    {
        get { return _strDefectN45; }
        set { _strDefectN45 = value; }

    }

    private string _strDefectN46 = string.Empty;
    public string DefectN46
    {
        get { return _strDefectN46; }
        set { _strDefectN46 = value; }

    }

    private string _strDefectN47 = string.Empty;
    public string DefectN47
    {
        get { return _strDefectN47; }
        set { _strDefectN47 = value; }

    }

    private string _strDefectN48 = string.Empty;
    public string DefectN48
    {
        get { return _strDefectN48; }
        set { _strDefectN48 = value; }

    }

    private string _strDefectN49 = string.Empty;
    public string DefectN49
    {
        get { return _strDefectN49; }
        set { _strDefectN49 = value; }

    }

    private string _strDefectN50 = string.Empty;
    public string DefectN50
    {
        get { return _strDefectN50; }
        set { _strDefectN50 = value; }

    }

    private string _strDefectN51 = string.Empty;
    public string DefectN51
    {
        get { return _strDefectN51; }
        set { _strDefectN51 = value; }

    }

    private string _strDefectN52 = string.Empty;
    public string DefectN52
    {
        get { return _strDefectN52; }
        set { _strDefectN52 = value; }

    }

    private string _strDefectN53 = string.Empty;
    public string DefectN53
    {
        get { return _strDefectN53; }
        set { _strDefectN53 = value; }

    }

    private string _strDefectN54 = string.Empty;
    public string DefectN54
    {
        get { return _strDefectN54; }
        set { _strDefectN54 = value; }

    }

    private string _strDefectN55 = string.Empty;
    public string DefectN55
    {
        get { return _strDefectN55; }
        set { _strDefectN55 = value; }

    }

    private string _strDefectN56 = string.Empty;
    public string DefectN56
    {
        get { return _strDefectN56; }
        set { _strDefectN56 = value; }

    }

    private string _strDefectN57 = string.Empty;
    public string DefectN57
    {
        get { return _strDefectN57; }
        set { _strDefectN57 = value; }

    }

    private string _strDefectN58 = string.Empty;
    public string DefectN58
    {
        get { return _strDefectN58; }
        set { _strDefectN58 = value; }

    }

    private string _strDefectN59 = string.Empty;
    public string DefectN59
    {
        get { return _strDefectN59; }
        set { _strDefectN59 = value; }

    }

    private string _strDefectN60 = string.Empty;
    public string DefectN60
    {
        get { return _strDefectN60; }
        set { _strDefectN60 = value; }

    }

    #endregion



    public void ReadFile(string strFilePath)
    {
        if (!System.IO.File.Exists(strFilePath))
        {
            return;
        }

        string text = System.IO.File.ReadAllText(strFilePath);
        TrayObj aTray = TrayObj.ToTrayObj(text);

        this.Date = aTray.Date;
        this.TimeStart = aTray.TimeStart;
        this.TimeEnd = aTray.TimeEnd;
        this.UsedTime = aTray.UsedTime;
        this.TrayID = aTray.TrayID;
        this.Pallet1 = aTray.Pallet1;
        this.Pallet2 = aTray.Pallet2;
        this.Pallet3 = aTray.Pallet3;
        this.Pallet4 = aTray.Pallet4;
        this.Pallet5 = aTray.Pallet5;
        this.Pallet6 = aTray.Pallet6;


        this.HGAN1 = aTray.HGAN1;
        this.HGAN2 = aTray.HGAN2;
        this.HGAN3 = aTray.HGAN3;
        this.HGAN4 = aTray.HGAN4;
        this.HGAN5 = aTray.HGAN5;

        this.HGAN6 = aTray.HGAN6;
        this.HGAN7 = aTray.HGAN7;
        this.HGAN8 = aTray.HGAN8;
        this.HGAN9 = aTray.HGAN9;
        this.HGAN10 = aTray.HGAN10;

        this.HGAN11 = aTray.HGAN11;
        this.HGAN12 = aTray.HGAN12;
        this.HGAN13 = aTray.HGAN13;
        this.HGAN14 = aTray.HGAN14;
        this.HGAN15 = aTray.HGAN15;

        this.HGAN16 = aTray.HGAN16;
        this.HGAN17 = aTray.HGAN17;
        this.HGAN18 = aTray.HGAN18;
        this.HGAN19 = aTray.HGAN19;
        this.HGAN20 = aTray.HGAN20;

        this.HGAN21 = aTray.HGAN21;
        this.HGAN22 = aTray.HGAN22;
        this.HGAN23 = aTray.HGAN23;
        this.HGAN24 = aTray.HGAN24;
        this.HGAN25 = aTray.HGAN25;

        this.HGAN26 = aTray.HGAN26;
        this.HGAN27 = aTray.HGAN27;
        this.HGAN28 = aTray.HGAN28;
        this.HGAN29 = aTray.HGAN29;
        this.HGAN30 = aTray.HGAN30;

        this.HGAN31 = aTray.HGAN31;
        this.HGAN32 = aTray.HGAN32;
        this.HGAN33 = aTray.HGAN33;
        this.HGAN34 = aTray.HGAN34;
        this.HGAN35 = aTray.HGAN35;

        this.HGAN36 = aTray.HGAN36;
        this.HGAN37 = aTray.HGAN37;
        this.HGAN38 = aTray.HGAN38;
        this.HGAN39 = aTray.HGAN39;
        this.HGAN40 = aTray.HGAN40;

        this.HGAN41 = aTray.HGAN41;
        this.HGAN42 = aTray.HGAN42;
        this.HGAN43 = aTray.HGAN43;
        this.HGAN44 = aTray.HGAN44;
        this.HGAN45 = aTray.HGAN45;

        this.HGAN46 = aTray.HGAN46;
        this.HGAN47 = aTray.HGAN47;
        this.HGAN48 = aTray.HGAN48;
        this.HGAN49 = aTray.HGAN49;
        this.HGAN50 = aTray.HGAN50;

        this.HGAN51 = aTray.HGAN51;
        this.HGAN52 = aTray.HGAN52;
        this.HGAN53 = aTray.HGAN53;
        this.HGAN54 = aTray.HGAN54;
        this.HGAN55 = aTray.HGAN55;

        this.HGAN56 = aTray.HGAN56;
        this.HGAN57 = aTray.HGAN57;
        this.HGAN58 = aTray.HGAN58;
        this.HGAN59 = aTray.HGAN59;
        this.HGAN60 = aTray.HGAN60;


        this.DefectN1 = aTray.DefectN1;
        this.DefectN2 = aTray.DefectN2;
        this.DefectN3 = aTray.DefectN3;
        this.DefectN4 = aTray.DefectN4;
        this.DefectN5 = aTray.DefectN5;

        this.DefectN6 = aTray.DefectN6;
        this.DefectN7 = aTray.DefectN7;
        this.DefectN8 = aTray.DefectN8;
        this.DefectN9 = aTray.DefectN9;
        this.DefectN10 = aTray.DefectN10;

        this.DefectN11 = aTray.DefectN11;
        this.DefectN12 = aTray.DefectN12;
        this.DefectN13 = aTray.DefectN13;
        this.DefectN14 = aTray.DefectN14;
        this.DefectN15 = aTray.DefectN15;

        this.DefectN16 = aTray.DefectN16;
        this.DefectN17 = aTray.DefectN17;
        this.DefectN18 = aTray.DefectN18;
        this.DefectN19 = aTray.DefectN19;
        this.DefectN20 = aTray.DefectN20;

        this.DefectN21 = aTray.DefectN21;
        this.DefectN22 = aTray.DefectN22;
        this.DefectN23 = aTray.DefectN23;
        this.DefectN24 = aTray.DefectN24;
        this.DefectN25 = aTray.DefectN25;

        this.DefectN26 = aTray.DefectN26;
        this.DefectN27 = aTray.DefectN27;
        this.DefectN28 = aTray.DefectN28;
        this.DefectN29 = aTray.DefectN29;
        this.DefectN30 = aTray.DefectN30;

        this.DefectN31 = aTray.DefectN31;
        this.DefectN32 = aTray.DefectN32;
        this.DefectN33 = aTray.DefectN33;
        this.DefectN34 = aTray.DefectN34;
        this.DefectN35 = aTray.DefectN35;

        this.DefectN36 = aTray.DefectN36;
        this.DefectN37 = aTray.DefectN37;
        this.DefectN38 = aTray.DefectN38;
        this.DefectN39 = aTray.DefectN39;
        this.DefectN40 = aTray.DefectN40;

        this.DefectN41 = aTray.DefectN41;
        this.DefectN42 = aTray.DefectN42;
        this.DefectN43 = aTray.DefectN43;
        this.DefectN44 = aTray.DefectN44;
        this.DefectN45 = aTray.DefectN45;

        this.DefectN46 = aTray.DefectN46;
        this.DefectN47 = aTray.DefectN47;
        this.DefectN48 = aTray.DefectN48;
        this.DefectN49 = aTray.DefectN49;
        this.DefectN50 = aTray.DefectN50;

        this.DefectN51 = aTray.DefectN51;
        this.DefectN52 = aTray.DefectN52;
        this.DefectN53 = aTray.DefectN53;
        this.DefectN54 = aTray.DefectN54;
        this.DefectN55 = aTray.DefectN55;

        this.DefectN56 = aTray.DefectN56;
        this.DefectN57 = aTray.DefectN57;
        this.DefectN58 = aTray.DefectN58;
        this.DefectN59 = aTray.DefectN59;
        this.DefectN60 = aTray.DefectN60;

    }

    public string ToXML()
    {
        StringBuilder sb = new StringBuilder();
        XmlWriterSettings settings = new XmlWriterSettings { Encoding = Encoding.UTF8, Indent = true };

        using (var writer = XmlWriter.Create(sb, settings))
        {
            System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(this.GetType());

            writer.WriteStartDocument();
            x.Serialize(writer, this);
        }

        return sb.ToString();
    }

    public static TrayObj ToTrayObj(string strXML)
    {
        TrayObj aTray;

        System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(typeof(TrayObj));
        using (StringReader reader = new StringReader(strXML))
        {
            aTray = (TrayObj)x.Deserialize(reader);
        }

        return aTray;
    }



}