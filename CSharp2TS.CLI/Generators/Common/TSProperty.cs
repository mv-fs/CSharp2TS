using Mono.Cecil;

namespace CSharp2TS.CLI.Generators.Entities {
    public class TSProperty {
        public required TypeReference TypeRef { get; init; }
        public required TSType TSType { get; init; }
        public required bool IsObject { get; init; }
        public required string? ObjectName { get; init; }
        public required bool IsTypeNullable { get; init; }
        public required bool IsPropertyNullable { get; init; }
        public required bool IsDictionary { get; init; }
        public required bool IsCollection { get; init; }
        public required int JaggedCount { get; init; }
        public required IList<TSProperty> GenericArguments { get; init; }

        public bool IsNullable => IsTypeNullable || IsPropertyNullable;

        public string GetTypeName() {
            return TSType switch {
                TSType.String => "string",
                TSType.Number => "number",
                TSType.Boolean => "boolean",
                TSType.File => "File",
                TSType.FormData => "FormData",
                TSType.Void => "void",
                TSType.Object => ObjectName ?? "Object",
                TSType.Unknown => "unknown",
                _ => throw new NotSupportedException($"Type '{TSType}' is not supported.")
            };
        }

        public override string ToString() {
            string tsType = GetTypeName();

            if (IsTypeNullable) {
                tsType += " | null";

                if (IsCollection || IsDictionary) {
                    tsType = $"({tsType})";
                }
            }

            if (IsCollection) {
                for (int i = 0; i < JaggedCount; i++) {
                    tsType += "[]";
                }
            }

            if (IsDictionary) {
                tsType = $"{{ [key: string]: {tsType} }}";
            }

            if (TypeRef.IsGenericInstance) {
                tsType += "<" + string.Join(", ", GenericArguments.Select(i => i.ToString())) + ">";
            }

            if (IsPropertyNullable && !tsType.EndsWith(" | null")) {
                tsType += " | null";
            }

            return tsType;
        }
    }
}
