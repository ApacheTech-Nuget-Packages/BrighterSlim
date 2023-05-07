﻿using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;

namespace Paramore.Brighter.PostgreSql.EntityFrameworkCore
{
    /// <summary>
    /// A connection provider that uses the same connection as EF Core
    /// </summary>
    /// <typeparam name="T">The Db Context to take the connection from</typeparam>
    public class PostgreSqlEntityFrameworkConnectonProvider<T> : RelationalDbConnectionProvider, IAmATransactionConnectonProvider where T : DbContext
    {
        private readonly T _context;

        /// <summary>
        /// Constructs and instance from a database context
        /// </summary>
        /// <param name="context">The database context to use</param>
        public PostgreSqlEntityFrameworkConnectonProvider(T context)
        {
            _context = context;
        }

        /// <summary>
        /// Get the current connection of the database context
        /// </summary>
        /// <returns>The NpgsqlConnection that is in use</returns>
        public override DbConnection GetConnection()
        {
            return _context.Database.GetDbConnection();
        }

        /// <summary>
        /// Get the current connection of the database context
        /// </summary>
        /// <param name="cancellationToken">A cancellation token</param>
        /// <returns></returns>
        public override Task<DbConnection> GetConnectionAsync(CancellationToken cancellationToken = default)
        {
            var tcs = new TaskCompletionSource<DbConnection>();
            tcs.SetResult(_context.Database.GetDbConnection());
            return tcs.Task;
        }

        /// <summary>
        /// Get the ambient Transaction
        /// </summary>
        /// <returns>The NpgsqlTransaction</returns>
        public override DbTransaction GetTransaction()
        {
            return _context.Database.CurrentTransaction?.GetDbTransaction();
        }

        public override bool HasOpenTransaction { get => _context.Database.CurrentTransaction != null; }

        public override bool IsSharedConnection { get => true; }
    }
}
