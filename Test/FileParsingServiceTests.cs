using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Text;

namespace Test
{
    public class FileParsingServiceTests
    {
        private Mock<ILogger<FileParsingService>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<FileParsingService>>();
        }

        private static IFormFile CreateMockFile(string fileName, string content)
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);
            var formFile = new Mock<IFormFile>();
            formFile.Setup(f => f.FileName).Returns(fileName);
            formFile.Setup(f => f.Length).Returns(stream.Length);
            formFile.Setup(f => f.OpenReadStream()).Returns(stream);
            formFile.Setup(f => f.ContentDisposition).Returns($"attachment; filename={fileName}");

            return formFile.Object;
        }

        [Test]
        public async Task When_ParseParticipantsFromFileAsync_Then_Should_Return_Participants()
        {
            // Arrange
            var fileName = "testfile.txt";
            var fileContent = "Steve Carell,3100693,14:07:10,14:14:05,1000m\n" +
                              "Steve Carell,3100693,12:16:11,12:19:08,eggRace\n" +
                              "Steve Carell,3100693,12:12:10,12:16:05,sackRace";
            var file = CreateMockFile(fileName, fileContent);
            var fileParsingService = new FileParsingService(_loggerMock.Object);

            // Act
            var participants = await fileParsingService.ParseParticipantsFromFileAsync(file);

            // Assert
            Assert.That(participants.Count, Is.EqualTo(3));
            Assert.IsTrue(participants.All(p => p.Name == "Steve Carell"));
            Assert.IsTrue(participants.All(p => p.Id == 3100693));
            Assert.IsTrue(participants.Any(p => p.RaceType == "1000m"));
            Assert.IsTrue(participants.Any(p => p.RaceType == "eggRace"));
            Assert.IsTrue(participants.Any(p => p.RaceType == "sackRace"));
        }

        [Test]
        public void When_ParseParticipantsFromFileAsync_Then_Should_Throw_Error_On_Duplicate_Entries()
        {
            // Arrange
            var fileName = "testfile_duplicate.txt";
            var fileContent = "Steve Carell,3100693,14:07:10,14:14:05,1000m\n" +
                              "Steve Carell,3100693,14:07:10,14:14:05,1000m";
            var file = CreateMockFile(fileName, fileContent);
            var fileParsingService = new FileParsingService(_loggerMock.Object);

            // Act & Assert
            var exception = Assert.ThrowsAsync<FormatException>(async () => await fileParsingService.ParseParticipantsFromFileAsync(file));
            Assert.That(exception.Message, Is.EqualTo("Duplicate race entry for participant."));
        }

        [Test]
        public async Task When_ParseParticipantsFromFileAsync_Then_Should_Log_Error_On_Invalid_Line()
        {
            // Arrange
            var fileName = "testfile_invalid.txt";
            var fileContent = "Invalid Line Content\n" +
                              "Steve Carell,3100693,12:16:11,12:19:08,eggRace\n";
            var file = CreateMockFile(fileName, fileContent);
            var fileParsingService = new FileParsingService(_loggerMock.Object);

            // Act
            var participants = await fileParsingService.ParseParticipantsFromFileAsync(file);

            // Assert
            Assert.That(participants.Count, Is.EqualTo(1));
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Invalid line format")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task When_ParseParticipantsFromFileAsync_Then_Should_Log_Error_On_Invalid_Race_Type()
        {
            // Arrange
            var fileName = "testfile_invalid_race_type.txt";
            var fileContent = "Steve Carell,3100693,14:07:10,14:14:05,1000m\n" +
                              "Steve Carell,3100693,12:16:11,12:19:08,eggRace\n" +
                              "Steve Carell,3100693,12:12:10,12:16:05,invalidRaceType";
            var file = CreateMockFile(fileName, fileContent);
            var fileParsingService = new FileParsingService(_loggerMock.Object);

            // Act
            var participants = await fileParsingService.ParseParticipantsFromFileAsync(file);

            // Assert
            Assert.That(participants.Count, Is.EqualTo(2));
            _loggerMock.Verify(
                x => x.Log(LogLevel.Warning,
                           It.IsAny<EventId>(),
                           It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Invalid race type")),
                           It.IsAny<Exception>(),
                           It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}