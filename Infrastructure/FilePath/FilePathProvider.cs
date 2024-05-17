using Microsoft.Extensions.Configuration;

namespace Infrastructure.FilePath
{
    public class FilePathProvider : IFilePathProvider
    {
        private readonly IConfiguration _configuration;
        public FilePathProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetFilePath()
        {
            string directory = _configuration["FilePaths:RaceResultDirectory"];
            if (directory == null)
            {
                throw new InvalidOperationException("Participant data directory is not configured.");
            }

            return Path.Combine(directory, "race-results.txt");
        }
    }
}
