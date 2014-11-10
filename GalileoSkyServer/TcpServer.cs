using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

namespace GalileoSkyServer
{
    class TcpServer
    {
        TcpListener mListener;

        public TcpServer()
        {
            mListener = new TcpListener(IPAddress.Any, 5555);
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

            // Process the connection here. (Add the client to a 
            // server table, read data, etc.)
            Console.WriteLine("Client connected completed");
        }

    }
}
