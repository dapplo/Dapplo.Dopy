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
using System.Linq.Expressions;
using Dapplo.Dopy.Core.Entities;
using Dapplo.Dopy.Core.Repositories;

namespace Dapplo.Dopy.Storage
{
    /// <summary>
    /// An ISessionRepository implementation
    /// </summary>
    public class SessionRepository : ISessionRepository
    {
        private readonly DatabaseProvider _databaseProvider;

        /// <summary>
        /// Constructor which sets up the database
        /// </summary>
        /// <param name="databaseProvider">DatabaseProvider</param>
        public SessionRepository(DatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider;
        }

        /// <inheritdoc />
        public Session GetById(int id)
        {
            using (var database = _databaseProvider.CreateReadonly())
            {
                var sessions = database.GetCollection<Session>();
                return sessions.FindById(id);
            }
        }
        
        /// <inheritdoc />
        public IEnumerable<Session> Find(Expression<Func<Session, bool>> predicate = null)
        {
            using (var database = _databaseProvider.CreateReadonly())
            {
                var sessions = database.GetCollection<Session>();
/*                sessions.EnsureIndex(x => x.WindowsStartup);
                sessions.EnsureIndex(x => x.SessionSid);
                sessions.EnsureIndex(x => x.Username);
                sessions.EnsureIndex(x => x.Domain);
                sessions.EnsureIndex(x => x.Timestamp);*/
                foreach (var foundSession in sessions.Find(predicate ?? (session => true)))
                {
                    yield return foundSession;
                }
            }
        }

        /// <inheritdoc />
        public void Create(Session session)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            using (var database = _databaseProvider.Create())
            {
                var sessions = database.GetCollection<Session>();
                sessions.Insert(session);
            }
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
            using (var database = _databaseProvider.Create())
            {
                var sessions = database.GetCollection<Session>();
                sessions.Delete(session.Id);
            }
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
            using (var database = _databaseProvider.Create())
            {
                var sessions = database.GetCollection<Session>();
                sessions.Update(session);
            }
        }
    }
}
