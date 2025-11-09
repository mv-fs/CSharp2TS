using CSharp2TS.CLI.Generators.TSEnums;
using CSharp2TS.Tests.Generators;
using CSharp2TS.Tests.Stubs.Enums;
using Mono.Cecil;

namespace CSharp2TS.Tests.Enums {
    public class TSEnumGeneratorTest : GeneratorTestBase {
        private ModuleDefinition module = null!;
        private TSEnumGenerator generator = null!;

        [SetUp]
        public void Setup() {
            // Load the test assembly to get TypeReferences
            string assemblyPath = typeof(TSEnumGeneratorTest).Assembly.Location;
            module = ModuleDefinition.ReadModule(assemblyPath);
            generator = new();
        }

        [TearDown]
        public void TearDown() {
            module?.Dispose();
        }

        [Test]
        public void EnumGenerator_Enum() {
            var typeRef = module.ImportReference(typeof(TestEnum));

            string result = generator.Generate(typeRef.Resolve());

            TestMatchesFile("Expected/TestEnum.ts", result);
        }

        [Test]
        public void EnumGenerator_EnumWithDescriptions() {
            var typeRef = module.ImportReference(typeof(TestEnumDescriptions));

            string result = generator.Generate(typeRef.Resolve());

            TestMatchesFile("Expected/TestEnumDescriptions.ts", result);
        }

        [Test]
        public void EnumGenerator_EnumWithDescriptionsAndItems() {
            var typeRef = module.ImportReference(typeof(TestEnumDescriptionsAndItemArray));

            string result = generator.Generate(typeRef.Resolve());

            TestMatchesFile("Expected/TestEnumDescriptionsAndItemArray.ts", result);
        }
    }
}
