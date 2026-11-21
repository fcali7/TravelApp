using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

using Models.DTO;
using Services;

namespace AppWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AttractionsController : Controller
    {
        readonly IAttractionsService _service = null;
        readonly ILogger<AttractionsController> _logger = null;

        //GET: api/attractions/read
        //Filter matches category, title, description, country or city, e.g. "museum" or "oslo"
        [HttpGet()]
        [ActionName("Read")]
        [ProducesResponseType(200, Type = typeof(ResponsePageDto<AttractionDto>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> Read(string seeded = "true", string filter = null,
            string pageNr = "0", string pageSize = "10")
        {
            try
            {
                bool seededArg = bool.Parse(seeded);
                int pageNrArg = int.Parse(pageNr);
                int pageSizeArg = int.Parse(pageSize);
                ValidateFilter(filter);

                _logger.LogInformation($"{nameof(Read)}: {nameof(seededArg)}: {seededArg}, {nameof(filter)}: {filter}, " +
                    $"{nameof(pageNrArg)}: {pageNrArg}, {nameof(pageSizeArg)}: {pageSizeArg}");

                var resp = await _service.ReadAttractionsAsync(seededArg, filter?.Trim().ToLower(), pageNrArg, pageSizeArg);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(Read)}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        //GET: api/attractions/readwithoutcomments
        [HttpGet()]
        [ActionName("ReadWithoutComments")]
        [ProducesResponseType(200, Type = typeof(ResponsePageDto<AttractionDto>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> ReadWithoutComments(string seeded = "true", string filter = null,
            string pageNr = "0", string pageSize = "10")
        {
            try
            {
                bool seededArg = bool.Parse(seeded);
                int pageNrArg = int.Parse(pageNr);
                int pageSizeArg = int.Parse(pageSize);
                ValidateFilter(filter);

                _logger.LogInformation($"{nameof(ReadWithoutComments)}: {nameof(seededArg)}: {seededArg}, {nameof(filter)}: {filter}, " +
                    $"{nameof(pageNrArg)}: {pageNrArg}, {nameof(pageSizeArg)}: {pageSizeArg}");

                var resp = await _service.ReadAttractionsWithoutCommentsAsync(seededArg, filter?.Trim().ToLower(), pageNrArg, pageSizeArg);
                return Ok(resp);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(ReadWithoutComments)}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        //GET: api/attractions/readitem
        //One attraction with category, title, description and its comments (paged)
        [HttpGet()]
        [ActionName("ReadItem")]
        [ProducesResponseType(200, Type = typeof(ResponseItemDto<AttractionDetailsDto>))]
        [ProducesResponseType(400, Type = typeof(string))]
        public async Task<IActionResult> ReadItem(string id = null, string pageNr = "0", string pageSize = "10")
        {
            try
            {
                var idArg = Guid.Parse(id);
                int pageNrArg = int.Parse(pageNr);
                int pageSizeArg = int.Parse(pageSize);

                _logger.LogInformation($"{nameof(ReadItem)}: {nameof(idArg)}: {idArg}, " +
                    $"{nameof(pageNrArg)}: {pageNrArg}, {nameof(pageSizeArg)}: {pageSizeArg}");

                var item = await _service.ReadAttractionAsync(idArg, pageNrArg, pageSizeArg);
                return Ok(item);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{nameof(ReadItem)}: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        // RegEx check to ensure filter only contains letters (including å, ä, ö, ø, æ), digits and spaces
        private static void ValidateFilter(string filter)
        {
            if (!string.IsNullOrEmpty(filter) && !Regex.IsMatch(filter, @"^[\p{L}0-9\s]*$"))
            {
                throw new ArgumentException("Filter can only contain letters, numbers (0-9), and spaces.");
            }
        }

        #region constructors
        public AttractionsController(IAttractionsService service, ILogger<AttractionsController> logger)
        {
            _service = service;
            _logger = logger;
        }
        #endregion
    }
}
