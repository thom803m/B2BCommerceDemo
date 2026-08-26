using System.Text.Json;
using System.Text.Json.Serialization;

namespace B2BCommerceDemo.API.Converters
{
    public class DateTimeConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
                return null;

            return DateTime.Parse(reader.GetString()!);
        }

        public override void Write(
            Utf8JsonWriter writer,
            DateTime? value,
            JsonSerializerOptions options)
        {
            if (value.HasValue)
            {
                writer.WriteStringValue(
                    value.Value.ToString("dd-MM-yyyy"));
            }
            else
            {
                writer.WriteNullValue();
            }
        }
    }
}

