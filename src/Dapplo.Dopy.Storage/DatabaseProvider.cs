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
using System.IO;
using Dapplo.Dopy.Core.Entities;
using LiteDB;

namespace Dapplo.Dopy.Storage
{
    /// <summary>
    /// This initializes the database
    /// </summary>
    public class DatabaseProvider
    {
        private static readonly string DbDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Dapplo.Dopy");
        private static readonly string DbFilename =  Path.Combine(DbDirectory , @"dopy.db");
        private readonly BsonMapper _defaultBsonMapper;

        /// <inheritdoc />
        public DatabaseProvider()
        {
            // Make sure the DB path is available
            if (DbDirectory != null && !Directory.Exists(DbDirectory))
            {
                Directory.CreateDirectory(DbDirectory);
            }
            // Make sure the database exists
            if (!File.Exists(DbFilename))
            {
                using (var db = Create())
                {
                    // Workaround to make sure the file is stored and not deleted
                    db.Engine.Info();
                }
            }
            _defaultBsonMapper = new BsonMapper();

            _defaultBsonMapper.Entity<Clip>()
                .Id(x => x.Id) // set your document ID
                .Ignore(x => x.Contents) // ignore this property (do not store)
                .Ignore(x => x.IsModifiedByDopy) // ignore this property (do not store)
                .Ignore(x => x.OwnerIcon); // ignore this property (do not store)
            _defaultBsonMapper.Entity<Session>()
                .Id(x => x.Id);
        }

        /// <summary>
        /// Create a database, this needs to be disposed by the classer
        /// </summary>
        /// <returns>LiteDatabase</returns>
        public LiteDatabase Create()
        {
            return new LiteDatabase($@"Mode=Shared;Upgrade=true;Filename={DbFilename}", _defaultBsonMapper);
        }

        /// <summary>
        /// Create a readonly database, this needs to be disposed by the classer
        /// </summary>
        /// <returns>LiteDatabase</returns>
        public LiteDatabase CreateReadonly()
        {

            return new LiteDatabase($@"Mode=ReadOnly;Upgrade=true;Filename={DbFilename}", _defaultBsonMapper);
        }
    }
}
