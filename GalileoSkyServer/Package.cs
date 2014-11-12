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
