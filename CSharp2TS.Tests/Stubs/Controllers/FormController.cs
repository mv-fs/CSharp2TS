using CSharp2TS.Core.Attributes;
using CSharp2TS.Tests.Stubs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CSharp2TS.Tests.Stubs.Controllers {
    [TSService]
    [ApiController]
    [Route("api/[controller]")]
    public class FormController : ControllerBase {
        [HttpPost]
        public ActionResult PostForm([FromForm] IFormCollection form) {
            return Ok();
        }

        [HttpPost]
        public ActionResult PostForm([FromForm] TestClass obj) {
            return Ok();
        }

        [HttpPost]
        public ActionResult PostForm([FromForm] string str) {
            return Ok();
        }
    }
}
