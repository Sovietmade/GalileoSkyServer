using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalileoSkyServer
{
    public abstract class Package
    {
        public abstract byte[] ToByteArray();

    }

    public abstract class GalileoSkyTcpPackage : Package
    {
        #region Package fields

        protected byte header;

        protected byte[] controlSum = new byte[2];

        #endregion
    }

    public class GalileoSkyTcpPackageResponse : GalileoSkyTcpPackage
    {

        public GalileoSkyTcpPackageResponse()
        {

        }

        public override byte[] ToByteArray()
        {
            byte[] PackageAsByteArray = new byte[1 + controlSum.Length];
            PackageAsByteArray[0] = header;
            Array.Copy(controlSum, 0, PackageAsByteArray, 1, controlSum.Length);
            return PackageAsByteArray;
        }

    }

    public class GalileoSkyTcpPackageData : GalileoSkyTcpPackage
    {

        public GalileoSkyTcpPackageData()
        {

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
