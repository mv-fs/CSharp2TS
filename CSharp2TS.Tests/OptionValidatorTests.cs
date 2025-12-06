using CSharp2TS.CLI;

namespace CSharp2TS.Tests {
    class OptionValidatorTests {

        [Test]
        public void Validate_NullOptions_ReturnsErrorMessage() {
            // Arrange & Act
            var result = OptionParser.Validate(null);

            // Assert
            Assert.That(result, Is.EqualTo("Failed to parse options"));
        }

        [Test]
        public void Validate_EmptyOptions_ReturnsErrorMessage() {
            // Arrange
            var options = new Options();

            // Act
            var result = OptionParser.Validate(options);

            // Assert
            Assert.That(result, Is.EqualTo("No generation tasks specified"));
        }

        [Test]
        public void Validate_ValidModelOptions_ReturnsNull() {
            // Arrange
            var options = new Options {
                GenerateModels = true,
                ModelOutputFolder = "out",
                ModelAssemblyPaths = ["CSharp2TS.Tests.dll"],
                FileNameCasingStyle = Consts.PascalCase,
                ServiceGenerator = Consts.AxiosService
            };

            // Act
            var result = OptionParser.Validate(options);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Validate_ValidServiceOptions_ReturnsNull() {
            // Arrange
            var options = new Options {
                GenerateServices = true,
                ServicesOutputFolder = "services",
                ServicesAssemblyPaths = ["CSharp2TS.Tests.dll"],
                FileNameCasingStyle = Consts.PascalCase,
                ServiceGenerator = Consts.AxiosService
            };

            // Act
            var result = OptionParser.Validate(options);

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public void Validate_InvalidServiceGenerator_ReturnsErrorMessage() {
            // Arrange
            var options = new Options {
                GenerateServices = true,
                ServicesOutputFolder = "services",
                ServicesAssemblyPaths = ["CSharp2TS.Tests.dll"],
                ServiceGenerator = "invalid"
            };

            // Act
            var result = OptionParser.Validate(options);

            // Assert
            Assert.That(result, Contains.Substring("Invalid service generator"));
        }

        [Test]
        public void Validate_InvalidCasingStyle_ReturnsErrorMessage() {
            // Arrange
            var options = new Options {
                GenerateModels = true,
                ModelOutputFolder = "out",
                ModelAssemblyPaths = ["CSharp2TS.Tests.dll"],
                FileNameCasingStyle = "invalid"
            };

            // Act
            var result = OptionParser.Validate(options);

            // Assert
            Assert.That(result, Contains.Substring("Invalid file name casing style"));
        }

        [Test]
        public void Validate_MissingModelOutputFolder_ReturnsErrorMessage() {
            // Arrange
            var options = new Options {
                GenerateModels = true,
                ModelAssemblyPaths = ["CSharp2TS.Tests.dll"]
            };

            // Act
            var result = OptionParser.Validate(options);

            // Assert
            Assert.That(result, Is.EqualTo("Models output folder is required"));
        }

        [Test]
        public void Validate_MissingModelAssemblyPath_ReturnsErrorMessage() {
            // Arrange
            var options = new Options {
                GenerateModels = true,
                ModelOutputFolder = "out"
            };

            // Act
            var result = OptionParser.Validate(options);

            // Assert
            Assert.That(result, Is.EqualTo("At least one model assembly path is required"));
        }

        [Test]
        public void Validate_MissingServiceOutputFolder_ReturnsErrorMessage() {
            // Arrange
            var options = new Options {
                GenerateServices = true,
                ServicesAssemblyPaths = ["CSharp2TS.Tests.dll"]
            };

            // Act
            var result = OptionParser.Validate(options);

            // Assert
            Assert.That(result, Is.EqualTo("Services output folder is required"));
        }

        [Test]
        public void Validate_MissingServiceAssemblyPath_ReturnsErrorMessage() {
            // Arrange
            var options = new Options {
                GenerateServices = true,
                ServicesOutputFolder = "services"
            };

            // Act
            var result = OptionParser.Validate(options);

            // Assert
            Assert.That(result, Is.EqualTo("At least one service assembly path is required"));
        }
    }
}
