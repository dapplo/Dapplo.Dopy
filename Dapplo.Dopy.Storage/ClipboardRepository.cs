using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq.Expressions;
using Dapplo.Dopy.Entities;
using Dapplo.Dopy.Services;
using LiteDB;

namespace Dapplo.Dopy.Storage
{
    /// <summary>
    /// An IClipboardRepository implementation
    /// </summary>
    [Export(typeof(IClipRepository))]
    public class ClipboardRepository : IClipRepository
    {
        private readonly LiteCollection<Clip> _clips;
        private readonly LiteStorage _liteStorage;
        /// <summary>
        /// Constructor which sets up the database
        /// </summary>
        /// <param name="database">LiteDatabase</param>
        [ImportingConstructor]
        public ClipboardRepository(
            [Import("clipboard", typeof(LiteDatabase))]
            LiteDatabase database
            )
        {
            _clips =  database.GetCollection<Clip>();
            _liteStorage = database.FileStorage;

        }

        /// <summary>
        /// This returns the clip by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Clip GetById(int id)
        {
            return _clips.FindById(id);
        }

        /// <summary>
        /// Returns all Clip object from the database
        /// </summary>
        /// <returns>IEnumerable with Clip entities</returns>
        public IEnumerable<Clip> List()
        {
            return _clips.FindAll();
        }

        /// <summary>
        /// Returns the clips specified by the predicate
        /// </summary>
        /// <param name="predicate">Expression</param>
        /// <returns>IEnumerable with Clip entities</returns>
        public IEnumerable<Clip> List(Expression<Func<Clip, bool>> predicate)
        {
            return _clips.Find(predicate);
        }

        /// <summary>
        /// Returns the File-ID of a clip
        /// </summary>
        /// <param name="clip">Clip</param>
        /// <param name="format">Format of the clip</param>
        /// <returns>string</returns>
        private string FileIdGenerator(Clip clip, string format)
        {
            string idFormat = format.Replace(" ", "_");
            return $"$/contents/{clip.Id}-{idFormat}";
        }
        /// <summary>
        /// Insert a new clip into the repository
        /// </summary>
        /// <param name="clip">Clip</param>
        public void Insert(Clip clip)
        {
            _clips.Insert(clip);
            foreach (var contentsKey in clip.Contents.Keys)
            {
                var stream = clip.Contents[contentsKey];
                var fileId = FileIdGenerator(clip, contentsKey);
                _liteStorage.Upload(fileId, fileId, stream);
            }
        }

        /// <summary>
        /// Delete a clip from the repository
        /// </summary>
        /// <param name="clip">Clip</param>
        public void Delete(Clip clip)
        {
            _clips.Delete(clip.Id);
            foreach (var contentsKey in clip.Contents.Keys)
            {
                _liteStorage.Delete(FileIdGenerator(clip, contentsKey));
            }
        }

        /// <summary>
        /// Update the specified clip
        /// </summary>
        /// <param name="clip">Clip</param>
        public void Update(Clip clip)
        {
            throw new NotSupportedException("Currently not implemented");
        }
    }
}
