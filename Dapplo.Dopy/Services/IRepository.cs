using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Dapplo.Dopy.Entities;

namespace Dapplo.Dopy.Services
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
