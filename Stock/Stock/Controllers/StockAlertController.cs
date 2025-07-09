using Application.Requests.Queries;
using Application.Responses;
using Core;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Stock.Controllers
{
    [ApiController]
    [Route("api/stockalert")]
    public class StockAlertController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StockAlertController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("low-stock-products-alert")]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(IEnumerable<StockAlertResponse>))]
        public async Task<IActionResult> AlertLowStockProducts()
        {
            var result = await _mediator.SendAsync(new StockAlertQuery());
            return result.Success
                ? Ok(result.Data)
                : StatusCode(result.StatusCode, result.ErrorMessage);
        }
    }
}
