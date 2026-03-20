using CSharp2TS.CLI.Generators.Common;

namespace CSharp2TS.CLI.Generators.TSInterfaces {
    public record TSInterfaceMethodParameter(string Name, TSType Type, bool IsNullableParameter) {
        public override string ToString() {
            string type = Type.ToString();

            if (IsNullableParameter && !type.EndsWith("| null")) {
                type += " | null";
            }

            return $"{Name}: {type}";
        }
    }
}
