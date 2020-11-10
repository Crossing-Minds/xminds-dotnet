using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using XMinds.Utils;

namespace XMinds
{
    /// <summary>
    /// The user object. The key from inherited dictionary is a user property name, value is a user property value.
    /// The property names are case-insensitive.
    /// </summary>
    public sealed class User : Dictionary<string, object>
    {
        /// <summary>
        /// The name of user_id property.
        /// </summary>
        public const string UserIdPropName = "user_id";

        /// <summary>
        /// The user_id property. null if not specified.
        /// Note that user id cannot be a “null” or “falsy” value, such as empty string or 0.
        /// </summary>
        [JsonIgnore]
        public object UserId 
        { 
            get
            {
                object value = null;
                if (!this.TryGetValue(UserIdPropName, out value))
                {
                    return null;
                }

                return value;
            }

            set 
            {
                this[UserIdPropName] = value;
            } 
        }

        /// <summary>
        /// Initializes a new instance of the User class that is empty, has the default inherited 
        /// dictionary initial capacity, and uses the case-insensitive equality comparer for the key type.
        /// </summary>
        public User() : base(new StringIgnoreCaseComparer()) { }

        /// <summary>
        /// Initializes a new instance of the User class with properties copied from the 
        /// user objects, and uses the case-insensitive equality comparer for the key type.
        /// </summary>
        /// <param name="user">The user object.</param>
        public User(User user) : base(user, new StringIgnoreCaseComparer()) { }

        /// <summary>
        /// Initializes a new instance of the User class with elements copied from the 
        /// IDictionary, and uses the case-insensitive equality comparer for the key type.
        /// </summary>
        /// <param name="properties">The user properties.</param>
        public User(IDictionary<string, object> properties): base(properties, new StringIgnoreCaseComparer()) { }

        /// <summary>
        /// Initializes a new instance of the User class that is empty, has the specified inherited 
        /// dictionary initial capacity, and uses the case-insensitive equality comparer for the key type.
        /// </summary>        
        /// <param name="capacity">The initial capacity for inherited dictionary.</param>
        public User(int capacity): base(capacity, new StringIgnoreCaseComparer()) { }
    }
}
