using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;


namespace GalileoSkyServer
{
    class Program
    {
        static void Main(string[] args)
        {
            IGalileoSkyServer galileoTcpServer = new GalileoTcpServer();
            galileoTcpServer.StartServer();
            galileoTcpServer.BeginListenForIncomingConnections();

            Console.ReadKey();
        }
    }
}
