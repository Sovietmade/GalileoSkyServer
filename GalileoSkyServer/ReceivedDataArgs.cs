using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalileoSkyServer
{
    class ReceivedDataArgs
    {
        private byte[] mData;

        public ReceivedDataArgs()
        {

        }

        public ReceivedDataArgs(byte[] inData)
        {
            mData = inData;
        }

        public byte[] Data
        {
            get
            {
                return mData;
            }
            set
            {
                mData = value;
            }
        }
    }
}
