using System.Security.Cryptography;
using System.Text;

namespace ImageGalleries.WebApi.Services.RandomGenerators
{
    public class RandomGenerator : IRandomGenerator
    {
        public static string Chars { get; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz";
        public static int IdLength { get; } = 40;

        public int Next(int toExclusive)
        {
            return RandomNumberGenerator.GetInt32(toExclusive);
        }

        public int Next(int fromInclusive, int toExclusive)
        {
            return RandomNumberGenerator.GetInt32(fromInclusive, toExclusive);
        }

        public int GetRandomInt()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(sizeof(int));

            return BitConverter.ToInt32(randomBytes, 0);
        }

        public uint GetRandomUInt()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(sizeof(uint));

            return BitConverter.ToUInt32(randomBytes, 0);
        }

        public long GetRandomLong()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(sizeof(long));

            return BitConverter.ToInt64(randomBytes, 0);
        }

        public ulong GetRandomULong()
        {
            var randomBytes = RandomNumberGenerator.GetBytes(sizeof(ulong));

            return BitConverter.ToUInt64(randomBytes, 0);
        }

        public string GetRandomString(int length)
        {
            var result = new StringBuilder();
            for (var j = 0; j < length; j++)
            {
                var randomNumber = RandomNumberGenerator.GetInt32(0, Chars.Length - 1);
                result.Append(Chars[randomNumber]);
            }

            return result.ToString();
        }

        public string GetRandomId() => GetRandomString(IdLength);
    }
}
