using CSharp2TS.CLI.Utility;

namespace CSharp2TS.Tests.Utility {
    public class GeneratorUtilityTests {
        [Test]
        [TestCase(null, null)]
        [TestCase("", null)]
        [TestCase("   ", null)]
        public void GetCleanRouteConstraints_NullOrWhiteSpace_ReturnsOriginal(string? template, string? expected) {
            // Act
            string? result = RouteUtility.GetCleanRouteConstraints(template);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("{id}", "${id}")]
        [TestCase("{name}", "${name}")]
        [TestCase("api/{controller}", "api/${controller}")]
        public void GetCleanRouteConstraints_NoConstraints_ReturnsUnchanged(string template, string expected) {
            // Act
            string? result = RouteUtility.GetCleanRouteConstraints(template);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("{id:int}", "${id}")]
        [TestCase("{name:string}", "${name}")]
        [TestCase("{price:decimal}", "${price}")]
        [TestCase("{active:bool}", "${active}")]
        [TestCase("{date:datetime}", "${date}")]
        public void GetCleanRouteConstraints_SimpleConstraints_RemovesConstraints(string template, string expected) {
            // Act
            string? result = RouteUtility.GetCleanRouteConstraints(template);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("{id:min(1)}", "${id}")]
        [TestCase("{id:max(100)}", "${id}")]
        [TestCase("{id:range(1,100)}", "${id}")]
        [TestCase("{name:minlength(3)}", "${name}")]
        [TestCase("{name:maxlength(50)}", "${name}")]
        [TestCase("{name:length(5,10)}", "${name}")]
        public void GetCleanRouteConstraints_ParameterizedConstraints_RemovesConstraints(string template, string expected) {
            // Act
            string? result = RouteUtility.GetCleanRouteConstraints(template);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("{slug:regex(^[a-z]+$)}", "${slug}")]
        [TestCase("{code:regex([[a-zA-Z]]{{2,3}})}", "${code}")]
        public void GetCleanRouteConstraints_RegexConstraints_RemovesConstraints(string template, string expected) {
            // Act
            string? result = RouteUtility.GetCleanRouteConstraints(template);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("api/{controller}/{id:int}", "api/${controller}/${id}")]
        [TestCase("{category:string}/{id:int}/{action}", "${category}/${id}/${action}")]
        [TestCase("products/{id:int}/reviews/{reviewId:int}", "products/${id}/reviews/${reviewId}")]
        public void GetCleanRouteConstraints_MultipleParameters_RemovesAllConstraints(string template, string expected) {
            // Act
            string? result = RouteUtility.GetCleanRouteConstraints(template);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("api/products/{id:int:min(1)}", "api/products/${id}")]
        [TestCase("{name:string:minlength(2):maxlength(50)}", "${name}")]
        public void GetCleanRouteConstraints_MultipleConstraintsOnSameParameter_RemovesAllConstraints(string template, string expected) {
            // Act
            string? result = RouteUtility.GetCleanRouteConstraints(template);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("api/values", "api/values")]
        [TestCase("static/content/file.txt", "static/content/file.txt")]
        public void GetCleanRouteConstraints_NoParameters_ReturnsUnchanged(string template, string expected) {
            // Act
            string? result = RouteUtility.GetCleanRouteConstraints(template);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        [TestCase("api/{controller=Home}/{action=Index}/{id:int?}", "api/${controller}/${action}/${id}")]
        public void GetCleanRouteConstraints_OptionalParametersWithDefaults(string template, string expected) {
            // Act
            string? result = RouteUtility.GetCleanRouteConstraints(template);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}
