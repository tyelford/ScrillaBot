using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scrilla.Lib.Attributes
{
    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public class WalletPropertyAttribute : Attribute
    {
        public readonly string propName;
        public WalletPropertyAttribute(string propName)
        {
            this.propName = propName;
        }
    }

    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class WalletSourceAttribute : Attribute
    {
        public readonly Type sourceType;
        public readonly string sourceName;
        public WalletSourceAttribute(Type sourceType, string sourceName)
        {
            this.sourceType = sourceType;
            this.sourceName = sourceName;
        }
    }
}
