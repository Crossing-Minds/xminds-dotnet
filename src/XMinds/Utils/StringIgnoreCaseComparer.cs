using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds.Utils
{
    class StringIgnoreCaseComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return x.Equals(y, StringComparison.OrdinalIgnoreCase);
        }

        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }
}
