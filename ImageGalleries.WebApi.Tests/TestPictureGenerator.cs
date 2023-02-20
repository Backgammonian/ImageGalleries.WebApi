using ImageGalleries.WebApi.Services.RandomGenerators;
using Microsoft.AspNetCore.Http;

namespace ImageGalleries.WebApi.Tests
{
    public class TestPictureGenerator
    {
        private readonly RandomGenerator _randomGenerator;

        public TestPictureGenerator()
        {
            _randomGenerator = new RandomGenerator();
        }

        public IFormFile GetPicture()
        {
            var content = _randomGenerator.GetRandomString(100);
            var fileName = _randomGenerator.GetRandomString(10) + ".jpg";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(content);
            writer.Flush();
            stream.Position = 0;

            return new FormFile(stream, 0, stream.Length, _randomGenerator.GetRandomString(10), fileName);
        }
    }
}
