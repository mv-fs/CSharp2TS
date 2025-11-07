using CSharp2TS.CLI.Utility;
using Microsoft.AspNetCore.Mvc;
using Mono.Cecil;

namespace CSharp2TS.Tests.Utility {
    public class AttributeExtensionTests {
        private ModuleDefinition module;
        private TypeDefinition typeDef;

        [SetUp]
        public void Setup() {
            var assembly = AssemblyDefinition.CreateAssembly(
                new AssemblyNameDefinition("TestAssembly", new Version(1, 0, 0, 0)),
                "TestModule",
                ModuleKind.Dll);

            module = assembly.MainModule;
            typeDef = new TypeDefinition("Test", "TestClass", TypeAttributes.Public, module.TypeSystem.Object);

            module.Types.Clear();
            module.Types.Add(typeDef);
        }

        [Test]
        public void HasAttribute_True() {
            // Arrange
            var attribute = new CustomAttribute(module.ImportReference(typeof(ObsoleteAttribute).GetConstructors().First()));
            typeDef.CustomAttributes.Add(attribute);

            // Act
            var result = typeDef.HasAttribute(typeof(ObsoleteAttribute));

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void HasAttribute_False() {
            // Arrange & Act
            var result = typeDef.HasAttribute(typeof(ObsoleteAttribute));

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void HasAttribute_Generic_True() {
            // Arrange
            var attribute = new CustomAttribute(module.ImportReference(typeof(ObsoleteAttribute).GetConstructors().First()));
            typeDef.CustomAttributes.Add(attribute);

            // Act
            var result = typeDef.HasAttribute<ObsoleteAttribute>();

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void HasAttribute_Generic_False() {
            // Arrange & Act
            var result = typeDef.HasAttribute<ObsoleteAttribute>();

            // Assert
            Assert.That(result, Is.False);
        }



        [Test]
        public void TryGetAttribute_WithMatchingAttribute_ReturnsTrueAndAttribute() {
            // Arrange
            var attribute = new CustomAttribute(module.ImportReference(typeof(ObsoleteAttribute).GetConstructors().First()));
            typeDef.CustomAttributes.Add(attribute);

            // Act
            var result = typeDef.TryGetAttribute<ObsoleteAttribute>(out var foundAttribute);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(foundAttribute, Is.Not.Null);
            Assert.That(foundAttribute.AttributeType.FullName, Is.EqualTo(typeof(ObsoleteAttribute).FullName));
        }

        [Test]
        public void TryGetAttribute_WithoutMatchingAttribute_ReturnsFalseAndNull() {
            // Arrange & Act
            var result = typeDef.TryGetAttribute<ObsoleteAttribute>(out var foundAttribute);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(foundAttribute, Is.Null);
        }

        [Test]
        public void TryGetByBaseAttribute_WithMatchingBaseType_ReturnsTrueAndAttribute() {
            // Arrange
            var baseType = new TypeReference("System", "Attribute", module, module.TypeSystem.CoreLibrary);
            var derivedType = new TypeDefinition("Test", "DerivedAttribute", TypeAttributes.Public, baseType);
            module.Types.Add(derivedType);

            var attribute = new CustomAttribute(module.ImportReference(typeof(ObsoleteAttribute).GetConstructors().First()));
            typeDef.CustomAttributes.Add(attribute);

            // Act
            var result = typeDef.TryGetByBaseAttribute<Attribute>(out var foundAttribute);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(foundAttribute, Is.Not.Null);
        }

        [Test]
        public void TryGetByBaseAttribute_WithoutMatchingBaseType_ReturnsFalseAndNull() {
            // Arrange & Act
            var result = typeDef.TryGetByBaseAttribute<ObsoleteAttribute>(out var foundAttribute);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(foundAttribute, Is.Null);
        }

        [Test]
        public void GetAttributeValue_WithExistingProperty_ReturnsValue() {
            // Arrange
            var attribute = new CustomAttribute(module.ImportReference(typeof(ObsoleteAttribute).GetConstructors().First()));
            var property = new CustomAttributeNamedArgument("TestProperty", new CustomAttributeArgument(module.TypeSystem.String, "TestValue"));
            attribute.Properties.Add(property);

            // Act
            var result = attribute.GetAttributeValue<string>("TestProperty");

            // Assert
            Assert.That(result, Is.EqualTo("TestValue"));
        }

        [Test]
        public void GetAttributeValue_WithNonExistingProperty_ReturnsDefault() {
            // Arrange
            var attribute = new CustomAttribute(module.ImportReference(typeof(ObsoleteAttribute).GetConstructors().First()));

            // Act
            var result = attribute.GetAttributeValue<string?>("NonExistingProperty");

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void GetConstructorArgument_WithValidIndex_ReturnsValue() {
            // Arrange
            var attribute = new CustomAttribute(module.ImportReference(typeof(ObsoleteAttribute).GetConstructors().First()));
            attribute.ConstructorArguments.Add(new CustomAttributeArgument(module.TypeSystem.String, "TestValue"));

            // Act
            var result = attribute.GetConstructorArgument<string>(0);

            // Assert
            Assert.That(result, Is.EqualTo("TestValue"));
        }

        [Test]
        public void GetConstructorArgument_WithInvalidIndex_ReturnsDefault() {
            // Arrange
            var attribute = new CustomAttribute(module.ImportReference(typeof(ObsoleteAttribute).GetConstructors().First()));

            // Act
            var result = attribute.GetConstructorArgument<string>(5);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void TryGetHttpAttributeTemplate_WithAttributeAndTemplate_ReturnsTrueAndTemplate() {
            // Arrange
            var method = new MethodDefinition("TestMethod", MethodAttributes.Public, module.TypeSystem.Void);
            var attributeType = new TypeReference(typeof(HttpGetAttribute).Namespace, nameof(HttpGetAttribute), module, module.TypeSystem.CoreLibrary);
            var attribute = new CustomAttribute(module.ImportReference(typeof(HttpGetAttribute).GetConstructors().First()));
            attribute.ConstructorArguments.Add(new CustomAttributeArgument(module.TypeSystem.String, "/api/test"));
            method.CustomAttributes.Add(attribute);

            // Act
            var result = method.TryGetHttpAttributeTemplate<HttpGetAttribute>(out var template);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(template, Is.EqualTo("/api/test"));
        }

        [Test]
        public void TryGetHttpAttributeTemplate_WithAttributeButNoTemplate_ReturnsTrueAndEmptyString() {
            // Arrange
            var method = new MethodDefinition("TestMethod", MethodAttributes.Public, module.TypeSystem.Void);
            var attributeType = new TypeReference(typeof(HttpGetAttribute).Namespace, nameof(HttpGetAttribute), module, module.TypeSystem.CoreLibrary);
            var attribute = new CustomAttribute(module.ImportReference(typeof(HttpGetAttribute).GetConstructors().First()));
            method.CustomAttributes.Add(attribute);

            // Act
            var result = method.TryGetHttpAttributeTemplate<HttpGetAttribute>(out var template);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(template, Is.EqualTo(string.Empty));
        }

        [Test]
        public void TryGetHttpAttributeTemplate_WithoutAttribute_ReturnsFalseAndEmptyString() {
            // Arrange
            var method = new MethodDefinition("TestMethod", MethodAttributes.Public, module.TypeSystem.Void);

            // Act
            var result = method.TryGetHttpAttributeTemplate<HttpGetAttribute>(out var template);

            // Assert
            Assert.That(result, Is.False);
            Assert.That(template, Is.EqualTo(string.Empty));
        }

        [Test]
        public void GetAttribute_WithMatchingAttribute_ReturnsAttribute() {
            // Arrange
            var attribute = new CustomAttribute(module.ImportReference(typeof(ObsoleteAttribute).GetConstructors().First()));
            typeDef.CustomAttributes.Add(attribute);

            // Act
            var result = typeDef.GetAttribute<ObsoleteAttribute>();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.AttributeType.FullName, Is.EqualTo(typeof(ObsoleteAttribute).FullName));
        }

        [Test]
        public void GetAttribute_WithoutMatchingAttribute_ReturnsNull() {
            // Arrange, Act & Assert
            Assert.Throws<InvalidOperationException>(() => typeDef.GetAttribute<ObsoleteAttribute>());
        }
    }
}
