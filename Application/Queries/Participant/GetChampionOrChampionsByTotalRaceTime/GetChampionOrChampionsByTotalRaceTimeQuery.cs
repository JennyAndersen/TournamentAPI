using Application.Dtos;
using MediatR;

namespace Application.Queries.Participant.GetChampionOrChampionsByTotalRaceTime
{
    public class GetChampionOrChampionsByTotalRaceTimeQuery : IRequest<IEnumerable<ParticipantDto>>
    {
    }
}
