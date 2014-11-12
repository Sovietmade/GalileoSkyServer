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

}
