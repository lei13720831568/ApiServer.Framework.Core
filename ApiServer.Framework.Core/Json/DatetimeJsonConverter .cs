using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ApiServer.Framework.Core.Json
{
    public class DatetimeJsonConverter : JsonConverter<DateTime>
    {
        public DatetimeJsonConverter(string format) {
            _format = format;
        }

        private string _format;

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => DateTime.Parse(reader.GetString());

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            => writer.WriteStringValue(value.ToString(_format));
    }

}
