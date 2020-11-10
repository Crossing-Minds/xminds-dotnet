using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace XMinds.Utils
{
    class UntypedJsonConverter : JsonConverter
    {
        public override bool CanRead => true;

        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType)
        {
            return typeof(object).Equals(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);

            if (token.Type == JTokenType.Array)
            {
                // We would like to return .NET arrays instead of JArray when deserializing to object.
                return token.ToObject<object[]>();
            }
            else if (token is JValue jValue)
            {
                return jValue.Value;
            }
            else if (token.Type == JTokenType.Object)
            {
                return token;
            }

            throw new JsonSerializationException("Unexpected token type");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

    }
}
