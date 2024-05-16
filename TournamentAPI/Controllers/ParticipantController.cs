using Application.Queries.Participant.GetAllParticipants;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipantController : Controller
    {
        internal readonly IMediator _mediator;

        public ParticipantController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("getAllParticipants")]
        public async Task<IActionResult> GetAllParticipants()
        {
            try
            {
                var allParticipants = await _mediator.Send(new GetAllParticipantsQuery());
                return allParticipants == null ? NotFound("No Participants found") : Ok(allParticipants);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
