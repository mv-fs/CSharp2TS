using CSharp2TS.Core.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CSharp2TS.Tests.Stubs.Controllers {
    [TSService]
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase {
        [HttpGet]
        public ActionResult<FileContentResult> GetFile() {
            return File(Array.Empty<byte>(), "application/download");
        }

        [HttpPost]
        public ActionResult PostFile(IFormFile file) {
            return Ok();
        }

        [HttpPost]
        public ActionResult PostFiles(IFormFileCollection files) {
            return Ok();
        }

        [HttpPost]
        public ActionResult<FileContentResult> PostAndReceiveFile(IFormFile file) {
            return File(Array.Empty<byte>(), "application/download");
        }

        [HttpPost]
        public ActionResult<FileContentResult> EmptyPostAndReceiveFile() {
            return File(Array.Empty<byte>(), "application/download");
        }
    }
}
