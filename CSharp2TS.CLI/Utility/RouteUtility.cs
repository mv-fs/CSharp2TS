using System.Text.RegularExpressions;

namespace CSharp2TS.CLI.Utility {
    public static class RouteUtility {
        public static string? GetCleanRouteConstraints(string? template) {
            if (string.IsNullOrWhiteSpace(template)) {
                return null;
            }

            // Replace double curly braces to avoid interference with regex parsing
            string parsedTemplate = template
                .Replace("{{", "__DOUBLE_CURLY_BRACE__")
                .Replace("}}", "__DOUBLE_CURLY_BRACE__");

            // Matches "{paramName:someConstraint}", capturing "paramName" in a group called "param"
            const string ConstraintPattern = @"\{(?<param>\w+)(?:[\:\=][^}]+)?\}";

            return Regex.Replace(
                parsedTemplate,
                ConstraintPattern,
                match => {
                    return "${" + match.Groups["param"].Value + "}";
                }
            );
        }
    }
}
