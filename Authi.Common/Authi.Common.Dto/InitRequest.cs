namespace Authi.Common.Dto
{
    public class InitRequest
    {
        public required byte[] ClientPublicKey { get; init; }
        public required byte[] Body { get; init; }

        public class Payload : PayloadBase
        {
        }
    }
}
