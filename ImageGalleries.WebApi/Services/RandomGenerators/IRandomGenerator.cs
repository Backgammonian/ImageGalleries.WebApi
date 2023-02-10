namespace ImageGalleries.WebApi.Services.RandomGenerators
{
    public interface IRandomGenerator
    {
        int Next(int toExclusive);
        int Next(int fromInclusive, int toExclusive);
        int GetRandomInt();
        uint GetRandomUInt();
        long GetRandomLong();
        ulong GetRandomULong();
        string GetRandomString(int length);
        string GetRandomId();
    }
}
