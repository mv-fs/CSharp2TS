using Mono.Cecil;

namespace CSharp2TS.CLI.Generators.Entities {
    public class TSProperty {
        public required TypeReference TypeRef { get; init; }
        public required RawTSType TSType { get; init; }
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
                RawTSType.String => "string",
                RawTSType.Number => "number",
                RawTSType.Boolean => "boolean",
                RawTSType.File => "File",
                RawTSType.FormData => "FormData",
                RawTSType.Void => "void",
                RawTSType.Object => ObjectName ?? "Object",
                RawTSType.Unknown => "unknown",
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
