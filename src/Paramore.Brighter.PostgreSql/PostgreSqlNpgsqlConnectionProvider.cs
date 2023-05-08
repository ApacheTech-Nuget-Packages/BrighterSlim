﻿using System;
using System.Data.Common;
using Npgsql;

namespace Paramore.Brighter.PostgreSql
{
    /// <summary>
    /// A connection provider that uses the connection string to create a connection
    /// </summary>
    public class PostgreSqlNpgsqlConnectionProvider : RelationalDbConnectionProvider
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initialise a new instance of PostgreSQl Connection provider from a connection string
        /// </summary>
        /// <param name="configuration">PostgreSQL Configuration</param>
        public PostgreSqlNpgsqlConnectionProvider(IAmARelationalDatabaseConfiguration configuration)
        {
            if (string.IsNullOrWhiteSpace(configuration?.ConnectionString))
                throw new ArgumentNullException(nameof(configuration.ConnectionString));

            _connectionString = configuration.ConnectionString;
        }

        public override DbConnection GetConnection() =>  Connection ?? (Connection = new NpgsqlConnection(_connectionString)); 
    }
}
