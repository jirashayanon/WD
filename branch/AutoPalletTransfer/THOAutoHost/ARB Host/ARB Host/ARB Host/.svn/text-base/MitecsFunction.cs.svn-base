using log4net;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using WDHelpers.Mitecs3Helper;

namespace ARB_Host
{
    public static class MitecsFunction
    {
        public static Mitecs3Data M3;
        public static readonly ILog MitecsMethodLog = LogManager.GetLogger("MitecsMethod");

        public static DataSet GetLotDataWithLotID(string Operations, string LotID, out string errorMessage)
        {
            string[] _Operations = Operations.Split(',');
            errorMessage = string.Empty;
            DataSet dsLotDetails = new DataSet();
            foreach (string operation in _Operations)
            {
                dsLotDetails = M3.GetLotDetails(operation, LotID, LotFilterType.SliderSerialNumber , out errorMessage);

                if (string.IsNullOrEmpty(errorMessage))
                {
                    return dsLotDetails;
                }
            }

            return dsLotDetails;
        }

        public static DataSet GetSliderSerialNumberByGroup(string Operations, string LotID, out string errorMessage)
        {
            string[] _Operations = Operations.Split(',');
            errorMessage = string.Empty;
            DataSet dsLotDetails = new DataSet();
            foreach (string operation in _Operations)
            {
                dsLotDetails = M3.GetSliderSerialNumbersByGroup(operation, LotID, SliderStatusFilterType.All, out errorMessage);

                if (string.IsNullOrEmpty(errorMessage))
                {
                    return dsLotDetails;
                }
            }

            return dsLotDetails;
        }

        public static DataSet GetBarGroupInfo(string Operation, string LotID, out string errorMessage)
        {
            MitecsMethodLog.Debug("GetBarGroupInfo : " + Operation + " , LotID " + LotID);
            errorMessage = string.Empty;
            DataSet lotDetails = new DataSet();
            lotDetails = M3.GetBarGroupInfo(Operation, LotID, WaferFilterType.GroupAll, out errorMessage); //[Piroon - Mar 09, 2015] AllBarStatus
            MitecsMethodLog.Debug("errorMessage : " + errorMessage);
           
            return lotDetails;
        }

        public static DataSet GetBarGroupInfoByGroupAll(string Operation, string LotID, out string errorMessage)
        {
            MitecsMethodLog.Debug("GetBarGroupInfo : " + Operation + " , LotID " + LotID);
            errorMessage = string.Empty;
            DataSet lotDetails = new DataSet();
            lotDetails = M3.GetBarGroupInfo(Operation, LotID, WaferFilterType.GroupAllWithTerminated, out errorMessage); //[Piroon - Mar 09, 2015] AllBarStatus
            MitecsMethodLog.Debug("errorMessage : " + errorMessage);
           
            return lotDetails;
        }

    }
}
