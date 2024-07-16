using Microsoft.AspNetCore.Mvc;
using Portfolio.Core;

namespace Portfolio.Controllers;

[Route("api")] [ApiController] public class CommonController(ImportService _importService) : ControllerBase {
  [HttpPost("import"), DisableRequestSizeLimit] public IEnumerable<ImportResult> ImportProjection() => importService.Import(Request.Form);

  private readonly ImportService importService = _importService;
}