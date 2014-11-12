using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace GalileoSkyServer
{

    class GalileoTcpServer : IGalileoSkyServer
    {

        public GalileoTcpServer()
        {
            mListener = new TcpListener(IPAddress.Any, 5555);

            mListOfConnectedClients = new List<GalileoTcpClient>();
            mListOfConnectedClientsLock = new object();
        }

        public void StartServer()
        {
            mListener.Start();
        }

        public void BeginListenForIncomingConnections()
        {
            mListener.BeginAcceptTcpClient(DoAcceptTcpClientCallback, mListener);
        }

        public void DoAcceptTcpClientCallback(IAsyncResult ar)
        {
            // Get the listener that handles the client request.
            TcpListener listener = (TcpListener)ar.AsyncState;

            // End the operation and display the received data on  
            // the console.
            TcpClient client = listener.EndAcceptTcpClient(ar);
            //now create a TcpConnection wrapper around TcpClient,add it to connections list.

            GalileoTcpClient galileoTcpClient = new GalileoTcpClient(client, 2048);
            IDataParser dataParser = new GalileoSkyTcpPackageParser();
            galileoTcpClient.DataParser = dataParser;

            lock (mListOfConnectedClientsLock)
            {
                mListOfConnectedClients.Add(galileoTcpClient);
            }

            // Process the connection here. (Add the client to a 
            // server table, read data, etc.)
            Console.WriteLine("Client connected completed");
            mListener.BeginAcceptTcpClient(DoAcceptTcpClientCallback, mListener);
        }

        #region TcpServerFields

        TcpListener mListener;

        object mListOfConnectedClientsLock;

        List<GalileoTcpClient> mListOfConnectedClients;

        #endregion

    }
}
