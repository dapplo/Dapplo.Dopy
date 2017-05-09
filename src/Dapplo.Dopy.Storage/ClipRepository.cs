﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
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
    public class ClipRepository : IClipRepository
    {
        private readonly LiteCollection<Clip> _clips;
        private readonly LiteStorage _liteStorage;
        /// <summary>
        /// Constructor which sets up the database
        /// </summary>
        /// <param name="database">LiteDatabase</param>
        [ImportingConstructor]
        public ClipRepository(
            [Import("clipboard", typeof(LiteDatabase))]
            LiteDatabase database
            )
        {
            _clips =  database.GetCollection<Clip>();
            _liteStorage = database.FileStorage;
        }

        /// <summary>
        /// Load the content to a Clip
        /// </summary>
        /// <param name="clip">Clip</param>
        /// <param name="format">string with clipboard format</param>
        /// <returns>MemoryStream</returns>
        private MemoryStream LoadContent(Clip clip, string format)
        {
            var stream = new MemoryStream();
            var fileId = FileIdGenerator(clip, format);
            _liteStorage.Download(fileId, stream);
            return stream;
        }

        /// <summary>
        /// Load the content to a Clip
        /// </summary>
        /// <param name="clip">Clip</param>
        /// <returns>Clip</returns>
        private Clip LoadContentFor(Clip clip)
        {
            foreach (var format in clip.Contents.Keys)
            {
                clip.Contents[format] = LoadContent(clip, format);
            }
            return clip;
        }
        /// <summary>
        /// This returns the clip by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Clip GetById(int id)
        {
            var clip = _clips.FindById(id);
            LoadContentFor(clip);
            return clip;
        }

        /// <summary>
        /// Returns all Clip object from the database
        /// </summary>
        /// <returns>IEnumerable with Clip entities</returns>
        public IEnumerable<Clip> List()
        {
            return _clips.FindAll().Select(LoadContentFor);
        }

        /// <summary>
        /// Returns the clips specified by the predicate
        /// </summary>
        /// <param name="predicate">Expression</param>
        /// <returns>IEnumerable with Clip entities</returns>
        public IEnumerable<Clip> List(Expression<Func<Clip, bool>> predicate)
        {
            return _clips.Find(predicate).Select(LoadContentFor);
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
