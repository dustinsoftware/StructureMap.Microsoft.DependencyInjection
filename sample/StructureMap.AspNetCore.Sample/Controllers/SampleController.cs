using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace StructureMap.AspNetCore.Sample.Controllers
{
    [Route("")]
    [ApiController]
    public class SampleController : ControllerBase
    {
        private readonly ISampleService _sampleService;

        public SampleController(ISampleService sampleService)
        {
            _sampleService = sampleService;
        }

        [HttpGet("")]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { _sampleService.GetMessage() };
        }
    }
}
