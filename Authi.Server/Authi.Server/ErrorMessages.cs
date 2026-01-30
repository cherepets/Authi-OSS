namespace Authi.Server
{
    public static class ErrorMessages
    {
        public const string CantCreateClient = "Can't create client.";
        public const string CantCreateData = "Can't create data.";
        public const string CantCreateSync = "Can't create sync.";

        public const string CantFindClient = "Can't find client.";
        public const string CantFindData = "Can't find data.";
        public const string CantFindSync = "Can't find sync.";

        public const string CantDecryptPayload = "Can't decrypt payload.";
        public const string CantParseClientPublicKey = "Can't parse client public key.";
        public const string CantReadDataVersion = "Can't read data version.";
        public const string CantParsePayload = "Can't parse payload.";
        public const string CantVerifyClock = "Can't verify clock.";

        public const string DataExceedsLimit = "Data exceeds limit.";

        public const string SyncExpired = "Sync session has expired.";
    }
}
