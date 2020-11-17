using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using XMinds.Utils;

namespace XMinds
{
    /// <summary>
    /// The item object. The key from inherited dictionary is a item property name, value is a item property value.
    /// The property names are case-insensitive.
    /// </summary>
    public sealed class Item : Dictionary<string, object>
    {
        /// <summary>
        /// The name of item_id property.
        /// </summary>
        public const string ItemIdPropName = "item_id";

        /// <summary>
        /// The item_id property. null if not specified.
        /// Note that item id cannot be a “null” or “falsy” value, such as empty string or 0.
        /// </summary>
        [JsonIgnore]
        public object ItemId 
        { 
            get
            {
                if (!this.TryGetValue(ItemIdPropName, out object value))
                {
                    return null;
                }

                return value;
            }

            set 
            {
                this[ItemIdPropName] = value;
            } 
        }

        /// <summary>
        /// Initializes a new instance of the User class that is empty, has the default inherited 
        /// dictionary initial capacity, and uses the case-insensitive equality comparer for the key type.
        /// </summary>
        public Item() : base(new StringIgnoreCaseComparer()) { }

        /// <summary>
        /// Initializes a new instance of the User class with properties copied from the 
        /// user objects, and uses the case-insensitive equality comparer for the key type.
        /// </summary>
        /// <param name="item">The item object.</param>
        public Item(Item item) : base(item, new StringIgnoreCaseComparer()) { }

        /// <summary>
        /// Initializes a new instance of the User class with elements copied from the 
        /// IDictionary, and uses the case-insensitive equality comparer for the key type.
        /// </summary>
        /// <param name="properties">The item properties.</param>
        public Item(IDictionary<string, object> properties): base(properties, new StringIgnoreCaseComparer()) { }

        /// <summary>
        /// Initializes a new instance of the User class that is empty, has the specified inherited 
        /// dictionary initial capacity, and uses the case-insensitive equality comparer for the key type.
        /// </summary>        
        /// <param name="capacity">The initial capacity for inherited dictionary.</param>
        public Item(int capacity): base(capacity, new StringIgnoreCaseComparer()) { }
    }
}
