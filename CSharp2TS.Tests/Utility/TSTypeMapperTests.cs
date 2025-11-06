using CSharp2TS.CLI;
using CSharp2TS.CLI.Generators.Common;
using CSharp2TS.CLI.Generators.Entities;
using Mono.Cecil;

namespace CSharp2TS.Tests.Utility {
    public class TSTypeMapperTests {
        private ModuleDefinition module = null!;
        private Options options = null!;

        [SetUp]
        public void Setup() {
            // Load the test assembly to get TypeReferences
            string assemblyPath = typeof(TSTypeMapperTests).Assembly.Location;
            module = ModuleDefinition.ReadModule(assemblyPath);

            options = new Options {
                UseNullableStrings = false
            };
        }

        [TearDown]
        public void TearDown() {
            module?.Dispose();
        }

        private TypeReference GetTypeReference(Type type) {
            return module.ImportReference(type);
        }

        #region String Types

        [Test]
        [TestCase(typeof(string))]
        [TestCase(typeof(char))]
        [TestCase(typeof(Guid))]
        [TestCase(typeof(DateTime))]
        [TestCase(typeof(DateTimeOffset))]
        [TestCase(typeof(DateOnly))]
        [TestCase(typeof(TimeOnly))]
        public void GetTSPropertyType_StringTypes_ReturnsStringType(Type stringType) {
            // Arrange
            var typeRef = GetTypeReference(stringType);

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(TSTypeConsts.String));
            Assert.That(result.IsNullable, Is.False);
            Assert.That(result.IsCollection, Is.False);
            Assert.That(result.JaggedCount, Is.Zero);
            Assert.That(result.IsDictionary, Is.False);
            Assert.That(result.GenericArguments.Count(), Is.Zero);
        }

