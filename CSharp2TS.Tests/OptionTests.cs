using CSharp2TS.CLI;

namespace CSharp2TS.Tests {
    public class OptionTests {
        [Test]
        [TestCase("-c")]
        [TestCase("--config")]
        public void OptionParser_Args_Config(string option) {
            // Arrange
            string configPath = "/config/path/config.json";

            // Act
            var result = OptionParser.TryParseConfigFilePath([option, configPath], out string path)!;
            var noValueResult = OptionParser.TryParseConfigFilePath([option], out string _);

            // Assert
            Assert.That(result, Is.True);
            Assert.That(path, Is.EqualTo(configPath));
            Assert.That(noValueResult, Is.False);
        }

        [Test]
        public void OptionParser_NoCommands() {
            Assert.That(OptionParser.ParseFromArgs([]), Is.Null);
        }

        [Test]
        [TestCase("-mo")]
        [TestCase("--model-output-folder")]
        public void OptionParser_Args_OutputFolder(string option) {
            // Arrange
            string outputFolder = "output folder";

            // Act
            var result = OptionParser.ParseFromArgs([option, outputFolder])!;
            var noValueResult = OptionParser.ParseFromArgs([option])!;

            // Assert
            Assert.That(result.ModelOutputFolder, Is.EqualTo(outputFolder));
            Assert.That(noValueResult.ModelOutputFolder, Is.Null);
        }

        [Test]
        [TestCase("-ma")]
        [TestCase("--model-assembly-path")]
        public void OptionParser_Args_AssemblyPath(string option) {
            // Arrange
            string assemblyFile = "assembly file";

            // Act
            var result = OptionParser.ParseFromArgs([option, assemblyFile])!;
            var noValueResult = OptionParser.ParseFromArgs([option])!;

            // Assert
            Assert.That(result.ModelAssemblyPaths[0], Is.EqualTo(assemblyFile));
            Assert.That(noValueResult.ModelAssemblyPaths.Length, Is.EqualTo(0));
        }

        [Test]
        [TestCase("-so")]
        [TestCase("--services-output-folder")]
        public void OptionParser_Args_ServicesOutputFolder(string option) {
            // Arrange
            string outputFolder = "output folder";

            // Act
            var result = OptionParser.ParseFromArgs([option, outputFolder])!;
            var noValueResult = OptionParser.ParseFromArgs([option])!;

            // Assert
            Assert.That(result.ServicesOutputFolder, Is.EqualTo(outputFolder));
            Assert.That(noValueResult.ServicesOutputFolder, Is.Null);
        }

        [Test]
        [TestCase("-sa")]
        [TestCase("--services-assembly-path")]
        public void OptionParser_Args_ServicesAssemblyPath(string option) {
            // Arrange
            string assemblyFile = "assembly file";

            // Act
            var result = OptionParser.ParseFromArgs([option, assemblyFile])!;
            var noValueResult = OptionParser.ParseFromArgs([option])!;

            // Assert
            Assert.That(result.ServicesAssemblyPaths[0], Is.EqualTo(assemblyFile));
            Assert.That(noValueResult.ServicesAssemblyPaths.Length, Is.EqualTo(0));
        }

        [Test]
        [TestCase("-sg")]
        [TestCase("--service-generator")]
        public void OptionParser_Args_ServiceGenerator(string option) {
            // Arrange
            string serviceGenerator = "test";

            // Act
            var result = OptionParser.ParseFromArgs([option, serviceGenerator])!;
            var noValueResult = OptionParser.ParseFromArgs([option])!;

            // Assert
            Assert.That(result.ServiceGenerator, Is.EqualTo(serviceGenerator));
            Assert.That(noValueResult.ServiceGenerator, Is.EqualTo(Consts.AxiosService));
        }

