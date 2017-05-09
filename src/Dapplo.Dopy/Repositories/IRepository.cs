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
using Dapplo.Dopy.Entities;

namespace Dapplo.Dopy.Repositories
{
    /// <summary>
    /// Generic interface for a repository
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> where T : EntityBase
    {
        /// <summary>
        /// Get an element by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetById(int id);

        /// <summary>
        /// Retrieve all the entities
        /// </summary>
        /// <returns></returns>
        IEnumerable<T> List();

        /// <summary>
        /// Retrieve all the entities defined by a predicate
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<T> List(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Add a new entity
        /// </summary>
        /// <param name="entity"></param>
        void Insert(T entity);

        /// <summary>
        /// Remove an existing entity
        /// </summary>
        /// <param name="clip"></param>
        void Delete(T clip);

        /// <summary>
        /// Modify an entity
        /// </summary>
        /// <param name="clip"></param>
        void Update(T clip);
    }
}
