using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalileoSkyServer
{
    interface IGalileoSkyServer
    {
        void StartServer();
        void BeginListenForIncomingConnections();
        void SendCommand(UInt16 inTerminalID, String inCommand);
    }
}
