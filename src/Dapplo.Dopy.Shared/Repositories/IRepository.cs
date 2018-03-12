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

namespace Dapplo.Dopy.Shared.Repositories
{
    /// <summary>
    /// Generic interface for a repository
    /// </summary>
    /// <typeparam name="TEntity">Type for the entity</typeparam>
    /// <typeparam name="TKey">Type for the key</typeparam>
    public interface IRepository<TEntity, in TKey> where TEntity : class
    {
        /// <summary>
        /// Get an element by ID
        /// </summary>
        /// <param name="id">TKey with the ID</param>
        /// <returns>TEntity</returns>
        TEntity GetById(TKey id);
        /// <summary>
        /// Creates a TEntity in the repository
        /// </summary>
        /// <param name="entity">TEntity</param>
        void Create(TEntity entity);
        /// <summary>
        /// Updates a TEntity in the repository
        /// </summary>
        /// <param name="entity">TEntity</param>
        void Update(TEntity entity);
        /// <summary>
        /// Deletes a TEntity from the repository
        /// </summary>
        /// <param name="entity"></param>
        void Delete(TEntity entity);
    }
}
