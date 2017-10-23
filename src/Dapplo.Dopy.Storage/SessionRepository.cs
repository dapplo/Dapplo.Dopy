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
using System.Linq.Expressions;
using LiteDB;
using Dapplo.Dopy.Shared.Entities;
using Dapplo.Dopy.Shared.Repositories;

namespace Dapplo.Dopy.Storage
{
    /// <summary>
    /// An IClipboardRepository implementation
    /// </summary>
    [Export(typeof(ISessionRepository))]
    public class SessionRepository : ISessionRepository
    {
        private readonly LiteCollection<Session> _sessions;

        /// <summary>
        /// Constructor which sets up the database
        /// </summary>
        /// <param name="database">LiteDatabase</param>
        [ImportingConstructor]
        public SessionRepository(
            [Import("clipboard", typeof(LiteDatabase))]
            LiteDatabase database
            )
        {
            _sessions =  database.GetCollection<Session>();
	        _sessions.EnsureIndex(x => x.WindowsStartup);
	        _sessions.EnsureIndex(x => x.SessionSid);
	        _sessions.EnsureIndex(x => x.Username);
	        _sessions.EnsureIndex(x => x.Domain);
	        _sessions.EnsureIndex(x => x.Timestamp);
        }

        /// <inheritdoc />

        public Session GetById(int id)
        {
            return _sessions.FindById(id);
        }
        
        /// <inheritdoc />
        public IEnumerable<Session> Find(Expression<Func<Session, bool>> predicate = null)
        {
            return _sessions.Find(predicate ?? (session => true) );
        }

        /// <inheritdoc />
        public void Create(Session session)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }
            _sessions.Insert(session);
        }

        /// <inheritdoc />
        public void Delete(Session session)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }
            if (session.Id == 0)
            {
                throw new NotSupportedException("Cannot delete a session without an ID");
            }
            _sessions.Delete(session.Id);
        }

        /// <inheritdoc />
        public void Update(Session session)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }
            if (session.Id == 0)
            {
                throw new NotSupportedException("Cannot update a session without an ID");
            }
            _sessions.Update(session);
        }
    }
}
