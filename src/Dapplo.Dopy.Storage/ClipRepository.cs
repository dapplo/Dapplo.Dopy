//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016-2019 Dapplo
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
using System.IO;
using System.Linq.Expressions;
using System.Reactive.Subjects;
using Dapplo.Dopy.Core.Entities;
using Dapplo.Dopy.Core.Extensions;
using Dapplo.Dopy.Core.Repositories;
using LiteDB;

namespace Dapplo.Dopy.Storage
{
    /// <summary>
    /// An IClipboardRepository implementation
    /// </summary>
    public class ClipRepository : IClipRepository
    {
        private readonly DatabaseProvider _databaseProvider;
        private readonly BehaviorSubject<RepositoryUpdateArgs<Clip>> _repositoryUpdates;

        /// <summary>
        /// Constructor which sets up the database
        /// </summary>
        /// <param name="databaseProvider">DatabaseProvider</param>
        public ClipRepository(DatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider;
            _repositoryUpdates = new BehaviorSubject<RepositoryUpdateArgs<Clip>>(new RepositoryUpdateArgs<Clip>(null, CrudActions.None));
        }

        /// <inheritdoc />
        public Clip GetById(int id)
        {
            using var database = _databaseProvider.CreateReadonly();
            var clipsCollection = database.GetCollection<Clip>();
            return LoadContentFor(database.FileStorage, clipsCollection.FindById(id));
        }

        /// <inheritdoc />
        public IObservable<RepositoryUpdateArgs<Clip>> Updates => _repositoryUpdates;

        /// <inheritdoc />
        public IEnumerable<Clip> Find(Expression<Func<Clip, bool>> predicate = null)
        {
            using var database = _databaseProvider.CreateReadonly();
            var clipsCollection = database.GetCollection<Clip>();
            foreach (var foundClip in predicate != null ? clipsCollection.Find(predicate) : clipsCollection.FindAll())
            {
                yield return LoadContentFor(database.FileStorage, foundClip);
            }
        }

        /// <inheritdoc />
        public void Create(Clip clip)
        {
            if (clip == null)
            {
                throw new ArgumentNullException(nameof(clip));
            }
            using (var database = _databaseProvider.Create())
            {
                var clipsCollection = database.GetCollection<Clip>();
                clipsCollection.Insert(clip);

                var iconFileId = IconFileIdGenerator(clip);

                var liteStorage = database.FileStorage;
                using (var memoryStream = clip.OwnerIconAsStream())
                {
                    if (memoryStream != null)
                    {
                        memoryStream.Seek(0, SeekOrigin.Begin);
                        liteStorage.Upload(iconFileId, $"{iconFileId}.png", memoryStream);
                    }
                }

                foreach (var contentsKey in clip.Contents.Keys)
                {
                    var stream = clip.Contents[contentsKey];
                    var fileId = FileIdGenerator(clip, contentsKey);
                    liteStorage.Upload(fileId, fileId, stream);
                }
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

            using (var database = _databaseProvider.Create())
            {
                var clipsCollection = database.GetCollection<Clip>();
                clipsCollection.Delete(clip.Id);

                var liteStorage = database.FileStorage;

                // Remove icon
                var iconFileId = IconFileIdGenerator(clip);
                if (liteStorage.Exists(iconFileId))
                {
                    liteStorage.Delete(iconFileId);
                }

                foreach (var contentsKey in clip.Contents.Keys)
                {
                    liteStorage.Delete(FileIdGenerator(clip, contentsKey));
                }
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

            using (var database = _databaseProvider.Create())
            {
                var clipsCollection = database.GetCollection<Clip>();

                clipsCollection.Update(clip);

                // TODO: Optimize!

                var liteStorage = database.FileStorage;

                // Remove icon
                var iconFileId = IconFileIdGenerator(clip);
                if (liteStorage.Exists(iconFileId))
                {
                    liteStorage.Delete(iconFileId);
                }

                // Write it again
                using (var memoryStream = clip.OwnerIconAsStream())
                {
                    if (memoryStream != null)
                    {
                        liteStorage.Upload(iconFileId, $"{iconFileId}.png", memoryStream);
                    }
                }

                // Remove all contents
                foreach (var contentsKey in clip.Contents.Keys)
                {
                    liteStorage.Delete(FileIdGenerator(clip, contentsKey));
                }

                // add them again
                foreach (var contentsKey in clip.Contents.Keys)
                {
                    var stream = clip.Contents[contentsKey];
                    var fileId = FileIdGenerator(clip, contentsKey);
                    liteStorage.Upload(fileId, fileId, stream);
                }
            }

            _repositoryUpdates.OnNext(new RepositoryUpdateArgs<Clip>(clip, CrudActions.Update));
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
        /// Returns the File-ID for the icon of a clip
        /// </summary>
        /// <param name="clip">Clip</param>
        /// <returns>string</returns>
        private string IconFileIdGenerator(Clip clip)
        {
            return $"$/icons/{clip.Id}";
        }

        /// <summary>
        /// Load the content to a Clip
        /// </summary>
        /// <param name="fileStorage">LiteStorage</param>
        /// <param name="clip">Clip</param>
        /// <param name="format">string with clipboard format</param>
        /// <returns>MemoryStream</returns>
        private MemoryStream LoadContent(ILiteStorage<string> fileStorage, Clip clip, string format)
        {
            var stream = new MemoryStream();
            var fileId = FileIdGenerator(clip, format);
            if (fileStorage.Exists(fileId))
            {
                fileStorage.Download(fileId, stream);
            }
            return stream;
        }

        /// <summary>
        /// Load the content to a Clip
        /// </summary>
        /// <param name="fileStorage">LiteStorage</param>
        /// <param name="clip">Clip</param>
        /// <returns>Clip</returns>
        private Clip LoadContentFor(ILiteStorage<string> fileStorage, Clip clip)
        {
            var iconFileId = IconFileIdGenerator(clip);
            if (fileStorage.Exists(iconFileId))
            {
                using var memoryStream = new MemoryStream();
                fileStorage.Download(iconFileId, memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);
                clip.OwnerIconFromStream(memoryStream);
            }

            foreach (var format in clip.Formats)
            {
                if (fileStorage.Exists(FileIdGenerator(clip, format)))
                {
                    clip.Contents[format] = LoadContent(fileStorage, clip, format);
                }
            }
            return clip;
        }
    }
}
