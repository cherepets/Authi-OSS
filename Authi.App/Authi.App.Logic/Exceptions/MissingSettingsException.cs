using System;

namespace Authi.App.Logic.Exceptions
{
    public class MissingSettingsException : Exception
    {
        public MissingSettingsException() : base("Settings expected but not found") { }
    }
}
