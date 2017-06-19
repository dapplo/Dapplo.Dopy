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
using System.ComponentModel.Composition;
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
        /// <summary>
        /// Clipboard database
        /// </summary>
        [Export("clipboard")]
        public LiteDatabase Database { get; private set; } = new LiteDatabase($@"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\Dapplo.Dopy\dopy.db");

        /// <inheritdoc />
        public void Start()
        {
            var mapper = BsonMapper.Global;
            mapper.Entity<Clip>()
                .Id(x => x.Id) // set your document ID
                .Ignore(x => x.Contents) // ignore this property (do not store)
                .Ignore(x => x.IsModifiedByDopy) // ignore this property (do not store)
                .Ignore(x => x.OwnerIcon) // ignore this property (do not store)
                .Index(x => x.SessionId)
                .Index(x => x.SequenceNumber)
                .Index(x => x.ClipboardText)
                .Index(x => x.Timestamp)
                .Index(x => x.ProcessName)
                .Index(x => x.ProductName)
                .Index(x => x.WindowTitle)
                .Index(x => x.Formats)
                .Index(x => x.Filenames)
                .Index(x => x.OriginalFormats)
                .Index(x => x.OriginalWindowHandle);
            mapper.Entity<Session>()
                .Id(x => x.Id)
                .Index(x => x.WindowsStartup)
                .Index(x => x.SessionSid)
                .Index(x => x.Username)
                .Index(x => x.Domain)
                .Index(x => x.Timestamp);
        }

        /// <inheritdoc />
        public void Shutdown()
        {
            Database.Dispose();
        }
    }
}
