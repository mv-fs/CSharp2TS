// Auto-generated from TestEnumDescriptionsAndItemArray.cs

enum TestEnumDescriptionsAndItemArray {
  Value1 = 1,
  Value2 = 2,
  Value3 = 3,
}

export default TestEnumDescriptionsAndItemArray;

export const TestEnumDescriptionsAndItemArrayDescriptions: Record<number, string> = {
  [TestEnumDescriptionsAndItemArray.Value1]: 'This is the first value',
  [TestEnumDescriptionsAndItemArray.Value2]: 'Value2',
  [TestEnumDescriptionsAndItemArray.Value3]: 'Value3',
};

export const TestEnumDescriptionsAndItemArrayItems = Object.entries(TestEnumDescriptionsAndItemArrayDescriptions).map(
  ([key, value]) => ({
    value: Number(key),
    title: value,
  })
);
