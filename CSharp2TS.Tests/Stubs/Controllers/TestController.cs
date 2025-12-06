using CSharp2TS.Core.Attributes;
using CSharp2TS.Tests.Stubs.Models;
using Microsoft.AspNetCore.Mvc;

namespace CSharp2TS.Tests.Stubs.Controllers {
    [TSService]
    [ApiController]
    [Route("api/Test")]
    public class TestController : ControllerBase {
        public TestController() {
        }

        [HttpGet]
        public string Get() {
            throw new NotImplementedException();
        }

        [HttpGet]
        public ActionResult<string> ActionResultGet() {
            throw new NotImplementedException();
        }

        [HttpGet]
        public async Task<ActionResult<string>> ActionResultGetAsync() {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }

        [HttpGet]
        [TSEndpoint(typeof(string))]
        public async Task<IActionResult> TSEndpointOverride() {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }

        [HttpGet("{id}")]
        public ActionResult<TestClass> Get(int id) {
            throw new NotImplementedException();
        }

        [HttpGet("{id}")]
        public ActionResult<TestClass> Get(int id, int externalId) {
            throw new NotImplementedException();
        }

        [HttpGet]
        public ActionResult<GenericClass1<TestClass2>> Generic() {
            throw new NotImplementedException();
        }

        [HttpGet("filtered")]
        public ActionResult<IEnumerable<TestClass>> GetFiltered([FromQuery] string filter, [FromQuery] int limit = 10) {
            throw new NotImplementedException();
        }

        [HttpPost]
        public ActionResult<TestClass> Create([FromBody] TestClass testClass) {
            throw new NotImplementedException();
        }

        [HttpPost]
        public ActionResult<string> CreateFromBody([FromBody] string model) {
            throw new NotImplementedException();
        }

        [HttpPut("{id}")]
        public ActionResult<TestClass> Update(int id, TestClass testClass) {
            throw new NotImplementedException();
        }

        [HttpPatch("{id}")]
        public ActionResult<TestClass> PartialUpdate(int id, TestClass model) {
            throw new NotImplementedException();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(int id) {
            throw new NotImplementedException();
        }

        [HttpGet("{id:int}")]
        public ActionResult GetWithTypedParam(int id) {
            throw new NotImplementedException();
        }
    }
}
