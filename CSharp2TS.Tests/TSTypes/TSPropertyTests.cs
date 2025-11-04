using CSharp2TS.CLI.Generators.Entities;
using Mono.Cecil;
using Moq;

namespace CSharp2TS.Tests.TSTypes {
    public class TSPropertyTests {
        private Mock<TypeReference> CreateMockTypeReference(bool isGenericInstance = false) {
            var mock = new Mock<TypeReference>("System", "Object", null!, null!);
            mock.Setup(x => x.IsGenericInstance).Returns(isGenericInstance);
            return mock;
        }

        [Test]
        public void GetTypeName_String_ReturnsString() {
            var property = new TSProperty {
                TypeRef = CreateMockTypeReference().Object,
                TSType = RawTSType.String,
                IsObject = false,
                ObjectName = null,
                IsTypeNullable = false,
                IsPropertyNullable = false,
                IsDictionary = false,
                IsCollection = false,
                JaggedCount = 0,
                GenericArguments = []
            };

            Assert.That(property.GetTypeName(), Is.EqualTo("string"));
        }

        [Test]
        public void GetTypeName_Number_ReturnsNumber() {
            var property = new TSProperty {
                TypeRef = CreateMockTypeReference().Object,
                TSType = RawTSType.Number,
                IsObject = false,
                ObjectName = null,
                IsTypeNullable = false,
                IsPropertyNullable = false,
                IsDictionary = false,
                IsCollection = false,
                JaggedCount = 0,
                GenericArguments = []
            };

            Assert.That(property.GetTypeName(), Is.EqualTo("number"));
        }

        [Test]
        public void GetTypeName_Boolean_ReturnsBoolean() {
            var property = new TSProperty {
                TypeRef = CreateMockTypeReference().Object,
                TSType = RawTSType.Boolean,
                IsObject = false,
                ObjectName = null,
                IsTypeNullable = false,
                IsPropertyNullable = false,
                IsDictionary = false,
                IsCollection = false,
                JaggedCount = 0,
                GenericArguments = []
            };

            Assert.That(property.GetTypeName(), Is.EqualTo("boolean"));
        }

        [Test]
        public void GetTypeName_Object_ReturnsObjectName() {
            var property = new TSProperty {
                TypeRef = CreateMockTypeReference().Object,
                TSType = RawTSType.Object,
                IsObject = true,
                ObjectName = "MyClass",
                IsTypeNullable = false,
                IsPropertyNullable = false,
                IsDictionary = false,
                IsCollection = false,
                JaggedCount = 0,
                GenericArguments = []
            };

            Assert.That(property.GetTypeName(), Is.EqualTo("MyClass"));
        }

        [Test]
        public void IsNullable_WhenTypeNullable_ReturnsTrue() {
            var property = new TSProperty {
                TypeRef = CreateMockTypeReference().Object,
                TSType = RawTSType.String,
                IsObject = false,
                ObjectName = null,
                IsTypeNullable = true,
                IsPropertyNullable = false,
                IsDictionary = false,
                IsCollection = false,
                JaggedCount = 0,
                GenericArguments = []
            };

            Assert.That(property.IsNullable, Is.True);
        }

        [Test]
        public void IsNullable_WhenPropertyNullable_ReturnsTrue() {
            var property = new TSProperty {
                TypeRef = CreateMockTypeReference().Object,
                TSType = RawTSType.String,
                IsObject = false,
                ObjectName = null,
                IsTypeNullable = false,
                IsPropertyNullable = true,
                IsDictionary = false,
                IsCollection = false,
                JaggedCount = 0,
                GenericArguments = []
            };

            Assert.That(property.IsNullable, Is.True);
        }

        [Test]
        public void ToString_SimpleString_ReturnsString() {
            var property = new TSProperty {
                TypeRef = CreateMockTypeReference().Object,
                TSType = RawTSType.String,
                IsObject = false,
                ObjectName = null,
                IsTypeNullable = false,
                IsPropertyNullable = false,
                IsDictionary = false,
                IsCollection = false,
                JaggedCount = 0,
                GenericArguments = []
            };

            Assert.That(property.ToString(), Is.EqualTo("string"));
        }

        [Test]
        public void ToString_TypeNullable_ReturnsStringWithNull() {
            var property = new TSProperty {
                TypeRef = CreateMockTypeReference().Object,
                TSType = RawTSType.String,
                IsObject = false,
                ObjectName = null,
                IsTypeNullable = true,
                IsPropertyNullable = false,
                IsDictionary = false,
                IsCollection = false,
                JaggedCount = 0,
                GenericArguments = []
            };

            Assert.That(property.ToString(), Is.EqualTo("string | null"));
        }

        [Test]
        public void ToString_PropertyNullable_ReturnsStringWithNull() {
            var property = new TSProperty {
                TypeRef = CreateMockTypeReference().Object,
                TSType = RawTSType.String,
                IsObject = false,
                ObjectName = null,
                IsTypeNullable = false,
                IsPropertyNullable = true,
                IsDictionary = false,
                IsCollection = false,
                JaggedCount = 0,
                GenericArguments = []
            };

            Assert.That(property.ToString(), Is.EqualTo("string | null"));
        }

