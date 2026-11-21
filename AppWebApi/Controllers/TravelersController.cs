using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

using Models.DTO;
using Services;

namespace AppWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TravelersController : Controller
    {
        readonly ITravelersService _service = null;
        readonly ILogger<TravelersController> _logger = null;

        //GET: api/travelers/read
        //All travelers (users) and the comments each traveler has written
        //Filter matches first name, last name or email
        [HttpGet()]
        [ActionName("Read")]
        [ProducesResponseType(200, Type = typeof(ResponsePageDto<TravelerDto>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> Read(string seeded = "true", string filter = null,
            string pageNr = "0", string pageSize = "10")
        {
            try
            {
                bool seededArg = bool.Parse(seeded);
                int pageNrArg = int.Parse(pageNr);
                int pageSizeArg = int.Parse(pageSize);

                // RegEx check to ensure filter only contains letters, digits and spaces
                if (!string.IsNullOrEmpty(filter) && !Regex.IsMatch(filter, @"^[\p{L}0-9\s]*$"))
                {
                    throw new ArgumentException("Filter can only contain letters, numbers (0-9), and spaces.");
                }

                _logger.LogInformation($"{nameof(Read)}: {nameof(seededArg)}: {seededArg}, {nameof(filter)}: {filter}, " +
                    $"{nameof(pageNrArg)}: {pageNrArg}, {nameof(pageSizeArg)}: {pageSizeArg}");

                var resp = await _service.ReadTravelersAsync(seededArg, filter?.Trim().ToLower(), pageNrArg, pageSizeArg);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(Read)}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        #region constructors
        public TravelersController(ITravelersService service, ILogger<TravelersController> logger)
        {
            _service = service;
            _logger = logger;
        }
        #endregion
    }
}
