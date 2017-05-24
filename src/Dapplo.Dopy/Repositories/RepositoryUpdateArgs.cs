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

namespace Dapplo.Dopy.Repositories
{
    /// <summary>
    /// This specifies which crud action was taken on an entiry
    /// </summary>
    /// <typeparam name="TEntity">Type of the Entity</typeparam>
    public class RepositoryUpdateArgs<TEntity>
    {
        /// <summary>
        /// The actual entity
        /// </summary>
        public TEntity Entity { get; }
        /// <summary>
        /// The crud action which was performed
        /// </summary>
        public CrudActions CrudAction { get; }
        /// <summary>
        /// Create a RepositoryUpdateArgs
        /// </summary>
        /// <param name="entity">Entity which was passed to the Repository</param>
        /// <param name="crudAction"></param>
        public RepositoryUpdateArgs(TEntity entity, CrudActions crudAction)
        {
            CrudAction = crudAction;
            Entity = entity;
        }
    }
}
