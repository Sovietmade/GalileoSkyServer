using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GalileoSkyServer
{
    public abstract class Package
    {
        public abstract byte[] ToByteArray();

    }

    public abstract class GalileoSkyTcpPackage : Package
    {
        public byte[] ControlSum
        {
            get {
                return controlSum;
            }
            set {
                controlSum = value;
            }
        }

        public void AddGalileoSkyData(GalileoSkyData inData)
        {
            mGalileoSkyData.Add(inData);
        }

        UInt16 Modbus(byte[] buf, int len)
        {
            UInt16 crc = 0xFFFF;

            for (int pos = 0; pos < len; pos++)
            {
                crc ^= (UInt16)buf[pos];          // XOR byte into least sig. byte of crc

                for (int i = 8; i != 0; i--)
                {    // Loop over each bit
                    if ((crc & 0x0001) != 0)
                    {      // If the LSB is set
                        crc >>= 1;                    // Shift right and XOR 0xA001
                        crc ^= 0xA001;
                    }
                    else                            // Else LSB is not set
                        crc >>= 1;                    // Just shift right
                }
            }
            // Note, this number has low and high bytes swapped, so use it accordingly (or swap bytes)
            return crc;
        }

        #region Package fields

        protected byte header;

        protected byte[] controlSum;

        protected List<GalileoSkyData> mGalileoSkyData = new List<GalileoSkyData>();

        #endregion
    }

    public class GalileoSkyTcpPackageResponse : GalileoSkyTcpPackage
    {

        public GalileoSkyTcpPackageResponse()
        {
            header = 0x02;
        }

        public override byte[] ToByteArray()
        {
            if (ControlSum != null)
            {
                byte[] PackageAsByteArray = new byte[1 + controlSum.Length];
                PackageAsByteArray[0] = header;
                Array.Copy(ControlSum, 0, PackageAsByteArray, 1, ControlSum.Length);
                return PackageAsByteArray;
            }
            else {
                throw new InvalidDataException("Control summ cannot be null");
            }
        }
    }

    public class GalileoSkyTcpPackageData : GalileoSkyTcpPackage
    {

        public GalileoSkyTcpPackageData()
        {
            header = 0x01;
        }

        public override byte[] ToByteArray()
        {
            throw new NotImplementedException();
        }

        #region GalileoSkyTcpPackageData fields


        byte[] packageLength = new byte[2];


        #endregion
    }
}
