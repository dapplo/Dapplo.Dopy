//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016-2017 Dapplo
// 
//  For more information see: http://dapplo.net/
//  Dapplo repositories are hosted on GitHub: https://github.com/dapplo
// 
//  This file is part of Dapplo.Dopy
// 
//  Dapplo.Dopy is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  Dapplo.Dopy is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
// 
//  You should have a copy of the GNU Lesser General Public License
//  along with Dapplo.Dopy. If not, see <http://www.gnu.org/licenses/lgpl.txt>.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Dapplo.Dopy.Entities;
using Dapplo.Dopy.Repositories;
using LiteDB;
using System.Reactive.Subjects;

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
        private readonly BehaviorSubject<RepositoryUpdateArgs<Clip>> _repositoryUpdates;
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
            _repositoryUpdates = new BehaviorSubject<RepositoryUpdateArgs<Clip>>(new RepositoryUpdateArgs<Clip>(null, CrudActions.None));
        }

        /// <inheritdoc />

        public Clip GetById(int id)
        {
            return LoadContentFor(_clips.FindById(id));
        }

        /// <inheritdoc />
        public IObservable<RepositoryUpdateArgs<Clip>> Updates => _repositoryUpdates;

        /// <inheritdoc />
        public IEnumerable<Clip> Find(Expression<Func<Clip, bool>> predicate = null)
        {

            return _clips.Find(predicate ?? (clip => true) ).Select(LoadContentFor);
        }

        /// <inheritdoc />
        public void Create(Clip clip)
        {
            if (clip == null)
            {
                throw new ArgumentNullException(nameof(clip));
            }
            _clips.Insert(clip);
            foreach (var contentsKey in clip.Contents.Keys)
            {
                var stream = clip.Contents[contentsKey];
                var fileId = FileIdGenerator(clip, contentsKey);
                _liteStorage.Upload(fileId, fileId, stream);
            }
            _repositoryUpdates.OnNext(new RepositoryUpdateArgs<Clip>(clip, CrudActions.Create));
        }

        /// <inheritdoc />
        public void Delete(Clip clip)
        {
            if (clip == null)
            {
                throw new ArgumentNullException(nameof(clip));
            }
            if (clip.Id == 0)
            {
                throw new NotSupportedException("Cannot delete a clip without an ID");
            }
            _clips.Delete(clip.Id);
            foreach (var contentsKey in clip.Contents.Keys)
            {
                _liteStorage.Delete(FileIdGenerator(clip, contentsKey));
            }
            _repositoryUpdates.OnNext(new RepositoryUpdateArgs<Clip>(clip, CrudActions.Delete));
        }

        /// <inheritdoc />
        public void Update(Clip clip)
        {
            if (clip == null)
            {
                throw new ArgumentNullException(nameof(clip));
            }
            if (clip.Id == 0)
            {
                throw new NotSupportedException("Cannot update a clip without an ID");
            }
            _repositoryUpdates.OnNext(new RepositoryUpdateArgs<Clip>(clip, CrudActions.Update));

            throw new NotSupportedException("Currently not implemented");
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
            foreach (var format in clip.Formats)
            {
                if (_liteStorage.Exists(FileIdGenerator(clip, format)))
                {
                    clip.Contents[format] = LoadContent(clip, format);
                }
            }
            return clip;
        }
    }
}
