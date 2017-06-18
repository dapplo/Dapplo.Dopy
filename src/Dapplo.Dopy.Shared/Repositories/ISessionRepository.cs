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
using System.Linq.Expressions;
using Dapplo.Dopy.Shared.Entities;

namespace Dapplo.Dopy.Shared.Repositories
{
    /// <summary>
    /// Interface for the Session repository
    /// </summary>
    public interface ISessionRepository : IRepository<Session, int>
    {
        /// <summary>
        /// Returns the session specified by the predicate, or all if this is null
        /// </summary>
        /// <param name="predicate">Expression</param>
        /// <returns>IEnumerable with Session entities</returns>
        IEnumerable<Session> Find(Expression<Func<Session, bool>> predicate = null);
    }
}
