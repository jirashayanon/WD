using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCRModule
{
    class ToolModel
    {

    }


    public class toolController : WDConnect.Application.WDConnectBase
    {
        public override void Initialize(string equipmentModelPath)
        {
            base.Initialize(equipmentModelPath);
            this.ConnectionStatus = ConnectionStatus.NotConnecting;
        }

        public void SendMessage(WDConnect.Common.SCITransaction transaction)
        {
            base.ProcessOutStream(transaction);
        }

        public void ReplyOutStream(WDConnect.Common.SCITransaction transaction)
        {
            base.ReplyOutSteam(transaction);
        }

        public string ToolId
        {
            get
            {
                return EquipmentModel.Nameable.id;
            }
        }

        public string connectionMode
        {
            get
            {
                return EquipmentModel.GemConnection.HSMS.connectionMode.ToString();
            }
        }

        public string remoteIPAddress
        {
            get
            {
                return EquipmentModel.GemConnection.HSMS.remoteIPAddress;
            }
        }

        public string remotePortNumber
        {
            get
            {
               return EquipmentModel.GemConnection.HSMS.remotePortNumber.ToString();
            }
        }




        public int T3Timeout
        {
            get
            {
                return EquipmentModel.GemConnection.HSMS.T3Timeout;
            }
        }

        public int T5Timeout
        {
            get
            {
                return EquipmentModel.GemConnection.HSMS.T5Timeout;
            }
        }

      
        private ConnectionStatus _connectionStatus;
        public ConnectionStatus ConnectionStatus
        {
            get
            {
                return _connectionStatus;
            }
            set
            {
                _connectionStatus = value;
            }
        }
    }

    public enum ConnectionStatus
    {
        Connecting,
        NotConnecting
    }
}
