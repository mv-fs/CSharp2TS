using CSharp2TS.Core.Attributes;

namespace CSharp2TS.Tests.Stubs.Models {
    [TSInterface]
    public class TestClassWithMethodsDefault {
        public int Value { get; set; }
        public int Compute() => Value;
    }
}
