using LiteDB;

namespace Authi.App.Logic.Data
{
    public class Preference
    {
        [BsonId]
        public required string Key { get; init; }
        public required byte[] Data { get; init; }
    }
}
