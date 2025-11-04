using CSharp2TS.CLI.Generators.Common;

namespace CSharp2TS.Tests.TSTypes {
    public class TSTypeTests {
        [Test]
        public void ToString_SimpleString_ReturnsString() {
            var tsType = new TSType {
                TypeName = "string"
            };

            Assert.That(tsType.ToString(), Is.EqualTo("string"));
        }

        [Test]
        public void ToString_SimpleNumber_ReturnsNumber() {
            var tsType = new TSType {
                TypeName = "number"
            };

            Assert.That(tsType.ToString(), Is.EqualTo("number"));
        }

        [Test]
        public void ToString_SimpleBoolean_ReturnsBoolean() {
            var tsType = new TSType {
                TypeName = "boolean"
            };

            Assert.That(tsType.ToString(), Is.EqualTo("boolean"));
        }

        [Test]
        public void ToString_ObjectName_ReturnsObjectName() {
            var tsType = new TSType {
                TypeName = "MyClass"
            };

            Assert.That(tsType.ToString(), Is.EqualTo("MyClass"));
        }

        [Test]
        public void ToString_Nullable_ReturnsStringWithNull() {
            var tsType = new TSType {
                TypeName = "string",
                IsNullable = true
            };

            Assert.That(tsType.ToString(), Is.EqualTo("string | null"));
        }

        [Test]
        public void ToString_Collection_ReturnsArraySyntax() {
            var tsType = new TSType {
                TypeName = "string",
                IsCollection = true,
                JaggedCount = 1
            };

            Assert.That(tsType.ToString(), Is.EqualTo("string[]"));
        }

        [Test]
        public void ToString_JaggedArray_ReturnsMultipleBrackets() {
            var tsType = new TSType {
                TypeName = "number",
                IsCollection = true,
                JaggedCount = 3
            };

            Assert.That(tsType.ToString(), Is.EqualTo("number[][][]"));
        }

        [Test]
        public void ToString_Dictionary_ReturnsDictionarySyntax() {
            var tsType = new TSType {
                TypeName = "string",
                IsDictionary = true
            };

            Assert.That(tsType.ToString(), Is.EqualTo("{ [key: string]: string }"));
        }

        [Test]
        public void ToString_NullableCollection_ReturnsParenthesizedNullUnion() {
            var tsType = new TSType {
                TypeName = "number",
                IsNullable = true,
                IsCollection = true,
                JaggedCount = 1
            };

            Assert.That(tsType.ToString(), Is.EqualTo("(number | null)[]"));
        }

        [Test]
        public void ToString_NullableDictionary_ReturnsParenthesizedNullUnion() {
            var tsType = new TSType {
                TypeName = "string",
                IsNullable = true,
                IsDictionary = true
            };

            Assert.That(tsType.ToString(), Is.EqualTo("{ [key: string]: (string | null) }"));
        }

        [Test]
        public void ToString_GenericType_ReturnsGenericSyntax() {
            var innerType = new TSType {
                TypeName = "string"
            };

            var tsType = new TSType {
                TypeName = "TestClass",
                GenericArguments = [innerType]
            };

            Assert.That(tsType.ToString(), Is.EqualTo("TestClass<string>"));
        }

        [Test]
        public void ToString_GenericType_ReturnsGenericSyntax_Multiple() {
            var innerType1 = new TSType {
                TypeName = "string"
            };

            var innerType2 = new TSType {
                TypeName = "number"
            };

            var tsType = new TSType {
                TypeName = "TestClass",
                GenericArguments = [innerType1, innerType2]
            };

            Assert.That(tsType.ToString(), Is.EqualTo("TestClass<string, number>"));
        }

        [Test]
        public void ToString_CollectionWithoutJaggedCount_ReturnsTypeWithoutBrackets() {
            var tsType = new TSType {
                TypeName = "string",
                IsCollection = true,
                JaggedCount = 0
            };

            Assert.That(tsType.ToString(), Is.EqualTo("string"));
        }
    }
}
