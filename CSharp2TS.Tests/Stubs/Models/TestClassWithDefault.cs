using CSharp2TS.Core.Attributes;

namespace CSharp2TS.Tests.Stubs.Models {
    [TSInterface(GenerateClass = true)]
    public class TestClassWithDefault {
        [TSDefault("018b1a05-8f0f-477d-8d95-c7effcde2eeb")]
        public Guid GuidProperty { get; set; }
        public Guid RegularGuidProperty { get; set; }
    }
}
