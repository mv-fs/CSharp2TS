using CSharp2TS.Core.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace CSharp2TS.Tests.Stubs.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    [TSService]
    [TSImport("CustomType", "../types/customType")]
    public class ImportController : ControllerBase {
        [HttpGet]
        [TSEndpoint("CustomType")]
        public async Task<IActionResult> Get() {
            await Task.CompletedTask;
            throw new NotImplementedException();
        }
    }
}
