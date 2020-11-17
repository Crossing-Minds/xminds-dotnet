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
        /// <summary>
        /// Equal to value.
        /// </summary>
        public const string Eq = "eq";

        /// <summary>
        /// Strictly less than value.
        /// </summary>
        public const string Lt = "lt";

        /// <summary>
        /// Less or equal to value.
        /// </summary>
        public const string Le = "lte";

        /// <summary>
        /// Strictly greater than value.
        /// </summary>
        public const string Gt = "gt";

        /// <summary>
        /// Greater or equal to value.
        /// </summary>
        public const string Gte = "gte";

        /// <summary>
        /// Equal to one of the values.
        /// </summary>
        public const string In = "in";

        /// <summary>
        /// Not equal to value (or null).
        /// </summary>
        public const string Neq = "neq";

        /// <summary>
        /// Not equal to any of the values (or null).
        /// </summary>
        public const string NotIn = "notin";

        /// <summary>
        /// Not null.
        /// </summary>
        public const string NotEmpty = "notempty";
    }
}
