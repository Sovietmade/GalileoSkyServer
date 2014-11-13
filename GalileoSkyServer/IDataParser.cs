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
                    Console.WriteLine("-----------New Packet------------");
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
                        while (((byte[])paramsArr[0]) != null && ((byte[])paramsArr[0]).Length > 0)
                        {
                            byte[] tempArr = (byte[])paramsArr[0];

                            if (mDataHandlersMap.ContainsKey(tempArr[0]))
                            {

                                GalileoSkyData gsd = (GalileoSkyData)mDataHandlersMap[tempArr[0]].Invoke(this, paramsArr);
                                galileoSkyTcpPackageData.AddGalileoSkyData(gsd);
                            }
                            else {
                                paramsArr[0] = null;
                            }
                        }
                        Console.WriteLine("-----------End------------");
                        Console.WriteLine("");
                        
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

            imei.IMEI = Encoding.ASCII.GetString(inData, 1, 15);

            gsd.Data = imei;
            gsd.TypeOfData = typeof(ImeiData);

            CutArray(ref inData, 16, inData.Length - 1);

            Console.WriteLine("IMEI: {0}", imei.IMEI);
            return gsd;
        }

        [PackageDataHandler(0x04)]
        GalileoSkyData TerminalID(ref byte[] inData)
        {
            GalileoSkyData gsd = new GalileoSkyData();

            gsd.Tag = 0x04;

            TerminalID terminalId = new TerminalID();
            terminalId.TerminalIDData = BitConverter.ToUInt16(inData, 1);

            gsd.Data = terminalId;
            gsd.TypeOfData = typeof(TerminalID);

            CutArray(ref inData, 3, inData.Length - 1);

            Console.WriteLine("Terminal ID: {0}", terminalId.TerminalIDData);
            return gsd;
        }

        [PackageDataHandler(0x10)]
        GalileoSkyData PackageNumber(ref byte[] inData)
        {
            GalileoSkyData gsd = new GalileoSkyData();

            gsd.Tag = 0x10;

            PackageNumer packageNumber = new PackageNumer();
            packageNumber.PackageNumerData = BitConverter.ToUInt16(inData, 1);

            gsd.Data = packageNumber;
            gsd.TypeOfData = typeof(PackageNumer);

            CutArray(ref inData, 3, inData.Length - 1);

            Console.WriteLine("Package number: {0}", packageNumber.PackageNumerData);
            return gsd;
        }

        [PackageDataHandler(0x20)]
        GalileoSkyData DateAndTime(ref byte[] inData)
        {
            GalileoSkyData gsd = new GalileoSkyData();

            gsd.Tag = 0x20;

            DateAndTime dateAndTime = new DateAndTime();
            dateAndTime.DateAndTimeData = BitConverter.ToUInt32(inData, 1);

            gsd.Data = dateAndTime;
            gsd.TypeOfData = typeof(DateAndTime);

            CutArray(ref inData, 5, inData.Length - 1);

            Console.WriteLine("Date is: {0}", dateAndTime.ActualDateAndTime.ToString());
            return gsd;
        }

        [PackageDataHandler(0x30)]
        GalileoSkyData CoordinatesAndSatellites(ref byte[] inData)
        {
            GalileoSkyData gsd = new GalileoSkyData();

            gsd.Tag = 0x30;

            CoordinatesAndSatellites cordinatesAndSatellites = new CoordinatesAndSatellites();
            byte satellitesAndCorrectness = inData[1];
            byte satellites = (byte)(satellitesAndCorrectness & 0x0F);
            byte correctness = (byte)(satellitesAndCorrectness & 0xF0);

            cordinatesAndSatellites.SatellitesAmount = satellites;
            cordinatesAndSatellites.Correctness = correctness;

            cordinatesAndSatellites.Latitude =  BitConverter.ToInt32(inData, 2);
            cordinatesAndSatellites.Longitude = BitConverter.ToInt32(inData, 6);

            gsd.Data = cordinatesAndSatellites;
            gsd.TypeOfData = typeof(CoordinatesAndSatellites);

            CutArray(ref inData, 10, inData.Length - 1);

            Console.WriteLine("{0}", cordinatesAndSatellites.ToString());
            return gsd;
        }

        [PackageDataHandler(0x33)]
        GalileoSkyData SpeedAndDirection(ref byte[] inData)
        {
            GalileoSkyData gsd = new GalileoSkyData();

            gsd.Tag = 0x33;

            SpeedAndDirection speedAndDirection = new SpeedAndDirection();
            speedAndDirection.Speed = (UInt16)(BitConverter.ToUInt16(inData, 1) / (UInt16)10);
            speedAndDirection.Direction = (UInt16)(BitConverter.ToUInt16(inData, 3) / (UInt16)10);

            gsd.Data = speedAndDirection;
            gsd.TypeOfData = typeof(SpeedAndDirection);

            CutArray(ref inData, 5, inData.Length - 1);

            Console.WriteLine("{0}", speedAndDirection.ToString());
            return gsd;
        }

        [PackageDataHandler(0x34)]
        GalileoSkyData Height(ref byte[] inData)
        {
            GalileoSkyData gsd = new GalileoSkyData();

            gsd.Tag = 0x34;

            Height height = new Height();
            height.HeightData = BitConverter.ToInt16(inData, 1);

            gsd.Data = height;
            gsd.TypeOfData = typeof(Height);

            CutArray(ref inData, 3, inData.Length - 1);

            Console.WriteLine("{0}", height.ToString());
            return gsd;
        }

        [PackageDataHandler(0x35)]
        GalileoSkyData HDOP(ref byte[] inData)
        {
            GalileoSkyData gsd = new GalileoSkyData();

            gsd.Tag = 0x35;

            HDOP hdop = new HDOP();
            hdop.HDOPData = inData[1];

            gsd.Data = hdop;
            gsd.TypeOfData = typeof(HDOP);

            CutArray(ref inData, 2, inData.Length - 1);

            Console.WriteLine("{0}", hdop.ToString());
            return gsd;
        }

        [PackageDataHandler(0x40)]
        GalileoSkyData DeviceStatus(ref byte[] inData)
        {
            GalileoSkyData gsd = new GalileoSkyData();

            gsd.Tag = 0x40;

            byte[] deviceStatusByteArray = new byte[2];
            deviceStatusByteArray[0] = inData[1];
            deviceStatusByteArray[1] = inData[2];

            DeviceStatus deviceStatus = new DeviceStatus(deviceStatusByteArray);


            gsd.Data = deviceStatus;
            gsd.TypeOfData = typeof(DeviceStatus);

            CutArray(ref inData, 3, inData.Length - 1);

            Console.WriteLine("{0}", deviceStatus.ToString());
            return gsd;
        }

        #region GalileoSkyTcpPackageParser fields

        Dictionary<Int32, MethodInfo> mDataHandlersMap = new Dictionary<int, MethodInfo>();

        MemoryStream mMessagesBuffer = new MemoryStream();

        #endregion

    }

}
