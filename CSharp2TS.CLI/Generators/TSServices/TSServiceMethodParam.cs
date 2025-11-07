using CSharp2TS.CLI.Generators.Common;

namespace CSharp2TS.CLI.Generators.TSServices {
    public record TSServiceMethodParam(string Name, TSType Type, bool IsBodyParam, bool IsFormData);
}
