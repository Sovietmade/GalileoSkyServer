using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GalileoSkyServer
{
    interface IDataParser
    {
        Package AddDataAndParse(byte[] inData);
         
    }

    public class GalileoSkyTcpPackageParser : IDataParser
    {
        public Package AddDataAndParse(byte[] inData)
        {
            //Collecting all data from mMessageBuffer and inData in the single buffer - tempTotalData.
            //Collected data will be parsed for distinct commands.
            //Collecting data...
            int MessagesBufferLength = (int)mMessagesBuffer.Length;
            byte[] tempTotalData = new byte[MessagesBufferLength + inData.Length];
            mMessagesBuffer.ToArray().CopyTo(tempTotalData, 0);
            Array.Copy(inData, 0, tempTotalData, MessagesBufferLength, inData.Length);

            mMessagesBuffer.SetLength(0);
            //Collecting finishes here.
            //Parsing data...
            int msgStart = 0;

            for (int i = 0; i < tempTotalData.Length - 1; i++)
            {

            }

            // What is left from msgStart til the end of data is only a partial message.
            // We want to save that for when the rest of the message arrives.
            mMessagesBuffer.Write(tempTotalData, msgStart, tempTotalData.Length - msgStart);

            return null;
        }



        MemoryStream mMessagesBuffer = new MemoryStream();

    }

}
