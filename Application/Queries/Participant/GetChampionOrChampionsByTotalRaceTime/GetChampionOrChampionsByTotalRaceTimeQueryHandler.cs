using Application.Dtos;
using Infrastructure.FilePath;
using MediatR;

namespace Application.Queries.Participant.GetChampionOrChampionsByTotalRaceTime
{
    public class GetChampionOrChampionsByTotalRaceTimeQueryHandler : IRequestHandler<GetChampionOrChampionsByTotalRaceTimeQuery, IEnumerable<ParticipantDto>>
    {
        private readonly IFilePathProvider _filePathProvider;
        public GetChampionOrChampionsByTotalRaceTimeQueryHandler(IFilePathProvider filePathProvider)
        {
            _filePathProvider = filePathProvider;
        }
        public async Task<IEnumerable<ParticipantDto>> Handle(GetChampionOrChampionsByTotalRaceTimeQuery request, CancellationToken cancellationToken)
        {
            string filePath = _filePathProvider.GetFilePath();

            List<Domain.Models.Participant> participants = [];

            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    while ((line = await sr.ReadLineAsync())
                        != null)
                    {
                        string[] fields = line.Split(',');

                        if (fields.Length != 5)
                        {
                            // TODO Logging
                            continue;
                        }

                        string name = fields[0];
                        int id;
                        if (!int.TryParse(fields[1], out id))
                        {
                            // TODO Logging
                            continue;
                        }

                        TimeSpan startTime;
                        if (!TimeSpan.TryParse(fields[2], out startTime))
                        {
                            // TODO Logging
                            continue;
                        }

                        TimeSpan endTime;
                        if (!TimeSpan.TryParse(fields[3], out endTime))
                        {
                            // TODO Logging
                            continue;
                        }

                        string raceType = fields[4];

                        participants.Add(new Domain.Models.Participant
                        {
                            Name = name,
                            Id = id,
                            StartTime = startTime,
                            EndTime = endTime,
                            RaceType = raceType
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                // TODO Logging
                throw;
            }

            var qualifiedParticipants = participants
                .GroupBy(p => new { p.Id, p.Name})
                .Where(g => g.Select(p => p.RaceType).Distinct().Count() == 3)
                .Select(group => new
                {
                    Id = group.Key.Id,
                    Name = group.Key.Name,
                    TotalRaceTime = TimeSpan.FromTicks(group.Sum(p => (p.EndTime - p.StartTime).Ticks))
                });

            var fastestParticipants = qualifiedParticipants
                .OrderBy(participants => participants.TotalRaceTime)
                .ToList();

            var fastestTotalRaceTime = fastestParticipants.FirstOrDefault()?.TotalRaceTime; 

            var champions = fastestParticipants
                .Where(p => p.TotalRaceTime == fastestTotalRaceTime)
                .Select(p => new ParticipantDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    TotalRaceTime = p.TotalRaceTime,
                }).ToList();
            return champions; 
        }
    }
}
