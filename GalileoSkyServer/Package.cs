using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
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


        protected UInt16 Modbus(byte[] buf, int len)
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

        public virtual object GetGalileoSkyData(Type inType)
        {
           var q =  from m in mGalileoSkyData where m.TypeOfData == inType select m.Data;
           return q.FirstOrDefault();
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


        public byte[] Length
        {
            get
            {
                return packageLength;
            }
            set
            {
                packageLength = value;
            }
        }

        public override byte[] ToByteArray()
        {
            List<byte> asByteList = new List<byte>();
            asByteList.Add(header);
            if (Length == null || Length.Length != 2)
            {
                asByteList.AddRange(new byte[] { 0x0, 0x0 }); // Length
            }
            else 
            {

                asByteList.AddRange(Length);
                
            }

            foreach (var m in mGalileoSkyData)
            {
                asByteList.Add(m.Tag);
                asByteList.AddRange(m.ToByteArray());
            }

            if (Length == null || Length.Length != 2)
            {

                UInt16 packetLength = (UInt16)(asByteList.Count - 3);
                byte[] lengthAsByteArray = BitConverter.GetBytes(packetLength);
                
                Length = lengthAsByteArray;

                asByteList[1] = lengthAsByteArray[0];
                asByteList[2] = lengthAsByteArray[1];
            }

            ControlSum = BitConverter.GetBytes(Modbus(asByteList.ToArray(), asByteList.Count));

            asByteList.AddRange(ControlSum); //ControlSum

            return asByteList.ToArray();
        }

        #region GalileoSkyTcpPackageData fields


        byte[] packageLength;


        #endregion
    }
}
