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
        public void GetTSPropertyType_String_ReturnsStringType() {
            // Arrange
            var typeRef = GetTypeReference(typeof(string));

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
            Assert.That(result.TypeName, Is.EqualTo(TSTypeConsts.String));
            Assert.That(result.IsNullable, Is.True);
            Assert.That(result.IsCollection, Is.False);
            Assert.That(result.JaggedCount, Is.Zero);
            Assert.That(result.IsDictionary, Is.False);
            Assert.That(result.GenericArguments.Count(), Is.Zero);
        }

        [Test]
        public void GetTSPropertyType_Char_ReturnsStringType() {
            // Arrange
            var typeRef = GetTypeReference(typeof(char));

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(TSTypeConsts.String));
            Assert.That(result.IsNullable, Is.False);
        }

        [Test]
        public void GetTSPropertyType_Guid_ReturnsStringType() {
            // Arrange
            var typeRef = GetTypeReference(typeof(Guid));

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(TSTypeConsts.String));
        }

        [Test]
        public void GetTSPropertyType_DateTime_ReturnsStringType() {
            // Arrange
            var typeRef = GetTypeReference(typeof(DateTime));

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(TSTypeConsts.String));
        }

        [Test]
        public void GetTSPropertyType_DateTimeOffset_ReturnsStringType() {
            // Arrange
            var typeRef = GetTypeReference(typeof(DateTimeOffset));

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(TSTypeConsts.String));
        }

        [Test]
        public void GetTSPropertyType_DateOnly_ReturnsStringType() {
            // Arrange
            var typeRef = GetTypeReference(typeof(DateOnly));

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(TSTypeConsts.String));
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
        }

        #endregion

        #region Void Types

        [Test]
        public void GetTSPropertyType_Void_ReturnsVoidType() {
            // Arrange
            var typeRef = GetTypeReference(typeof(void));

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(TSTypeConsts.Void));
        }

        [Test]
        public void GetTSPropertyType_Task_ReturnsVoidType() {
            // Arrange
            var typeRef = GetTypeReference(typeof(Task));

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(TSTypeConsts.Void));
        }

        #endregion

        #region Nullable Types

        [Test]
        public void GetTSPropertyType_NullableInt_ReturnsNullableNumber() {
            // Arrange
            var typeRef = GetTypeReference(typeof(int?));

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(TSTypeConsts.Number));
            Assert.That(result.IsNullable, Is.True);
        }

        [Test]
        public void GetTSPropertyType_NullableBool_ReturnsNullableBoolean() {
            // Arrange
            var typeRef = GetTypeReference(typeof(bool?));

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(TSTypeConsts.Boolean));
            Assert.That(result.IsNullable, Is.True);
        }

        [Test]
        public void GetTSPropertyType_NullableDateTime_ReturnsNullableString() {
            // Arrange
            var typeRef = GetTypeReference(typeof(DateTime?));

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(TSTypeConsts.String));
            Assert.That(result.IsNullable, Is.True);
        }

        #endregion

        #region Collection Types

        [Test]
        public void GetTSPropertyType_Array_ReturnsCollectionType() {
            // Arrange
            var typeRef = GetTypeReference(typeof(int[]));

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(TSTypeConsts.Number));
            Assert.That(result.IsCollection, Is.True);
            Assert.That(result.JaggedCount, Is.EqualTo(1));
        }

        [Test]
        public void GetTSPropertyType_JaggedArray_ReturnsCorrectJaggedCount() {
            // Arrange
            var typeRef = GetTypeReference(typeof(int[][]));

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(TSTypeConsts.Number));
            Assert.That(result.IsCollection, Is.True);
            Assert.That(result.JaggedCount, Is.EqualTo(2));
        }

        [Test]
        public void GetTSPropertyType_List_ReturnsCollectionType() {
            // Arrange
            var typeRef = GetTypeReference(typeof(List<string>));

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(TSTypeConsts.String));
            Assert.That(result.IsCollection, Is.True);
            Assert.That(result.JaggedCount, Is.EqualTo(1));
        }

        [Test]
        public void GetTSPropertyType_IEnumerable_ReturnsCollectionType() {
            // Arrange
            var typeRef = GetTypeReference(typeof(IEnumerable<int>));

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(TSTypeConsts.Number));
            Assert.That(result.IsCollection, Is.True);
        }

        [Test]
        public void GetTSPropertyType_HashSet_ReturnsCollectionType() {
            // Arrange
            var typeRef = GetTypeReference(typeof(HashSet<bool>));

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(TSTypeConsts.Boolean));
            Assert.That(result.IsCollection, Is.True);
        }

        #endregion

        #region Dictionary Types

        [Test]
        public void GetTSPropertyType_Dictionary_ReturnsDictionaryType() {
            // Arrange
            var typeRef = GetTypeReference(typeof(Dictionary<string, int>));

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(TSTypeConsts.Number));
            Assert.That(result.IsDictionary, Is.True);
            Assert.That(result.IsCollection, Is.False);
        }

        [Test]
        public void GetTSPropertyType_IDictionary_ReturnsDictionaryType() {
            // Arrange
            var typeRef = GetTypeReference(typeof(IDictionary<string, string>));

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(TSTypeConsts.String));
            Assert.That(result.IsDictionary, Is.True);
        }

        [Test]
        public void GetTSPropertyType_DictionaryWithArrayValue_ReturnsDictionaryAndCollection() {
            // Arrange
            var typeRef = GetTypeReference(typeof(Dictionary<string, int[]>));

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(TSTypeConsts.Number));
            Assert.That(result.IsDictionary, Is.True);
            Assert.That(result.IsCollection, Is.True);
            Assert.That(result.JaggedCount, Is.EqualTo(1));
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
        }

        [Test]
        public void GetTSPropertyType_IFormFileCollection_ReturnsFileCollectionType() {
            // Arrange
            var typeRef = module.ImportReference(typeof(Microsoft.AspNetCore.Http.IFormFileCollection));

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(TSTypeConsts.File));
            Assert.That(result.IsCollection, Is.True);
            Assert.That(result.JaggedCount, Is.EqualTo(1));
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
        }

        #endregion

        #region Generic Types

        [Test]
        public void GetTSPropertyType_TaskOfString_ExtractsGenericAndReturnsString() {
            // Arrange
            var typeRef = GetTypeReference(typeof(Task<string>));

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(TSTypeConsts.String));
        }

        [Test]
        public void GetTSPropertyType_TaskOfInt_ExtractsGenericAndReturnsNumber() {
            // Arrange
            var typeRef = GetTypeReference(typeof(Task<int>));

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Assert
            Assert.That(result.TypeName, Is.EqualTo(TSTypeConsts.Number));
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
        }

        #endregion

        #region Import Handler

        [Test]
        public void GetTSPropertyType_CustomObject_CallsImportHandler() {
            // Arrange
            var typeRef = GetTypeReference(typeof(TSTypeMapperTests));
            bool importHandlerCalled = false;

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options, importHandler: (prop) => {
                importHandlerCalled = true;
                return true;
            });

            // Assert
            Assert.That(importHandlerCalled, Is.True);
        }

        [Test]
        public void GetTSPropertyType_PrimitiveType_DoesNotCallImportHandler() {
            // Arrange
            var typeRef = GetTypeReference(typeof(int));
            bool importHandlerCalled = false;

            // Act
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options, importHandler: (prop) => {
                importHandlerCalled = true;
                return true;
            });

            // Assert
            Assert.That(importHandlerCalled, Is.False);
        }

        #endregion

        #region ToString Tests

        [Test]
        public void TSProperty_ToString_SimpleNumber_ReturnsCorrectString() {
            // Arrange
            var typeRef = GetTypeReference(typeof(int));
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Act
            var str = result.ToString();

            // Assert
            Assert.That(str, Is.EqualTo("number"));
        }

        [Test]
        public void TSProperty_ToString_NullableNumber_ReturnsCorrectString() {
            // Arrange
            var typeRef = GetTypeReference(typeof(int?));
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Act
            var str = result.ToString();

            // Assert
            Assert.That(str, Is.EqualTo("number | null"));
        }

        [Test]
        public void TSProperty_ToString_NumberArray_ReturnsCorrectString() {
            // Arrange
            var typeRef = GetTypeReference(typeof(int[]));
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Act
            var str = result.ToString();

            // Assert
            Assert.That(str, Is.EqualTo("number[]"));
        }

        [Test]
        public void TSProperty_ToString_Dictionary_ReturnsCorrectString() {
            // Arrange
            var typeRef = GetTypeReference(typeof(Dictionary<string, int>));
            var result = TSTypeMapper2.GetTSPropertyType(typeRef, options);

            // Act
            var str = result.ToString();

            // Assert
            Assert.That(str, Is.EqualTo("{ [key: string]: number }"));
        }

        #endregion
    }
}