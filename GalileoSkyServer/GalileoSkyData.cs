using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace GalileoSkyServer
{
    public class GalileoSkyData
    {
        public Int32 Tag { get; set; }

        public object Data{ get; set; }

        public Type TypeOfData{ get; set; }
    }



    public class HardwareVersion
    {
        public HardwareVersion()
        { 
        
        }

        public HardwareVersion(UInt32 inHW)
        {
            HW = inHW;
        }

        public UInt32 HW { get; set; }

    }

    public class SoftwareVersion
    {
        public SoftwareVersion()
        {

        }

        public SoftwareVersion(UInt32 inSW)
        {
            SW = inSW;
        }

        public UInt32 SW { get; set; }
    }

    public class ImeiData
    {
        public ImeiData()
        {

        }

        public ImeiData(String inIMEI)
        {
            IMEI = inIMEI;
        }
        public String IMEI { get; set; }
    }

    public class TerminalID
    {
        public TerminalID()
        { 
        
        }

        public TerminalID(UInt32 inTerminalID)
        {
            TerminalIDData = inTerminalID;
        }

        public UInt32 TerminalIDData { get; set; }
    }

    public class PackageNumer
    {
        public PackageNumer()
        {

        }

        public PackageNumer(UInt32 inPackageNumer)
        {
            PackageNumerData = inPackageNumer;
        }

        public UInt32 PackageNumerData { get; set; }
    }

    public class DateAndTime
    {
        public DateAndTime()
        {

        }

        public DateAndTime(UInt32 inDateAndTime)
        {
            DateAndTimeData = inDateAndTime;
        }

        public UInt32 DateAndTimeData { get; set; }
        public DateTime ActualDateAndTime
        {
            get {
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                return epoch.AddSeconds(DateAndTimeData);
            }
        }
    }

}
