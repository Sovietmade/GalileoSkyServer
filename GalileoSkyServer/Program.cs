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

            //TcpClient tcpClient = new TcpClient();
            //tcpClient.Connect("electronics.integra-s.com", 5555);
            //tcpClient.Connect("192.168.38.121", 5555);

            //tcpClient.Connect("eproxy.volga", 8080);
            //GalileoTcpClient gTcpClient = new GalileoTcpClient(tcpClient, 2048);
           // gTcpClient.SendMessage("CONNECT electronics.integra-s.com:5555  HTTP/1.1\r\nHost: electronics.integra-s.com:5555\r\n\r\n");

            Console.ReadKey();
        }
    }
}
