using CSharp2TS.CLI;
using CSharp2TS.CLI.Generators.Common;
using CSharp2TS.CLI.Generators.TSServices;
using CSharp2TS.CLI.Utility;
using CSharp2TS.Tests.Stubs.Controllers;
using CSharp2TS.Tests.Stubs.Enums;
using CSharp2TS.Tests.Stubs.Models;
using Mono.Cecil;

namespace CSharp2TS.Tests.Generators {
    public class TSAxiosServiceGeneratorTest : GeneratorTestBase {
        private ModuleDefinition module = null!;
        private TSAxiosServiceGenerator generator = null!;
        private Dictionary<string, TSFileInfo> files = null!;
        private Options options = null!;

        [SetUp]
        public void Setup() {
            // Load the test assembly to get TypeReferences
            string assemblyPath = typeof(TSAxiosServiceGeneratorTest).Assembly.Location;
            module = ModuleDefinition.ReadModule(assemblyPath);

            // Setup options
            options = new Options {
                UseNullableStrings = false,
                ServicesOutputFolder = "Services",
            };

            // Setup files dictionary - needed for import resolution
            files = [];

            // Add test types to files dictionary
            AddModelType(typeof(TestClass));
            AddModelType(typeof(TestClass2));
            AddModelType(typeof(ParentClass));
            AddModelType(typeof(ChildClass));
            AddModelType(typeof(TestRecord));
            AddModelType(typeof(GenericClass1<>));
            AddModelType(typeof(GenericClass2<,>));
            AddModelType(typeof(TestClassInFolder));
            AddModelType(typeof(TestEnum));
            AddModelType(typeof(TestEnumInFolder));

            // Add controller types to files dictionary
            AddServiceType(typeof(ActionResult_TestController));
            AddServiceType(typeof(IActionResult_TestController));
            AddServiceType(typeof(AsyncActionResult_TestController));
            AddServiceType(typeof(AsyncIActionResult_TestController));
            AddServiceType(typeof(CustomRouteController));
            AddServiceType(typeof(NoRouteController));
            AddServiceType(typeof(TemplatedRouteController));
            AddServiceType(typeof(FileController));
            AddServiceType(typeof(FormController));

            generator = new TSAxiosServiceGenerator(options, files);
        }

        [TearDown]
        public void TearDown() {
            module?.Dispose();
        }

        [TestCase(typeof(IActionResult_TestController))]
        [TestCase(typeof(ActionResult_TestController))]
        [TestCase(typeof(AsyncIActionResult_TestController))]
        [TestCase(typeof(AsyncActionResult_TestController))]
        public void ServiceGenerator_AsyncIActionResult_TestController(Type controllerType) {
            var typeRef = module.ImportReference(controllerType);

            string result = generator.Generate(typeRef.Resolve());

            TestMatchesFile("Expected/TestService.ts", result);
        }

        [Test]
        public void ServiceGenerator_CustomRoute() {
            var typeRef = module.ImportReference(typeof(CustomRouteController));

            string result = generator.Generate(typeRef.Resolve());

            TestMatchesFile("Expected/CustomRouteService.ts", result);
        }

        [Test]
        public void ServiceGenerator_NoRoute() {
            var typeRef = module.ImportReference(typeof(NoRouteController));

            string result = generator.Generate(typeRef.Resolve());

            TestMatchesFile("Expected/NoRouteService.ts", result);
        }

        [Test]
        public void ServiceGenerator_TemplatedRoute() {
            var typeRef = module.ImportReference(typeof(TemplatedRouteController));

            string result = generator.Generate(typeRef.Resolve());

            TestMatchesFile("Expected/TemplatedRouteService.ts", result);
        }

        [Test]
        public void ServiceGenerator_FileController() {
            var typeRef = module.ImportReference(typeof(FileController));

            string result = generator.Generate(typeRef.Resolve());

            TestMatchesFile("Expected/FileService.ts", result);
        }

        [Test]
        public void ServiceGenerator_FormController() {
            var typeRef = module.ImportReference(typeof(FormController));

            string result = generator.Generate(typeRef.Resolve());

            TestMatchesFile("Expected/FormService.ts", result);
        }

        [Test]
        public void GetFileInfo_StripsControllerAndAddsService() {
            var type = typeof(IActionResult_TestController);

            Assert.That(files[type.FullName!].TypeName, Is.EqualTo("IActionResult_TestService"));
            Assert.That(files[type.FullName!].Folder, Is.EqualTo(options.ServicesOutputFolder));
            Assert.That(files[type.FullName!].FileNameWithoutExtension, Is.EqualTo("IActionResult_TestService"));
        }

        private void AddModelType(Type type) {
            var typeRef = module.ImportReference(type);

            files[type.FullName!] = NameUtility.GetFileDetails(typeRef.Resolve(), options, "Models");
        }

        private void AddServiceType(Type type) {
            var typeRef = module.ImportReference(type);

            files[type.FullName!] = TSAxiosServiceGenerator.GetFileInfo(typeRef.Resolve(), options);
        }
    }
}