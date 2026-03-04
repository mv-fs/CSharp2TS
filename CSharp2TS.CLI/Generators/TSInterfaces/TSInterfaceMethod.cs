using CSharp2TS.CLI.Generators.Common;

namespace CSharp2TS.CLI.Generators.TSInterfaces {
    public record TSInterfaceMethod(string Name, TSType ReturnType, IList<TSInterfaceMethodParameter> Parameters, IList<string> GenericParameters, string SignatureKey, bool IsNullableReturnType) {
        public string GenericString => GenericParameters.Count > 0 ? $"<{string.Join(", ", GenericParameters)}>" : string.Empty;

        public override string ToString() {
            string returnType = ReturnType.ToString();

            if (IsNullableReturnType && !returnType.EndsWith("| null")) {
                returnType += " | null";
            }

            return $"{Name}{GenericString}({string.Join(", ", Parameters)}): {returnType}";
        }
    }
}
