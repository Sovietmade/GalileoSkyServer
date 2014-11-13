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
        GalileoSkyTcpPackage AddDataAndParse(byte[] inData);
         
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

        public GalileoSkyTcpPackage AddDataAndParse(byte[] inData)
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

            GalileoSkyTcpPackageData galileoSkyTcpPackageData = null;

            if (tempTotalData.Length >= 3)
            { 
                //1.Get the package length field value
                //2.If tempTotalData.Length == package length + 5 (1 byte for header,2 for packet length field, 2 for control sum)
                //  2.1 then parse the buffer.
                //3.else there is nothing to do
                if (tempTotalData[0] == 0x01)
                {
                    Console.WriteLine("Header: {0}", tempTotalData[0]);

                    byte[] bytearrayPackageLength = new byte[2];
                    bytearrayPackageLength[0] = tempTotalData[1];
                    bytearrayPackageLength[1] = tempTotalData[2];

                    byte mask = (byte)(1 << 7);
                    bytearrayPackageLength[1] &= (byte)(bytearrayPackageLength[1] & ~mask);

                    UInt16 uint16PackageLength = BitConverter.ToUInt16(bytearrayPackageLength, 0);
                    if (tempTotalData.Length >= (uint16PackageLength + 5) )
                    {
                        galileoSkyTcpPackageData = new GalileoSkyTcpPackageData();
                        

                        byte[] ControlSum = new byte[2];
                        ControlSum[0] = tempTotalData[uint16PackageLength + 3];
                        ControlSum[1] = tempTotalData[uint16PackageLength + 3 + 1];

                        galileoSkyTcpPackageData.ControlSum = ControlSum;
                        

                        byte[] toParse = new byte[uint16PackageLength];
                        Array.Copy(tempTotalData, 3, toParse, 0, uint16PackageLength);

                        byte[] trimmedTempTotalData = new byte[tempTotalData.Length - (uint16PackageLength + 5)];
                        Array.Copy(tempTotalData, uint16PackageLength + 5 - 1, trimmedTempTotalData, 0, tempTotalData.Length - (uint16PackageLength+5));

                        tempTotalData = trimmedTempTotalData;

                        object[] paramsArr = new object[] { toParse };
                        while (toParse != null)
                        {
                            byte[] tempArr = (byte[])paramsArr[0];

                            if (mDataHandlersMap.ContainsKey(tempArr[0]))
                            {

                                GalileoSkyData gsd = (GalileoSkyData)mDataHandlersMap[tempArr[0]].Invoke(this, paramsArr);
                                galileoSkyTcpPackageData.AddGalileoSkyData(gsd);
                            }
                            else {
                                toParse = null;
                            }
                        }
                        
                    }
                    else 
                    {
                        
                    }
                }

                else if (tempTotalData[0] == 0x02)
                {
                    
                }

                else
                {
                    throw new InvalidDataException("Header of the packet is maleformed");
                }
               
            }

            // What is left from msgStart til the end of data is only a partial message.
            // We want to save that for when the rest of the message arrives.
            mMessagesBuffer.Write(tempTotalData, msgStart, tempTotalData.Length - msgStart);

            return galileoSkyTcpPackageData;
        }

        void CutArray<T>(ref T[] inData,int inStartIndex, int inEndindex)
        {
            int length = inEndindex - inStartIndex + 1;
            T[] t = new T[length];

            Array.Copy(inData,inStartIndex,t,0,length);
            inData = t;
        }

        [PackageDataHandler(0x01)]
        GalileoSkyData HardwareVersionDataHandler(ref byte[] inData)
        {
            GalileoSkyData gsd = new GalileoSkyData();

            gsd.Tag = 0x01;

            HardwareVersion hw = new HardwareVersion();
            hw.HW = inData[1];

            gsd.Data = hw;
            gsd.TypeOfData = typeof(HardwareVersion);

            CutArray(ref inData, 2, inData.Length - 1);


            Console.WriteLine("Hardware Version: {0}", hw.HW);
            return gsd;
        }

        [PackageDataHandler(0x02)]
        GalileoSkyData SoftwareVersionDataHandler(ref byte[] inData)
        {
            GalileoSkyData gsd = new GalileoSkyData();

            gsd.Tag = 0x02;

            SoftwareVersion sw = new SoftwareVersion();
            sw.SW = inData[1];

            gsd.Data = sw;
            gsd.TypeOfData = typeof(SoftwareVersion);

            CutArray(ref inData, 2, inData.Length - 1);

            Console.WriteLine("Software Version: {0}", sw.SW);
            return gsd;
        }

        [PackageDataHandler(0x03)]
        GalileoSkyData ImeiDataHandler(ref byte[] inData)
        {
            GalileoSkyData gsd = new GalileoSkyData();

            gsd.Tag = 0x03;

            ImeiData imei = new ImeiData();
            imei.IMEI = BitConverter.ToString(inData, 1, 15);

            gsd.Data = imei;
            gsd.TypeOfData = typeof(ImeiData);

            CutArray(ref inData, 16, inData.Length - 1);

            Console.WriteLine("IMEI: {0}", imei.IMEI);
            return gsd;
        }

        #region GalileoSkyTcpPackageParser fields

        Dictionary<Int32, MethodInfo> mDataHandlersMap = new Dictionary<int, MethodInfo>();

        MemoryStream mMessagesBuffer = new MemoryStream();

        #endregion

    }

}