        [Test]
        [TestCase("-fc")]
        [TestCase("--file-casing")]
        public void OptionParser_Args_FileNameCasingStyle(string option) {
            // Arrange
            string casingStyle = "camel";

            // Act
            var result = OptionParser.ParseFromArgs([option, casingStyle])!;
            var noValueResult = OptionParser.ParseFromArgs([option])!;

            // Assert
            Assert.That(result.FileNameCasingStyle, Is.EqualTo(CasingStyle.CamelCase));
            Assert.That(noValueResult.FileNameCasingStyle, Is.EqualTo(CasingStyle.PascalCase));
        }

        [Test]
        [TestCase("--nullable-strings")]
        public void OptionParser_Args_NullableStrings(string option) {
            // Act
            var result = OptionParser.ParseFromArgs([option])!;
            var noValueResult = OptionParser.ParseFromArgs([string.Empty])!;

            // Assert
            Assert.That(result.UseNullableStrings, Is.True);
            Assert.That(noValueResult.UseNullableStrings, Is.False);
        }

        [Test]
        [TestCase("-tm")]
        [TestCase("--type-mapping")]
        public void OptionParser_Args_TypeMapping(string option) {
            // Act
            var result = OptionParser.ParseFromArgs([option, "System.Uri=string"])!;
            var noValueResult = OptionParser.ParseFromArgs([string.Empty])!;

            // Assert
            Assert.That(result.CustomTypeMappings, Has.Count.EqualTo(1));
            Assert.That(result.CustomTypeMappings["System.Uri"], Is.EqualTo("string"));
            Assert.That(noValueResult.CustomTypeMappings, Is.Empty);
        }

        [Test]
        public void OptionParser_Args_MultipleTypeMappings() {
            // Act
            var result = OptionParser.ParseFromArgs([
                "--type-mapping", "System.Uri=string",
                "--type-mapping", "NodaTime.Instant=string"
            ])!;

            // Assert
            Assert.That(result.CustomTypeMappings, Has.Count.EqualTo(2));
            Assert.That(result.CustomTypeMappings["System.Uri"], Is.EqualTo("string"));
            Assert.That(result.CustomTypeMappings["NodaTime.Instant"], Is.EqualTo("string"));
        }

        [Test]
        public void OptionParser_Config_Exists() {
            // Arrange
            string fileName = "config.json";
            string fullPath = Path.GetDirectoryName(Path.GetFullPath(fileName))!;

            // Act
            var options = OptionParser.ParseFromFile(fileName);

            // Assert
            Assert.That(File.Exists(fileName), Is.True);
            Assert.That(options, Is.Not.Null);
            Assert.That(options.ModelOutputFolder, Is.EqualTo(Path.Combine(fullPath, "model-output")));
            Assert.That(options.ModelAssemblyPaths[0], Is.EqualTo(Path.Combine(fullPath, "model-assembly-1")));
            Assert.That(options.ModelAssemblyPaths[1], Is.EqualTo(Path.Combine(fullPath, "model-assembly-2")));
            Assert.That(options.ServicesOutputFolder, Is.EqualTo(Path.GetFullPath(Path.Combine(fullPath, "../service-output"))));
            Assert.That(options.ServicesAssemblyPaths[0], Is.EqualTo(Path.Combine(fullPath, "service-assembly-1")));
            Assert.That(options.ServicesAssemblyPaths[1], Is.EqualTo(Path.Combine(fullPath, "service-assembly-2")));
            Assert.That(options.FileNameCasingStyle, Is.EqualTo(CasingStyle.CamelCase));
            Assert.That(options.ServiceGenerator, Is.EqualTo("axios"));
            Assert.That(options.UseNullableStrings, Is.True);
            Assert.That(options.CustomTypeMappings, Has.Count.EqualTo(2));
            Assert.That(options.CustomTypeMappings["System.Uri"], Is.EqualTo("string"));
            Assert.That(options.CustomTypeMappings["NodaTime.Instant"], Is.EqualTo("string"));
        }

        [Test]
        public void OptionParser_Config_NotExists() {
            // Arrange
            string fileName = "missing-config.json";

            // Assert
            Assert.Throws<FileNotFoundException>(() => OptionParser.ParseFromFile(fileName));
        }
    }
}
