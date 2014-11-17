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
            galileoTcpClient.DataReceived += ProccessMessageData;
            galileoTcpClient.CloseConnectionHandlers += CloseClientConnection;

            lock (mListOfConnectedClientsLock)
            {
                mListOfConnectedClients.Add(galileoTcpClient);
            }

            // Process the connection here. (Add the client to a 
            // server table, read data, etc.)
            Console.WriteLine("Client connected completed");
            mListener.BeginAcceptTcpClient(DoAcceptTcpClientCallback, mListener);
        }

        public void SendCommand(UInt16 inTerminalID, String inCommand)
        {
            var q = from m in mListOfConnectedClients where m.TerminalId.TerminalIDData == inTerminalID select m;
            GalileoTcpClient gtc = q.FirstOrDefault();
            if (gtc != null)
            {
                GalileoSkyTcpPackageData command = new GalileoSkyTcpPackageData();
                command.AddGalileoSkyData(new GalileoSkyData(typeof(ImeiData),gtc.IMEI,0x03));
                command.AddGalileoSkyData(new GalileoSkyData(typeof(TerminalID), gtc.TerminalId, 0x04));

                CommandID cid = new CommandID();
                cid.CommandIDDData = mCommandId;
                mCommandId++;

                command.AddGalileoSkyData(new GalileoSkyData(typeof(CommandID), cid, 0xE0));

                Command c = new Command();
                c.CommandLength = (byte)inCommand.Length;
                c.CommandData = inCommand;

                command.AddGalileoSkyData(new GalileoSkyData(typeof(Command), c, 0xE1));
                gtc.SendMessage(command.ToByteArray());
            }
        }

        void ProccessMessageData(object sender, ReceivedDataArgs ea)
        { 
            //TEST
            //SendCommand(50, "MainPack 111111000");
        }

        void CloseClientConnection(object sender, EventArgs ea)
        {
            var galileoTcpClient = sender as GalileoTcpClient;
            if (galileoTcpClient != null)
            {
                galileoTcpClient.CloseConnection();
                lock (mListOfConnectedClientsLock)
                {
                    mListOfConnectedClients.Remove(galileoTcpClient);
                }
                Console.WriteLine("Connection with client has been closed.");
            }
            else
            {

            }
        }

        #region TcpServerFields

        UInt32 mCommandId;

        TcpListener mListener;

        object mListOfConnectedClientsLock;

        List<GalileoTcpClient> mListOfConnectedClients;

        #endregion

    }
}
