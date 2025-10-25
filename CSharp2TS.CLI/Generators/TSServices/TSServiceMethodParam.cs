using CSharp2TS.CLI.Generators.Entities;

namespace CSharp2TS.CLI.Generators.TSServices {
    public record TSServiceMethodParam(string Name, TSProperty Property, bool IsBodyParam, bool IsFormData);
}
