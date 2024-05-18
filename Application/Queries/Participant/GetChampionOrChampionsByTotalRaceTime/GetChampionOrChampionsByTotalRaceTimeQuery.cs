using Application.Dtos;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Queries.Participant.GetChampionOrChampionsByTotalRaceTime
{
    public class GetChampionOrChampionsByTotalRaceTimeQuery : IRequest<IEnumerable<ParticipantDto>>
    {
        public required IFormFile File { get; set; }
    }
}
