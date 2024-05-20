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

        [HttpPost("calculateChampionOrChampions")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("File is empty");
            }

            try
            {
                var champions = await _mediator.Send(new CalculateChampionOrChampionsByTotalRaceTimeQuery { File = file });
                return (champions == null || !champions.Any()) ? NotFound("No winner nor winners were found or file is invalid") : Ok(champions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the file");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing the file");
            }
        }
    }
}
