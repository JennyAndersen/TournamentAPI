using Domain.Models;
using Microsoft.AspNetCore.Http;

namespace Application.Services
{
    public interface IFileParsingService
    {
        Task<List<Participant>> ParseParticipantsFromFileAsync(IFormFile file);
    }
}