        [Test]
        public void GetTSPropertyType_String_WithNullableStringsOption_ReturnsNullableString() {
            // Arrange
            var typeRef = GetTypeReference(typeof(string));
            var optionsWithNullableStrings = new Options { UseNullableStrings = true };

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, optionsWithNullableStrings);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(TSTypeConsts.String));
            Assert.That(result.IsNullable, Is.True);
            Assert.That(result.IsCollection, Is.False);
            Assert.That(result.JaggedCount, Is.Zero);
            Assert.That(result.IsDictionary, Is.False);
            Assert.That(result.GenericArguments.Count(), Is.Zero);
        }

        #endregion

        #region Number Types

        [Test]
        [TestCase(typeof(int))]
        [TestCase(typeof(long))]
        [TestCase(typeof(short))]
        [TestCase(typeof(byte))]
        [TestCase(typeof(sbyte))]
        [TestCase(typeof(uint))]
        [TestCase(typeof(ulong))]
        [TestCase(typeof(ushort))]
        [TestCase(typeof(float))]
        [TestCase(typeof(double))]
        [TestCase(typeof(decimal))]
        public void GetTSPropertyType_NumericTypes_ReturnsNumberType(Type numericType) {
            // Arrange
            var typeRef = GetTypeReference(numericType);

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(TSTypeConsts.Number));
            Assert.That(result.IsNullable, Is.False);
            Assert.That(result.IsCollection, Is.False);
            Assert.That(result.JaggedCount, Is.Zero);
            Assert.That(result.IsDictionary, Is.False);
            Assert.That(result.GenericArguments.Count(), Is.Zero);
        }

        #endregion

        #region Boolean Type

        [Test]
        public void GetTSPropertyType_Boolean_ReturnsBooleanType() {
            // Arrange
            var typeRef = GetTypeReference(typeof(bool));

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(TSTypeConsts.Boolean));
            Assert.That(result.IsNullable, Is.False);
            Assert.That(result.IsCollection, Is.False);
            Assert.That(result.JaggedCount, Is.Zero);
            Assert.That(result.IsDictionary, Is.False);
            Assert.That(result.GenericArguments.Count(), Is.Zero);
        }

        #endregion

        #region Void Types

        [Test]
        [TestCase(typeof(void))]
        [TestCase(typeof(Task))]
        public void GetTSPropertyType_VoidTypes_ReturnsVoidType(Type voidType) {
            // Arrange
            var typeRef = GetTypeReference(voidType);

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(TSTypeConsts.Void));
            Assert.That(result.IsNullable, Is.False);
            Assert.That(result.IsCollection, Is.False);
            Assert.That(result.JaggedCount, Is.Zero);
            Assert.That(result.IsDictionary, Is.False);
            Assert.That(result.GenericArguments.Count(), Is.Zero);
        }

        #endregion

        #region Nullable Types

        [Test]
        [TestCase(typeof(int?), TSTypeConsts.Number)]
        [TestCase(typeof(bool?), TSTypeConsts.Boolean)]
        [TestCase(typeof(DateTime?), TSTypeConsts.String)]
        public void GetTSPropertyType_NullableTypes_ReturnsNullableType(Type nullableType, string expectedTypeName) {
            // Arrange
            var typeRef = GetTypeReference(nullableType);

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(expectedTypeName));
            Assert.That(result.IsNullable, Is.True);
            Assert.That(result.IsCollection, Is.False);
            Assert.That(result.JaggedCount, Is.Zero);
            Assert.That(result.IsDictionary, Is.False);
            Assert.That(result.GenericArguments.Count(), Is.Zero);
        }

        #endregion

        #region Collection Types

        [Test]
        [TestCase(typeof(int[]), TSTypeConsts.Number, 1)]
        [TestCase(typeof(int[][]), TSTypeConsts.Number, 2)]
        public void GetTSPropertyType_Arrays_ReturnsCollectionType(Type arrayType, string expectedTypeName, int expectedJaggedCount) {
            // Arrange
            var typeRef = GetTypeReference(arrayType);

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(expectedTypeName));
            Assert.That(result.IsNullable, Is.False);
            Assert.That(result.IsCollection, Is.True);
            Assert.That(result.JaggedCount, Is.EqualTo(expectedJaggedCount));
            Assert.That(result.IsDictionary, Is.False);
            Assert.That(result.GenericArguments.Count(), Is.Zero);
        }

        [Test]
        [TestCase(typeof(List<string>), TSTypeConsts.String)]
        [TestCase(typeof(IEnumerable<int>), TSTypeConsts.Number)]
        [TestCase(typeof(HashSet<bool>), TSTypeConsts.Boolean)]
        public void GetTSPropertyType_GenericCollections_ReturnsCollectionType(Type collectionType, string expectedTypeName) {
            // Arrange
            var typeRef = GetTypeReference(collectionType);

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(expectedTypeName));
            Assert.That(result.IsNullable, Is.False);
            Assert.That(result.IsCollection, Is.True);
            Assert.That(result.JaggedCount, Is.EqualTo(1));
            Assert.That(result.IsDictionary, Is.False);
            Assert.That(result.GenericArguments.Count(), Is.Zero);
        }

        #endregion

        #region Dictionary Types

        [Test]
        [TestCase(typeof(Dictionary<string, int>), TSTypeConsts.Number)]
        [TestCase(typeof(IDictionary<string, string>), TSTypeConsts.String)]
        public void GetTSPropertyType_Dictionaries_ReturnsDictionaryType(Type dictionaryType, string expectedValueTypeName) {
            // Arrange
            var typeRef = GetTypeReference(dictionaryType);

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(expectedValueTypeName));
            Assert.That(result.IsNullable, Is.False);
            Assert.That(result.IsCollection, Is.False);
            Assert.That(result.JaggedCount, Is.Zero);
            Assert.That(result.IsDictionary, Is.True);
            Assert.That(result.GenericArguments.Count(), Is.Zero);
        }

        [Test]
        public void GetTSPropertyType_DictionaryWithArrayValue_ReturnsDictionaryAndCollection() {
            // Arrange
            var typeRef = GetTypeReference(typeof(Dictionary<string, int[]>));

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(TSTypeConsts.Number));
            Assert.That(result.IsNullable, Is.False);
            Assert.That(result.IsCollection, Is.True);
            Assert.That(result.JaggedCount, Is.EqualTo(1));
            Assert.That(result.IsDictionary, Is.True);
            Assert.That(result.GenericArguments.Count(), Is.Zero);
        }

        #endregion

        #region File Types

        [Test]
        public void GetTSPropertyType_IFormFile_ReturnsFileType() {
            // Arrange
            var typeRef = module.ImportReference(typeof(Microsoft.AspNetCore.Http.IFormFile));

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(TSTypeConsts.File));
            Assert.That(result.IsNullable, Is.False);
            Assert.That(result.IsCollection, Is.False);
            Assert.That(result.JaggedCount, Is.Zero);
            Assert.That(result.IsDictionary, Is.False);
            Assert.That(result.GenericArguments.Count(), Is.Zero);
        }

        [Test]
        public void GetTSPropertyType_IFormFileCollection_ReturnsFileCollectionType() {
            // Arrange
            var typeRef = module.ImportReference(typeof(Microsoft.AspNetCore.Http.IFormFileCollection));

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(TSTypeConsts.File));
            Assert.That(result.IsNullable, Is.False);
            Assert.That(result.IsCollection, Is.True);
            Assert.That(result.JaggedCount, Is.EqualTo(1));
            Assert.That(result.IsDictionary, Is.False);
            Assert.That(result.GenericArguments.Count(), Is.Zero);
        }

        #endregion

        #region FormData Types

        [Test]
        public void GetTSPropertyType_IFormCollection_ReturnsFormDataType() {
            // Arrange
            var typeRef = module.ImportReference(typeof(Microsoft.AspNetCore.Http.IFormCollection));

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(TSTypeConsts.FormData));
            Assert.That(result.IsNullable, Is.False);
            Assert.That(result.IsCollection, Is.False);
            Assert.That(result.JaggedCount, Is.Zero);
            Assert.That(result.IsDictionary, Is.False);
            Assert.That(result.GenericArguments.Count(), Is.Zero);
        }

        #endregion

        #region Unknown Types

        [Test]
        public void GetTSPropertyType_JsonElement_ReturnsUnknownType() {
            // Arrange
            var typeRef = GetTypeReference(typeof(System.Text.Json.JsonElement));

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(TSTypeConsts.Unknown));
            Assert.That(result.IsNullable, Is.False);
            Assert.That(result.IsCollection, Is.False);
            Assert.That(result.JaggedCount, Is.Zero);
            Assert.That(result.IsDictionary, Is.False);
            Assert.That(result.GenericArguments.Count(), Is.Zero);
        }

        #endregion

        #region Generic Types

        [Test]
        [TestCase(typeof(Task<string>), TSTypeConsts.String)]
        [TestCase(typeof(Task<int>), TSTypeConsts.Number)]
        public void GetTSPropertyType_TaskOfT_ExtractsGenericAndReturnsCorrectType(Type taskType, string expectedTypeName) {
            // Arrange
            var typeRef = GetTypeReference(taskType);

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(expectedTypeName));
            Assert.That(result.IsNullable, Is.False);
            Assert.That(result.IsCollection, Is.False);
            Assert.That(result.JaggedCount, Is.Zero);
            Assert.That(result.IsDictionary, Is.False);
            Assert.That(result.GenericArguments.Count(), Is.Zero);
        }

        #endregion

        #region Custom Object Types

        [Test]
        public void GetTSPropertyType_CustomClass_ReturnsObjectType() {
            // Arrange
            var typeRef = GetTypeReference(typeof(TSTypeMapperTests));

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo("TSTypeMapperTests"));
            Assert.That(result.IsNullable, Is.False);
            Assert.That(result.IsCollection, Is.False);
            Assert.That(result.JaggedCount, Is.Zero);
            Assert.That(result.IsDictionary, Is.False);
            Assert.That(result.GenericArguments.Count(), Is.Zero);
        }

        #endregion

        #region Import Handler

        [Test]
        [TestCase(typeof(TSTypeMapperTests), true)]
        [TestCase(typeof(int), false)]
        public void GetTSPropertyType_ImportHandler_CalledForCustomTypesOnly(Type type, bool shouldCallHandler) {
            // Arrange
            var typeRef = GetTypeReference(type);
            bool importHandlerCalled = false;

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options, importHandler: (fullName, typeName) => {
                importHandlerCalled = true;
                return true;
            });

            // Assert
            Assert.That(importHandlerCalled, Is.EqualTo(shouldCallHandler));
        }

        #endregion
    }
}