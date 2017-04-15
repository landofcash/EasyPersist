using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyPersist.Core.IFaces
{
    /// <summary>
    /// Converts DB value to property
    /// </summary>
    public abstract class DBValueConverterBase
    {
        public abstract object Convert(object dbValue);
    }
}
