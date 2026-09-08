using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EInvoiceUtils.Models
{
    /*
     * NOTE:
     *  LHDN APIs may return the error object in different formats (see Submit Documents error responses),
     *  so we use a custom JSON converter to handle the different formats cleanly.
     */
    public class StandardErrorConverter: JsonConverter<StandardError>
    {
        public override StandardError Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    return new StandardError() { Message = reader.GetString() };

                default:
                    return JsonSerializer.Deserialize<StandardError>(JsonElement.ParseValue(ref reader));
            }
        }

        public override void Write(Utf8JsonWriter writer, StandardError value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Message);
        }
    }
}
