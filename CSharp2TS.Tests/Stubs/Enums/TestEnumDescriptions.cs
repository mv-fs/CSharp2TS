using CSharp2TS.Core.Attributes;
using DescriptionAttribute = System.ComponentModel.DescriptionAttribute;

namespace CSharp2TS.Tests.Stubs.Enums {
    [TSEnum(GenerateDescriptions = true)]
    public enum TestEnumDescriptions {
        [Description("This is the first value")]
        Value1 = 1,
        Value2 = 2,
        Value3 = 3,
    }

    [TSEnum(GenerateItemsArray = true)]
    public enum TestEnumDescriptionsAndItemArray {
        [Description("This is the first value")]
        Value1 = 1,
        Value2 = 2,
        Value3 = 3,
    }
}
