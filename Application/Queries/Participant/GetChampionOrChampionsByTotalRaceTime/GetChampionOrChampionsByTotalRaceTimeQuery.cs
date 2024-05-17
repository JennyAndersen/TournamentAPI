using Application.Dtos;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Queries.Participant.GetChampionOrChampionsByTotalRaceTime
{
    public class GetChampionOrChampionsByTotalRaceTimeQuery : IRequest<IEnumerable<ParticipantDto>>
    {
    }
}
