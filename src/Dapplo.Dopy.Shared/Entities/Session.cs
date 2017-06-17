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

namespace Dapplo.Dopy.Shared.Entities
{
    /// <summary>
    /// Session is the entity to uniquely identify a clipboard entry, even if dopy is stoped and started.
    /// In our case, a session is a windows login session for a user/domain
    /// Everytime someone logs in a new session, the clipboard sequence number starts with 0.
    /// </summary>
    public class Session : EntityBase<int>
    {
        /// <summary>
        /// Defines the time when windows was started
        /// </summary>
        public DateTimeOffset WindowsStartup { get; set; }

        /// <summary>
        /// User for which the clip was stored
        /// </summary>
        public string Username { get; set; } = Environment.UserName;

        /// <summary>
        /// Domain of the user for which the clip was stored
        /// </summary>
        public string Domain { get; set; } = Environment.UserDomainName;

        /// <summary>
        /// Timestamp when the session was first created
        /// </summary>
        public DateTimeOffset Timestamp { get; set; } = DateTimeOffset.Now;

        /// <summary>
        /// SID for the session
        /// </summary>
        public string SessionSid { get; set; }
    }
}
