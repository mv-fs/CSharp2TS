using System.Text;

namespace CSharp2TS.CLI.Utility {
    public static class StringExtensions {
        public static string ToCamelCase(this string value) {
            if (string.IsNullOrWhiteSpace(value)) {
                return value;
            }

            if (value.Length == 1) {
                return value.ToLowerInvariant();
            }

            return char.ToLowerInvariant(value[0]) + value[1..];
        }

        public static string ToPascalCase(this string value) {
            if (string.IsNullOrWhiteSpace(value)) {
                return value;
            }

            if (value.Length == 1) {
                return value.ToUpperInvariant();
            }

            return char.ToUpperInvariant(value[0]) + value[1..];
        }

        public static string ToKebabCase(this string value) {
            if (string.IsNullOrWhiteSpace(value)) {
                return value;
            }

            var builder = new StringBuilder();

            for (int i = 0; i < value.Length; i++) {
                char c = value[i];

                if (char.IsUpper(c)) {
                    if (builder.Length > 0) {
                        builder.Append('-');
                    }

                    builder.Append(char.ToLowerInvariant(c));
                } else {
                    builder.Append(c);
                }
            }

            return builder.ToString();
        }

        public static string ApplyCasing(this string str, CasingStyle caseStyle) {
            if (string.IsNullOrEmpty(str)) {
                return str;
            }

            return caseStyle switch {
                CasingStyle.CamelCase => str.ToCamelCase(),
                CasingStyle.PascalCase => str.ToPascalCase(),
                CasingStyle.KebabCase => str.ToKebabCase(),
                _ => str
            };
        }
    }
}
