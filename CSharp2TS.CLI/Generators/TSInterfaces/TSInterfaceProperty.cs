using CSharp2TS.CLI.Generators.Common;

namespace CSharp2TS.CLI.Generators.TSInterfaces {
    public record TSInterfaceProperty(string Name, TSType Type, bool IsNullableProperty) {
        public string GetDefaultValue() {
            if (IsNullableProperty) {
                return "null";
            }

            return Type.GetDefaultValue();
        }

        public override string ToString() {
            string type = Type.ToString();

            if (IsNullableProperty && !type.EndsWith("| null")) {
                type += " | null";
            }

            return $"{Name}: {type}";
        }
    }
}
