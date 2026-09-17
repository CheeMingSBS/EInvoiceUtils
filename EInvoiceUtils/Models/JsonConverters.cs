using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

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

    /*
     *  NOTE:
     *   Helps to convert a list of the specified elements into duplicate keys in the JSON document as
     *   required by LHDN. E.g.:
     *   
     *   "Element": [ { "a": 1 }, { "a": 2 } ]
     *      
     *      becomes
     *   
     *   "Element": [ { "a": 1 } ],
     *   "Element": [ { "a": 2 } ]
     *   
     *  TODO: Handle deserialization from duplicate keys into list of the specified elements
     */
    public class TaxSubtotalListConverter: JsonConverter<List<TaxSubtotal>>
    {
        public override List<TaxSubtotal> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<List<TaxSubtotal>>(JsonElement.ParseValue(ref reader));
        }

        public override void Write(Utf8JsonWriter writer, List<TaxSubtotal> value, JsonSerializerOptions options)
        {
            for (int i = 0; i < value.Count; i++)
            {
                if (i != 0)
                    writer.WritePropertyName("TaxSubtotal");
                writer.WriteStartArray();
                JsonSerializer.Serialize(writer, value[i], options);
                writer.WriteEndArray();
            }
        }
    }

    /*
     *  NOTE:
     *   Helps to convert a list of the specified elements into duplicate keys in the JSON document as
     *   required by LHDN. E.g.:
     *   
     *   "Element": [ { "a": 1 }, { "a": 2 } ]
     *      
     *      becomes
     *   
     *   "Element": [ { "a": 1 } ],
     *   "Element": [ { "a": 2 } ]
     *   
     *  TODO: Handle deserialization from duplicate keys into list of the specified elements
     */
    public class AllowanceChargeListConverter: JsonConverter<List<AllowanceCharge>>
    {
        public override List<AllowanceCharge> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return JsonSerializer.Deserialize<List<AllowanceCharge>>(JsonElement.ParseValue(ref reader));
        }

        public override void Write(Utf8JsonWriter writer, List<AllowanceCharge> value, JsonSerializerOptions options)
        {
            for (int i = 0; i < value.Count; i++)
            {
                if (i != 0)
                    writer.WritePropertyName("AllowanceCharge");
                writer.WriteStartArray();
                JsonSerializer.Serialize(writer, value[i], options);
                writer.WriteEndArray();
            }
        }
    }
}
