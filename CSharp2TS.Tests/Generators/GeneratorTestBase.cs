namespace CSharp2TS.Tests.Generators {
    public abstract class GeneratorTestBase {
        protected void TestMatchesFile(string expectedFile, string actualContents) {
            if (!File.Exists(expectedFile)) {
                Assert.Fail("Expected file does not exist.");
            }

            string expected = File.ReadAllText(expectedFile);

            // Skip commented line
            actualContents = string.Join(Environment.NewLine, actualContents.Split(Environment.NewLine).Skip(1));
            expected = string.Join(Environment.NewLine, expected.Split(Environment.NewLine).Skip(1));

            Assert.That(actualContents, Is.EqualTo(expected));
        }
    }
}
