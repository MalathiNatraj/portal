using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Diebold.RemoteService.Proxies.EMC
{
    public class TcpManager
    {
        public string Host { get; set; }
        public int Port { get; set; }
        
        public TcpManager(string host, int port)
        {
            Host = host;
            Port = port;
        }

        /// <summary>
        /// Sends the message to the Host using a TcpClient
        /// </summary>
        /// <param name="message"></param>
        /// <returns>True if the response is correct</returns>
        public bool Send(string message)
        {
            var tcpClient = new TcpClient(Host, Port); ;
            tcpClient.ReceiveTimeout = 10000; // Milliseconds
            // Get a client stream for reading and writing.
            var networkStream = tcpClient.GetStream();

            try
            {
                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = Encoding.ASCII.GetBytes(message);

                // Send the message to the connected TcpServer. 
                networkStream.Write(data, 0, data.Length);

                // Buffer to store the response bytes.
                data = new Byte[256];

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = networkStream.Read(data, 0, data.Length);
                var responseData = Encoding.ASCII.GetString(data, 0, bytes);

                return (responseData == "ack");
            }
            catch (ArgumentNullException e)
            {
                return false;
            }
            //catch (SocketException e)
            //{
            //    return false;
            //}
            finally
            {
                networkStream.Close();
                tcpClient.Close();
            }
        }


        /// <summary>
        /// Sends an async message to the Host using a TcpClient
        /// </summary>
        /// <param name="message"></param>
        /// <returns>True if the response is correct</returns>
        public void SendAsync(string message)
        {
            var tcpClient = new TcpClient(Host, Port);

            // Translate the passed message into ASCII and store it as a Byte array.
            Byte[] data = Encoding.ASCII.GetBytes(message);

            // Get a client stream for reading and writing.
            var networkStream = tcpClient.GetStream();

            // Send the async message to the connected TcpServer. 
            networkStream.BeginWrite(data, 0, data.Length, WriteDataToEmcCallback, tcpClient);
        }

        private void WriteDataToEmcCallback(IAsyncResult result)
        {
            var tcpClient = (TcpClient)result.AsyncState;
            var networkStream = tcpClient.GetStream();

            try
            {
                networkStream.EndWrite(result);

                // Buffer to store the response bytes.
                Byte[] data = new Byte[256];

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = networkStream.Read(data, 0, data.Length);
                var responseData = Encoding.ASCII.GetString(data, 0, bytes);

                //(responseData == "ack")
            }
            catch (ArgumentNullException e)
            {
            }
            catch (SocketException e)
            {
            }
            finally
            {
                networkStream.Close();
                tcpClient.Close();
            }
        }

    }
}
