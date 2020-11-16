using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds
{
    /// <summary>
    /// The operatoers supported by recommendation filter.
    /// </summary>
    public static class RecoFilterOperator
    {
        public const string Eq = "eq";
        public const string Lt = "lt";
        public const string Le = "lte";
        public const string Gt = "gt";
        public const string Gte = "gte";
        public const string In = "in";
        public const string NotEmpty = "notempty";
        public const string Neq = "neq";
        public const string NotIn = "notin";
    }
}
