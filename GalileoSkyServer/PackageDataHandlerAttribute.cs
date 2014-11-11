using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalileoSkyServer
{
    [AttributeUsage(AttributeTargets.Method,AllowMultiple = true)]
    class PackageDataHandlerAttribute : Attribute
    {
        private Int32 tag;

        public PackageDataHandlerAttribute(Int32 tag)
        {
            this.tag = tag;
        }

        public Int32 Tag
        {
            get { return tag; }
        }
    }
}
