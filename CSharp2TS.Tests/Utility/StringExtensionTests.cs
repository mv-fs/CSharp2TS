using CSharp2TS.CLI.Utility;

namespace CSharp2TS.Tests.Utility {
    public class StringExtensionTests {
        [TestCase(null!, null!)]
        [TestCase("", "")]
        [TestCase(" ", " ")]
        [TestCase("A", "a")]
        [TestCase("a", "a")]
        [TestCase("Test", "test")]
        [TestCase("test", "test")]
        [TestCase("TestString", "testString")]
        [TestCase("TEST", "tEST")]
        public void ToCamelCase(string input, string expected) {
            Assert.That(input.ToCamelCase(), Is.EqualTo(expected));
        }

        [TestCase(null!, null!)]
        [TestCase("", "")]
        [TestCase(" ", " ")]
        [TestCase("a", "A")]
        [TestCase("A", "A")]
        [TestCase("test", "Test")]
        [TestCase("Test", "Test")]
        [TestCase("testString", "TestString")]
        [TestCase("tEST", "TEST")]
        public void ToPascalCase(string input, string expected) {
            Assert.That(input.ToPascalCase(), Is.EqualTo(expected));
        }
    }
}
