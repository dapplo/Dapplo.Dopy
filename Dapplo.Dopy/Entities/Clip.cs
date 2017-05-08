using System;
using System.Collections.Generic;
using System.IO;
using LiteDB;

namespace Dapplo.Dopy.Entities
{
    /// <summary>
    /// Clip is the entity for the clipboard contents in the database
    /// </summary>
    public class Clip : EntityBase
    {
        /// <summary>
        /// User for which the clip was stored
        /// </summary>
        public string Username { get; set; } = Environment.UserName;

        /// <summary>
        /// Domain of the user for which the clip was stored
        /// </summary>
        public string Domain { get; set; } = Environment.UserDomainName;

        /// <summary>
        /// Timestamp when the clip was created
        /// </summary>
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// Name of the process which created the clip, e.g. Chrome.exe
        /// </summary>
        public string ProcessName { get; set; }

        /// <summary>
        /// Name of the product which created the clip, e.g. Google Chrome
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Title of the window which created the clip
        /// </summary>
        public string WindowTitle { get; set; }

        /// <summary>
        /// Formats stored in the clip
        /// </summary>
        public IList<string> Formats { get; set; }

        /// <summary>
        /// The actual clipboard contents
        /// </summary>
        [BsonIgnore]
        public IDictionary<string, MemoryStream> Contents { get; } = new Dictionary<string, MemoryStream>();

    }
}
