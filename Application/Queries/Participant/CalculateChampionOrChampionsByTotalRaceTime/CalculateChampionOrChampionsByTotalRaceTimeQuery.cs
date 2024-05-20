using Application.Dtos;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Queries.Participant.GetChampionOrChampionsByTotalRaceTime
{
    public class CalculateChampionOrChampionsByTotalRaceTimeQuery : IRequest<IEnumerable<ParticipantDto>>
    {
        public required IFormFile File { get; set; }
    }
}
