using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalileoSkyServer
{
    public abstract class Package
    {

    }

    public class GalileoSkyTcpPackage : Package
    {
        #region GalileoSkyTcpPackage fields

        byte header;

        byte[] packageLength = new byte[2];

        byte[] controlSum = new byte[2];

        #endregion
    }

}
