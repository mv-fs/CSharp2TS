using System.Text.Json;
using System.Text.Json.Serialization;

namespace CSharp2TS.CLI {
    public class CasingStyleJsonConverter : JsonConverter<CasingStyle> {
        public override CasingStyle Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            string? value = reader.GetString();

            return value?.ToLowerInvariant() switch {
                "camel" => CasingStyle.CamelCase,
                "pascal" => CasingStyle.PascalCase,
                "kebab" => CasingStyle.KebabCase,
                _ => throw new JsonException($"Invalid casing style '{value}'. Valid options: 'camel', 'pascal', 'kebab'"),
            };
        }

        public override void Write(Utf8JsonWriter writer, CasingStyle value, JsonSerializerOptions options) {
            string str = value switch {
                CasingStyle.CamelCase => "camel",
                CasingStyle.PascalCase => "pascal",
                CasingStyle.KebabCase => "kebab",
                _ => throw new JsonException($"Unknown casing style '{value}'"),
            };

            writer.WriteStringValue(str);
        }
    }
}
