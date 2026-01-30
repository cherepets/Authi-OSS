using LiteDB;
using System;

namespace Authi.App.Logic.Data
{
    public class Removal
    {
        [BsonId]
        public Guid CloudId { get; set; }
    }
}
