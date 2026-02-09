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

        /// <param name="caseStyle">One of the two strings defined in <see cref="Consts"/></param>
        public static string ApplyCasing(this string str, string caseStyle) {
            if (string.IsNullOrEmpty(str)) {
                return str;
            }
            if (string.IsNullOrEmpty(caseStyle)) {
                return str;
            } else if (caseStyle.Equals(Consts.CamelCase, StringComparison.OrdinalIgnoreCase){
                return str.ToCamelCase();
            } else if (caseStyle.Equals(Consts.PascalCase, StringComparison.OrdinalIgnoreCase)) {
                return str.ToPascalCase();
            }
            return str;
        }
    }
}
