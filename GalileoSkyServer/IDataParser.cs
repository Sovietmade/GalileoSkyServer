using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

namespace GalileoSkyServer
{
    interface IDataParser
    {
        Package AddDataAndParse(byte[] inData);
         
    }

    public class GalileoSkyTcpPackageParser : IDataParser
    {

        public GalileoSkyTcpPackageParser()
        {
            var Methods = from mi in this.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.InvokeMethod)
                          from attr in mi.GetCustomAttributes(typeof(PackageDataHandlerAttribute), true).Cast<PackageDataHandlerAttribute>()
                          select new { Method = mi, Tag = attr.Tag };
            foreach (var m in Methods)
            {
                mDataHandlersMap[m.Tag] = m.Method;
            }
        }

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

            //There are have to be a header and packet length fields to process packet further

            if (tempTotalData.Length >= 3)
            { 
                //1.Get the package length field value
                //2.If tempTotalData.Length == package length + 5 (1 byte for header,2 for packet length field, 2 for control sum)
                //  2.1 then parse the buffer.
                //3.else there is nothing to do
                if (tempTotalData[0] == 0x01)
                {
                    
                }

                else if (tempTotalData[0] == 0x02)
                {

                }

                else
                {
                    throw new InvalidDataException("Header of the packet is maleformed");
                }
               
            }

            for (int i = 0; i < tempTotalData.Length - 1; i++)
            {

            }

            // What is left from msgStart til the end of data is only a partial message.
            // We want to save that for when the rest of the message arrives.
            mMessagesBuffer.Write(tempTotalData, msgStart, tempTotalData.Length - msgStart);

            return null;
        }


        [PackageDataHandler(0x01)]
        UInt32 HardwareVersionDataHandler(byte[] inData)
        {
            throw new NotImplementedException();
        }

        [PackageDataHandler(0x02)]
        UInt32 SoftwareVersionDataHandler(byte[] inData)
        {
            throw new NotImplementedException();
        }

        [PackageDataHandler(0x03)]
        String ImeiDataHandler(byte[] inData)
        {
            throw new NotImplementedException();
        }

        #region GalileoSkyTcpPackageParser fields

        Dictionary<Int32, MethodInfo> mDataHandlersMap = new Dictionary<int, MethodInfo>();

        MemoryStream mMessagesBuffer = new MemoryStream();

        #endregion

    }

}