        [Test]
        public void ToString_Collection_ReturnsArraySyntax() {
            var property = new TSProperty {
                TypeRef = CreateMockTypeReference().Object,
                TSType = RawTSType.String,
                IsObject = false,
                ObjectName = null,
                IsTypeNullable = false,
                IsPropertyNullable = false,
                IsDictionary = false,
                IsCollection = true,
                JaggedCount = 1,
                GenericArguments = []
            };

            Assert.That(property.ToString(), Is.EqualTo("string[]"));
        }

        [Test]
        public void ToString_JaggedArray_ReturnsMultipleBrackets() {
            var property = new TSProperty {
                TypeRef = CreateMockTypeReference().Object,
                TSType = RawTSType.Number,
                IsObject = false,
                ObjectName = null,
                IsTypeNullable = false,
                IsPropertyNullable = false,
                IsDictionary = false,
                IsCollection = true,
                JaggedCount = 3,
                GenericArguments = []
            };

            Assert.That(property.ToString(), Is.EqualTo("number[][][]"));
        }

        [Test]
        public void ToString_Dictionary_ReturnsDictionarySyntax() {
            var property = new TSProperty {
                TypeRef = CreateMockTypeReference().Object,
                TSType = RawTSType.String,
                IsObject = false,
                ObjectName = null,
                IsTypeNullable = false,
                IsPropertyNullable = false,
                IsDictionary = true,
                IsCollection = false,
                JaggedCount = 0,
                GenericArguments = []
            };

            Assert.That(property.ToString(), Is.EqualTo("{ [key: string]: string }"));
        }

        [Test]
        public void ToString_NullableCollection_ReturnsParenthesizedNullUnion() {
            var property = new TSProperty {
                TypeRef = CreateMockTypeReference().Object,
                TSType = RawTSType.Number,
                IsObject = false,
                ObjectName = null,
                IsTypeNullable = true,
                IsPropertyNullable = false,
                IsDictionary = false,
                IsCollection = true,
                JaggedCount = 1,
                GenericArguments = []
            };

            Assert.That(property.ToString(), Is.EqualTo("(number | null)[]"));
        }

        [Test]
        public void ToString_NullableDictionary_ReturnsParenthesizedNullUnion() {
            var property = new TSProperty {
                TypeRef = CreateMockTypeReference().Object,
                TSType = RawTSType.String,
                IsObject = false,
                ObjectName = null,
                IsTypeNullable = true,
                IsPropertyNullable = false,
                IsDictionary = true,
                IsCollection = false,
                JaggedCount = 0,
                GenericArguments = []
            };

            Assert.That(property.ToString(), Is.EqualTo("{ [key: string]: (string | null) }"));
        }

        [Test]
        public void ToString_GenericType_ReturnsGenericSyntax() {
            var innerProperty = new TSProperty {
                TypeRef = CreateMockTypeReference().Object,
                TSType = RawTSType.String,
                IsObject = false,
                ObjectName = null,
                IsTypeNullable = false,
                IsPropertyNullable = false,
                IsDictionary = false,
                IsCollection = false,
                JaggedCount = 0,
                GenericArguments = []
            };

            var property = new TSProperty {
                TypeRef = CreateMockTypeReference(isGenericInstance: true).Object,
                TSType = RawTSType.Object,
                IsObject = true,
                ObjectName = "TestClass",
                IsTypeNullable = false,
                IsPropertyNullable = false,
                IsDictionary = false,
                IsCollection = false,
                JaggedCount = 0,
                GenericArguments = [innerProperty]
            };

            Assert.That(property.ToString(), Is.EqualTo("TestClass<string>"));
        }

        [Test]
        public void ToString_GenericType_ReturnsGenericSyntax_Multiple() {
            var innerProperty = new TSProperty {
                TypeRef = CreateMockTypeReference().Object,
                TSType = RawTSType.String,
                IsObject = false,
                ObjectName = null,
                IsTypeNullable = false,
                IsPropertyNullable = false,
                IsDictionary = false,
                IsCollection = false,
                JaggedCount = 0,
                GenericArguments = []
            };

            var innerProperty2 = new TSProperty {
                TypeRef = CreateMockTypeReference().Object,
                TSType = RawTSType.Number,
                IsObject = false,
                ObjectName = null,
                IsTypeNullable = false,
                IsPropertyNullable = false,
                IsDictionary = false,
                IsCollection = false,
                JaggedCount = 0,
                GenericArguments = []
            };

            var property = new TSProperty {
                TypeRef = CreateMockTypeReference(isGenericInstance: true).Object,
                TSType = RawTSType.Object,
                IsObject = true,
                ObjectName = "TestClass",
                IsTypeNullable = false,
                IsPropertyNullable = false,
                IsDictionary = false,
                IsCollection = false,
                JaggedCount = 0,
                GenericArguments = [innerProperty, innerProperty2]
            };

            Assert.That(property.ToString(), Is.EqualTo("TestClass<string, number>"));
        }

        [Test]
        public void ToString_BothTypeAndPropertyNullable_OnlyAddsNullOnce() {
            var property = new TSProperty {
                TypeRef = CreateMockTypeReference().Object,
                TSType = RawTSType.String,
                IsObject = false,
                ObjectName = null,
                IsTypeNullable = true,
                IsPropertyNullable = true,
                IsDictionary = false,
                IsCollection = false,
                JaggedCount = 0,
                GenericArguments = []
            };

            var result = property.ToString();
            Assert.That(result, Is.EqualTo("string | null"));
        }
    }
}
