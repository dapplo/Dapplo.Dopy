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
            var clips = Database.GetCollection<Clip>("clips");
            clips.EnsureIndex(clip => clip.Username);
            clips.EnsureIndex(clip => clip.Domain);
            clips.EnsureIndex(clip => clip.Timestamp);
            clips.EnsureIndex(clip => clip.ProcessName);
            clips.EnsureIndex(clip => clip.ProductName);
            clips.EnsureIndex(clip => clip.WindowTitle);
            clips.EnsureIndex(clip => clip.Formats);
        }

        /// <inheritdoc />
        public void Shutdown()
        {
            Database.Dispose();
        }
    }
}
