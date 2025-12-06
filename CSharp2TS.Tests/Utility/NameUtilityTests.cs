using CSharp2TS.CLI;
using CSharp2TS.CLI.Utility;
using CSharp2TS.Core.Attributes;
using Mono.Cecil;

namespace CSharp2TS.Tests.Utility {
    [TestFixture]
    public class NameUtilityTests {
        private ModuleDefinition module;

        [SetUp]
        public void Setup() {
            var assembly = AssemblyDefinition.CreateAssembly(
                new AssemblyNameDefinition("TestAssembly", new Version(1, 0, 0, 0)),
                "TestModule",
                ModuleKind.Dll);

            module = assembly.MainModule;
            module.Types.Clear();
        }

        private TypeDefinition AddType(Type type) {
            return module.ImportReference(type).Resolve();
        }

        [TestCase(typeof(object), "Object")]
        // Built in objects with generic params
        [TestCase(typeof(List<int>), "List")]
        [TestCase(typeof(Dictionary<int, int>), "Dictionary")]
        [TestCase(typeof(Tuple<int, int, string>), "Tuple")]
        public void GetName_WithoutTSAttribute_ReturnsTypeName(Type type, string expected) {
            // Arrange
            var typeDef = AddType(type);

            // Act
            var result = NameUtility.GetName(typeDef);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetName_WithTSAttributeButNoCustomName_ReturnsOriginalName() {
            // Arrange
            var typeDef = AddType(typeof(DummyClass));

            // Act
            var result = NameUtility.GetName(typeDef);

            // Assert
            Assert.That(result, Is.EqualTo(nameof(DummyClass)));
        }

        [Test]
        public void GetName_WithTSAttributeAndCustomName_ReturnsCustomName() {
            // Arrange
            var typeDef = AddType(typeof(DummyClass2));

            // Act
            var result = NameUtility.GetName(typeDef);

            // Assert
            Assert.That(result, Is.EqualTo("CustomName"));
        }

        [TestCase(Consts.PascalCase, typeof(DummyClass), "DummyClass")]
        [TestCase(Consts.CamelCase, typeof(DummyClass), "dummyClass")]
        public void GetFileDetails_WithDifferentCasing_ReturnsCorrectFileInfo(string casing, Type type, string expectedFileName) {
            // Arrange
            var typeDef = AddType(type);
            var options = new Options { FileNameCasingStyle = casing };
            var basePath = "/base/path";

            // Act
            var result = NameUtility.GetFileDetails(typeDef, options, basePath);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(type.Name));
            Assert.That(result.Folder, Is.EqualTo(basePath));
            Assert.That(result.FileNameWithoutExtension, Is.EqualTo(expectedFileName));
        }

        [Test]
        public void GetFileDetails_WithCustomFolder_ReturnsCorrectPath() {
            // Arrange
            var typeDef = AddType(typeof(DummyClass3));
            var options = new Options { FileNameCasingStyle = Consts.PascalCase };
            var basePath = "/base/path";

            // Act
            var result = NameUtility.GetFileDetails(typeDef, options, basePath);

            // Assert
            Assert.That(result.Folder, Is.EqualTo($"{basePath}/custom/folder"));
            Assert.That(result.TypeName, Is.EqualTo(nameof(DummyClass3)));
        }

        [Test]
        public void GetFileDetails_WithoutCustomFolder_UsesBasePath() {
            // Arrange
            var typeDef = AddType(typeof(DummyClass));
            var options = new Options { FileNameCasingStyle = Consts.PascalCase };
            var basePath = "/base/path";

            // Act
            var result = NameUtility.GetFileDetails(typeDef, options, basePath);

            // Assert
            Assert.That(result.Folder, Is.EqualTo(basePath));
            Assert.That(result.TypeName, Is.EqualTo(nameof(DummyClass)));
            Assert.That(result.FileNameWithoutExtension, Is.EqualTo(nameof(DummyClass)));
        }

        [Test]
        public void GetFileDetails_WithCustomNameAndFolder_ReturnsCorrectInfo() {
            // Arrange
            var typeDef = AddType(typeof(DummyClass4));
            var options = new Options { FileNameCasingStyle = Consts.PascalCase };
            var basePath = "/output";

            // Act
            var result = NameUtility.GetFileDetails(typeDef, options, basePath);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo("CustomName"));
            Assert.That(result.Folder, Is.EqualTo($"{basePath}/custom/folder"));
            Assert.That(result.FileNameWithoutExtension, Is.EqualTo("CustomName"));
        }

        [Test]
        public void GetCustomFolderLocation_WithoutFolder() {
            // Arrange
            var typeDef = AddType(typeof(DummyClass));

            // Act
            var result = NameUtility.GetCustomFolderLocation(typeDef);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetCustomFolderLocation_WithFolder() {
            // Arrange
            var typeDef = AddType(typeof(DummyClass3));

            // Act
            var result = NameUtility.GetCustomFolderLocation(typeDef);

            // Assert
            Assert.That(result, Is.EqualTo("custom/folder"));
        }

        [TSInterface]
        private class DummyClass { }

        [TSInterface("CustomName")]
        private class DummyClass2 { }

        [TSInterface(Folder = "custom/folder")]
        private class DummyClass3 { }

        [TSInterface("CustomName", Folder = "custom/folder")]
        private class DummyClass4 { }
    }
}
