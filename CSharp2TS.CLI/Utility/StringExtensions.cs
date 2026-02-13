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

        public static string ApplyCasing(this string str, CasingStyle caseStyle) {
            if (string.IsNullOrEmpty(str)) {
                return str;
            }

            return caseStyle switch {
                CasingStyle.CamelCase => str.ToCamelCase(),
                CasingStyle.PascalCase => str.ToPascalCase(),
                _ => str
            };
        }
    }
}
