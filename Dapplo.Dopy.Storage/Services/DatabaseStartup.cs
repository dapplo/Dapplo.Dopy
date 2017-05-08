using System.ComponentModel.Composition;
using Dapplo.Addons;
using Dapplo.Dopy.Entities;
using LiteDB;

namespace Dapplo.Dopy.Storage.Services
{
    /// <summary>
    /// This initialized the database
    /// </summary>
    [StartupAction, ShutdownAction]
    public class DatabaseStartup : IStartupAction, IShutdownAction
    {
        /// <summary>
        /// Clipboard database
        /// </summary>
        [Export("clipboard")]
        public LiteDatabase Database { get; private set; } = new LiteDatabase(@"clipboard.db");

        /// <inheritdoc />
        public void Start()
        {
            var mapper = BsonMapper.Global;
            mapper.Entity<Clip>()
                .Id(x => x.Id) // set your document ID
                .Ignore(x => x.Contents) // ignore this property (do not store)
                .Index(x => x.Username)
                .Index(x => x.Domain)
                .Index(x => x.Timestamp)
                .Index(x => x.ProcessName)
                .Index(x => x.ProductName)
                .Index(x => x.WindowTitle)
                .Index(x => x.Formats);
        }

        /// <inheritdoc />
        public void Shutdown()
        {
            Database.Dispose();
        }
    }
}
