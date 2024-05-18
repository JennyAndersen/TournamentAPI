using Application.Queries.Participant.GetChampionOrChampionsByTotalRaceTime;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipantController : Controller
    {
        internal readonly IMediator _mediator;
        private readonly ILogger<ParticipantController> _logger;

        public ParticipantController(IMediator mediator, ILogger<ParticipantController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is empty");
            }

            try
            {
                var query = new GetChampionOrChampionsByTotalRaceTimeQuery { File = file };
                var champions = await _mediator.Send(query);

                if (champions == null || !champions.Any())
                {
                    return NotFound("No winner found or file is invalid");
                }

                return Ok(champions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the file");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the file");
            }
        }
    }
}
