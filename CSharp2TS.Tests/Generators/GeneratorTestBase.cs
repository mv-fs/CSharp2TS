namespace CSharp2TS.Tests.Generators {
    public abstract class GeneratorTestBase {
        protected void TestMatchesFile(string expectedFile, string actualContents) {
            if (!File.Exists(expectedFile)) {
                Assert.Fail("Expected file does not exist.");
            }

            string expected = File.ReadAllText(expectedFile);

            static string NormalizeLineEndings(string s) => s.Replace("\r\n", "\n").Replace("\r", "\n");
            expected = NormalizeLineEndings(expected);
            actualContents = NormalizeLineEndings(actualContents);


            Assert.That(actualContents, Is.EqualTo(expected));
        }
    }
}
