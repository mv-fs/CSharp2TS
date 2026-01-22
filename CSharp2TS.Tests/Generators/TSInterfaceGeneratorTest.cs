using CSharp2TS.CLI;
using CSharp2TS.CLI.Generators.Common;
using CSharp2TS.CLI.Generators.TSInterfaces;
using CSharp2TS.CLI.Utility;
using CSharp2TS.Tests.Stubs.Enums;
using CSharp2TS.Tests.Stubs.Models;
using Mono.Cecil;

namespace CSharp2TS.Tests.Generators {
    public class TSInterfaceGeneratorTest : GeneratorTestBase {
        private ModuleDefinition module = null!;
        private TSInterfaceGenerator generator = null!;
        private Dictionary<string, TSFileInfo> files = null!;
        private Options options = null!;

        [SetUp]
        public void Setup() {
            // Load the test assembly to get TypeReferences
            string assemblyPath = typeof(TSInterfaceGeneratorTest).Assembly.Location;
            module = ModuleDefinition.ReadModule(assemblyPath);

            // Setup options
            options = new Options {
                UseNullableStrings = false,
            };

            // Setup files dictionary - needed for import resolution
            files = [];

            // Add test types to files dictionary
            AddType(typeof(TestClass));
            AddType(typeof(TestClass2));
            AddType(typeof(ParentClass));
            AddType(typeof(ChildClass));
            AddType(typeof(TestRecord));
            AddType(typeof(GenericClass1<>));
            AddType(typeof(GenericClass2<,>));
            AddType(typeof(TestClassInFolder));
            AddType(typeof(TestEnum));
            AddType(typeof(TestEnumInFolder));
            AddType(typeof(TestClassWithStub));

            generator = new TSInterfaceGenerator(files, options);
        }

        [TearDown]
        public void TearDown() {
            module?.Dispose();
        }

        [Test]
        public void InterfaceGenerator_Class_AllProperties() {
            var typeRef = module.ImportReference(typeof(TestClass));

            string result = generator.Generate(typeRef.Resolve());

            TestMatchesFile("Expected/TestClass.ts", result);
        }

        [Test]
        public void InterfaceGenerator_ChildClassWithInheritance() {
            var typeRef = module.ImportReference(typeof(ChildClass));

            string result = generator.Generate(typeRef.Resolve());

            TestMatchesFile("Expected/ChildClass.ts", result);
        }

        /// <summary>
        /// This test checks of the generation adds duplicated attributes when a child class overrides properties from a parent class.
        /// </summary>
        [Test]
        public void InterfaceGenerator_ChildClassWithInheritanceOverride() {
            var typeRef = module.ImportReference(typeof(ChildClassOverride));

            string result = generator.Generate(typeRef.Resolve());

            TestMatchesFile("Expected/ChildClassOverride.ts", result);
        }

        [Test]
        public void InterfaceGenerator_Record() {
            var typeRef = module.ImportReference(typeof(TestRecord));

            string result = generator.Generate(typeRef.Resolve());

            TestMatchesFile("Expected/TestRecord.ts", result);
        }

        [Test]
        public void InterfaceGenerator_GenericClassSingleParameter() {
            var typeRef = module.ImportReference(typeof(GenericClass1<>));

            string result = generator.Generate(typeRef.Resolve());

            TestMatchesFile("Expected/GenericClass1.ts", result);
        }

        [Test]
        public void InterfaceGenerator_GenericClassMultipleParameters() {
            var typeRef = module.ImportReference(typeof(GenericClass2<,>));

            string result = generator.Generate(typeRef.Resolve());

            TestMatchesFile("Expected/GenericClass2.ts", result);
        }

        [Test]
        public void InterfaceGenerator_StubGeneration() {
            var typeRef = module.ImportReference(typeof(TestClassWithStub));

            string result = generator.Generate(typeRef.Resolve());

            TestMatchesFile("Expected/TestClassWithStub.ts", result);
        }

        private void AddType(Type type) {
            var typeRef = module.ImportReference(type);

            files[type.FullName!] = NameUtility.GetFileDetails(typeRef.Resolve(), options, "Models");
        }
    }
}