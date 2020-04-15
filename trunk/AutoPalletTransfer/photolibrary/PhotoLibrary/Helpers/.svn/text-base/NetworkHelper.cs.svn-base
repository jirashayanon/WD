using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace PhotoLibrary.Helpers
{
    public class NetworkHelper
    {
        public string SendPacket(string message, string ip = "127.0.0.1", int port = 7001)
        {
            message = "Hello, there!" + "$";
            try
            {
                TcpClient clientSocket = new TcpClient();
                clientSocket.Connect(ip, port);

                NetworkStream serverStream = clientSocket.GetStream();
                byte[] outStream = System.Text.Encoding.ASCII.GetBytes(message);
                serverStream.Write(outStream, 0, outStream.Length);
                serverStream.Flush();

                byte[] inStream = new byte[10025];
                serverStream.Read(inStream, 0, (int)clientSocket.ReceiveBufferSize);
                string returndata = System.Text.Encoding.ASCII.GetString(inStream).Trim().Replace("\0", string.Empty);

                return returndata;
            }
            catch (Exception ex)
            {
                LogHelper.AppendErrorFile(ex.ToString(), true);
            }
            return "";
        }
    }
}
