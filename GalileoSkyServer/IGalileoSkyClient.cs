using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalileoSkyServer
{
    interface IGalileoSkyClient
    {
        void SendMessage(string inMessage);

        event EventHandler<ReceivedDataArgs> DataReceived;

    }
}
