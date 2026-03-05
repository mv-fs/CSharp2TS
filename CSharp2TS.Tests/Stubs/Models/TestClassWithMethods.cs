using CSharp2TS.Core.Attributes;

namespace CSharp2TS.Tests.Stubs.Models {
    [TSInterface(IncludeMethods = true, GenerateClass = true)]
    public class TestClassWithMethods {
        public int Value { get; set; }
        public TestClass2 Build([TSNullable] TestClass2 input) => input;
        public void Reset() { }

        [TSExclude]
        public void Hidden() { }

        [TSExclude]
        public string ExcludedProperty { get; set; } = string.Empty;
    }
}
