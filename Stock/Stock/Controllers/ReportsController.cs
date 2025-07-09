using Application.Requests.Queries;
using Core;
using Microsoft.AspNetCore.Mvc;

namespace Stock.Controllers
{
    [ApiController]
    [Route("api/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReportsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("monthly-report")]
        public async Task<IActionResult> GetMonthlySaleReport([FromQuery] MonthlySaleReportQuery query)
        {
            var result = await _mediator.SendAsync(query);
            return result.Success
                ? Ok(result.Data)
                : StatusCode(result.StatusCode, result.ErrorMessage);
        }
    }
}
