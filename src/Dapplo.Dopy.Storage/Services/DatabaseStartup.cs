//  Dapplo - building blocks for desktop applications
//  Copyright (C) 2016-2018 Dapplo
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
using System.ComponentModel.Composition;
using System.IO;
using Dapplo.Addons;
using Dapplo.Dopy.Shared.Entities;
using LiteDB;

namespace Dapplo.Dopy.Storage.Services
{
    /// <summary>
    /// This initialized the database
    /// </summary>
    [StartupAction, ShutdownAction]
    public class DatabaseStartup : IStartupAction, IShutdownAction
    {
        private static readonly string DbFilename = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Dapplo.Dopy\dopy.db"; 
        /// <summary>
        /// Clipboard database
        /// </summary>
        [Export("clipboard")]
        public LiteDatabase Database { get; private set; } = new LiteDatabase($@"Mode=Shared;Upgrade=true;Filename={DbFilename}");

        /// <inheritdoc />
        public DatabaseStartup()
        {
            // Make sure the DB path is available
            var dbDirectory = Path.GetDirectoryName(DbFilename);

            if (dbDirectory != null && !Directory.Exists(dbDirectory))
            {
                Directory.CreateDirectory(dbDirectory);
            }
        }

        /// <inheritdoc />
        public void Start()
        {
            var mapper = BsonMapper.Global;
	        mapper.Entity<Clip>()
		        .Id(x => x.Id) // set your document ID
		        .Ignore(x => x.Contents) // ignore this property (do not store)
		        .Ignore(x => x.IsModifiedByDopy) // ignore this property (do not store)
		        .Ignore(x => x.OwnerIcon); // ignore this property (do not store)
            mapper.Entity<Session>()
                .Id(x => x.Id);
        }

        /// <inheritdoc />
        public void Shutdown()
        {
            Database.Dispose();
        }
    }
}
