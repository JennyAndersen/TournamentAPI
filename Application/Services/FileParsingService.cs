using Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class FileParsingService : IFileParsingService
    {
        private readonly ILogger<FileParsingService> _logger;

        public FileParsingService(ILogger<FileParsingService> logger)
        {
            _logger = logger;
        }

        public async Task<List<Participant>> ParseParticipantsFromFileAsync(IFormFile file)
        {
            var participants = new List<Participant>();
            var idNameMapping = new Dictionary<int, string>();

            try
            {
                using (var reader = new StreamReader(file.OpenReadStream()))
                {
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        var participant = ParseParticipantFromLine(line, idNameMapping, participants);
                        if (participant != null)
                        {
                            participants.Add(participant);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while parsing the file");
                throw;
            }

            return participants;
        }

        private Participant? ParseParticipantFromLine(string line, Dictionary<int, string> idNameMapping, List<Participant> participants)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                _logger.LogWarning("Empty line found in the file");
                return null;
            }

            string[] fields = line.Split(',');
            if (fields.Length != 5)
            {
                _logger.LogWarning($"Invalid line format: {line}");
                return null;
            }

            string name = fields[0].Trim();
            if (!int.TryParse(fields[1], out int id))
            {
                _logger.LogWarning($"Invalid ID format: {fields[1]} in line: {line}");
                return null;
            }

            string normalizedFullName = name.ToUpperInvariant();
            if (!idNameMapping.ContainsKey(id))
            {
                idNameMapping[id] = normalizedFullName;
            }
            else
            {
                string existingName = idNameMapping[id];
                if (existingName != normalizedFullName)
                {
                    _logger.LogError($"ID conflict: ParticipantID {id} has different names: '{existingName}' and '{name}'");
                    return null;
                }
            }

            if (string.IsNullOrWhiteSpace(name) || !name.All(c => char.IsLetter(c) || char.IsWhiteSpace(c)))
            {
                _logger.LogWarning($"Invalid name: {name} in line: {line}");
                return null;
            }

            if (!TimeSpan.TryParse(fields[2], out TimeSpan startTime))
            {
                _logger.LogWarning($"Invalid start time format: {fields[2]} in line: {line}");
                return null;
            }

            if (!TimeSpan.TryParse(fields[3], out TimeSpan endTime))
            {
                _logger.LogWarning($"Invalid end time format: {fields[3]} in line: {line}");
                return null;
            }

            string raceType = fields[4];

            if (!new[] { "1000m", "eggRace", "sackRace" }.Contains(raceType))
            {
                _logger.LogWarning($"Invalid race type: {raceType} in line: {line}");
                return null;
            }

            if (participants.Any(p => p.Id == id && p.RaceType == raceType))
            {
                _logger.LogWarning($"Duplicate participation found for ID {id} in race type {raceType}");
            }

            return new Participant
            {
                Name = name,
                Id = id,
                StartTime = startTime,
                EndTime = endTime,
                RaceType = raceType
            };
        }
    }
}
