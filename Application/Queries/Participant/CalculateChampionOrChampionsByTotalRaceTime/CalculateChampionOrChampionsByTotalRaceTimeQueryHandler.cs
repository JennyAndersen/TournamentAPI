using Application.Dtos;
using Application.Services;
using MediatR;

namespace Application.Queries.Participant.GetChampionOrChampionsByTotalRaceTime
{
    public class CalculateChampionOrChampionsByTotalRaceTimeQueryHandler : IRequestHandler<CalculateChampionOrChampionsByTotalRaceTimeQuery, IEnumerable<ParticipantDto>>
    {
        private readonly IFileParsingService _fileParsingService;
        public CalculateChampionOrChampionsByTotalRaceTimeQueryHandler(IFileParsingService fileParsingService)
        {
            _fileParsingService = fileParsingService;
        }
        public async Task<IEnumerable<ParticipantDto>> Handle(CalculateChampionOrChampionsByTotalRaceTimeQuery request, CancellationToken cancellationToken)
        {
            var participants = await _fileParsingService.ParseParticipantsFromFileAsync(request.File);

            var qualifiedParticipants = participants
                .GroupBy(p => new { p.Id, p.Name })
                .Where(g => g.Select(p => p.RaceType).Distinct().Count() == 3)
                .Select(group => new
                {
                    group.Key.Id,
                    group.Key.Name,
                    TotalRaceTime = TimeSpan.FromTicks(group.Sum(p => (p.EndTime - p.StartTime).Ticks))
                });

            var fastestParticipants = qualifiedParticipants
                .OrderBy(p => p.TotalRaceTime)
                .ToList();

            var fastestTotalRaceTime = fastestParticipants.FirstOrDefault()?.TotalRaceTime;

            var champions = fastestParticipants
                .Where(p => p.TotalRaceTime == fastestTotalRaceTime)
                .Select(p => new ParticipantDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    AverageRaceTime = p.TotalRaceTime / 3,
                }).ToList();

            return champions;
        }
    }
}
