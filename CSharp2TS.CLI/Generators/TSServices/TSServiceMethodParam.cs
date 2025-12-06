using CSharp2TS.CLI.Generators.Common;

namespace CSharp2TS.CLI.Generators.TSServices {
    public record TSServiceMethodParam(string Name, TSType Type, bool IsBodyParam, bool IsFormData, bool IsNullable) {
        public override string ToString() {
            string type = Type.ToString();

            if (IsNullable && !type.EndsWith(" | null")) {
                type += " | null";
            }

            return $"{Name}: {type}";
        }
    }
}
