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

        [TestCase(nameof(Object), "Object")]
        // Built in objects with generic params
        [TestCase(nameof(List<int>), "List")]
        [TestCase(nameof(Dictionary<int, int>), "Dictionary")]
        [TestCase(nameof(Tuple<int, int, string>), "Tuple")]
        public void GetName_WithoutTSAttribute_ReturnsTypeName(string typeName, string expected) {
            // Arrange
            var typeDef = CreateTypeDefinition(typeName);

            // Act
            var result = NameUtility.GetName(typeDef);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetName_WithTSAttributeAndCustomName_ReturnsCustomName() {
            // Arrange
            var typeDef = CreateTypeDefinition("OriginalName");
            AddTSAttribute(typeDef, "CustomTypeName");

            // Act
            var result = NameUtility.GetName(typeDef);

            // Assert
            Assert.That(result, Is.EqualTo("CustomTypeName"));
        }

        [Test]
        public void GetName_WithTSAttributeButNoCustomName_ReturnsOriginalName() {
            // Arrange
            var typeDef = CreateTypeDefinition("OriginalName`1");
            AddTSAttribute(typeDef, null);

            // Act
            var result = NameUtility.GetName(typeDef);

            // Assert
            Assert.That(result, Is.EqualTo("OriginalName"));
        }

        [TestCase(Consts.PascalCase, "MyType", "MyType")]
        [TestCase(Consts.CamelCase, "MyType", "myType")]
        public void GetFileDetails_WithDifferentCasing_ReturnsCorrectFileInfo(string casing, string typeName, string expectedFileName) {
            // Arrange
            var typeDef = CreateTypeDefinition(typeName);
            var options = new Options { FileNameCasingStyle = casing };
            var basePath = "/base/path";

            // Act
            var result = NameUtility.GetFileDetails(typeDef, options, basePath);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(typeName));
            Assert.That(result.Folder, Is.EqualTo(basePath));
            Assert.That(result.FileNameWithoutExtension, Is.EqualTo(expectedFileName));
        }

        [Test]
        public void GetFileDetails_WithCustomFolder_ReturnsCorrectPath() {
            // Arrange
            var typeDef = CreateTypeDefinition("TestType");
            AddTSAttributeWithFolder(typeDef, null, "custom/folder");
            var options = new Options { FileNameCasingStyle = Consts.PascalCase };
            var basePath = "/base/path";

            // Act
            var result = NameUtility.GetFileDetails(typeDef, options, basePath);

            // Assert
            Assert.That(result.Folder, Is.EqualTo($"{basePath}/custom/folder"));
            Assert.That(result.TypeName, Is.EqualTo("TestType"));
        }

        [Test]
        public void GetFileDetails_WithoutCustomFolder_UsesBasePath() {
            // Arrange
            var typeDef = CreateTypeDefinition("TestType");
            var options = new Options { FileNameCasingStyle = Consts.PascalCase };
            var basePath = "/base/path";

            // Act
            var result = NameUtility.GetFileDetails(typeDef, options, basePath);

            // Assert
            Assert.That(result.Folder, Is.EqualTo(basePath));
            Assert.That(result.TypeName, Is.EqualTo("TestType"));
            Assert.That(result.FileNameWithoutExtension, Is.EqualTo("TestType"));
        }

        [Test]
        public void GetFileDetails_WithCustomNameAndFolder_ReturnsCorrectInfo() {
            // Arrange
            var typeDef = CreateTypeDefinition("OriginalType");
            AddTSAttributeWithFolder(typeDef, "CustomType", "models");
            var options = new Options { FileNameCasingStyle = Consts.PascalCase };
            var basePath = "/output";

            // Act
            var result = NameUtility.GetFileDetails(typeDef, options, basePath);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo("CustomType"));
            Assert.That(result.Folder, Is.EqualTo($"{basePath}/models"));
            Assert.That(result.FileNameWithoutExtension, Is.EqualTo("CustomType"));
        }

        [TestCase("", "/base", "/base")]
        [TestCase("subfolder", "/base", "/base/subfolder")]
        [TestCase("nested/deep/folder", "/base", "/base/nested/deep/folder")]
        public void GetFileDetails_WithVariousFolderPaths_CombinesCorrectly(string customFolder, string basePath, string expectedPath) {
            // Arrange
            var typeDef = CreateTypeDefinition("TestType");
            if (!string.IsNullOrEmpty(customFolder)) {
                AddTSAttributeWithFolder(typeDef, null, customFolder);
            }

            var options = new Options { FileNameCasingStyle = "pascal" };

            // Act
            var result = NameUtility.GetFileDetails(typeDef, options, basePath);

            // Assert
            Assert.That(result.Folder, Is.EqualTo(expectedPath));
        }

        private TypeDefinition CreateTypeDefinition(string name) {
            var typeDef = new TypeDefinition("TestNamespace", name, TypeAttributes.Public | TypeAttributes.Class);
            module.Types.Add(typeDef);
            return typeDef;
        }

        private void AddTSAttribute(TypeDefinition typeDef, string? customName) {
            var attrType = CreateTSAttributeType();
            var constructor = new MethodDefinition(".ctor", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, module.TypeSystem.Void);

            if (customName != null) {
                constructor.Parameters.Add(new ParameterDefinition(module.TypeSystem.String));
            }

            attrType.Methods.Add(constructor);

            var attribute = new CustomAttribute(constructor);
            if (customName != null) {
                attribute.ConstructorArguments.Add(new CustomAttributeArgument(module.TypeSystem.String, customName));
            }

            typeDef.CustomAttributes.Add(attribute);
        }

        private void AddTSAttributeWithFolder(TypeDefinition typeDef, string? customName, string folder) {
            var attrType = CreateTSAttributeType();
            var constructor = new MethodDefinition(".ctor", MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, module.TypeSystem.Void);

            if (customName != null) {
                constructor.Parameters.Add(new ParameterDefinition(module.TypeSystem.String));
            }

            attrType.Methods.Add(constructor);

            var folderProperty = new PropertyDefinition("Folder", PropertyAttributes.None, module.TypeSystem.String);
            attrType.Properties.Add(folderProperty);

            var attribute = new CustomAttribute(constructor);
            if (customName != null) {
                attribute.ConstructorArguments.Add(new CustomAttributeArgument(module.TypeSystem.String, customName));
            }

            attribute.Properties.Add(new CustomAttributeNamedArgument("Folder", new CustomAttributeArgument(module.TypeSystem.String, folder)));

            typeDef.CustomAttributes.Add(attribute);
        }

        private TypeDefinition CreateTSAttributeType() {
            var baseAttrType = new TypeDefinition(typeof(TSAttributeBase).Namespace, nameof(TSAttributeBase), TypeAttributes.Public | TypeAttributes.Abstract | TypeAttributes.Class);
            module.Types.Add(baseAttrType);

            var attrType = new TypeDefinition("CSharp2TS.Core.Attributes", "TSAttribute", TypeAttributes.Public | TypeAttributes.Class, baseAttrType);
            module.Types.Add(attrType);

            return attrType;
        }
    }
}
