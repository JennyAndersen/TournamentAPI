using Application.Dtos;
using MediatR;

namespace Application.Queries.Participant.GetAllParticipants
{
    public class GetAllParticipantsQuery : IRequest<IEnumerable<ParticipantDto>>
    {
    }
}
