using System;
using System.Runtime.Serialization;

namespace SecuruStik.DropBox
{
    /// <summary>
    /// Identifies errors in interacting with the Dropbox app (not installed, folder not found, not accessible).
    /// </summary>
    [Serializable]
    internal class DropboxInstallError : Exception
    {
        public DropboxInstallError()
        {
        }

        public DropboxInstallError(string message) : base(message)
        {
        }

        public DropboxInstallError(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DropboxInstallError(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}