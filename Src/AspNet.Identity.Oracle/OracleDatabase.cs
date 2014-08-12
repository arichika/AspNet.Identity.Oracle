using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Threading;
using Oracle.DataAccess.Client;

namespace AspNet.Identity.Oracle
{
     /// <summary>
     /// Class that encapsulates a Oracle Database connections 
     /// and CRUD operations
     /// </summary>
    public class OracleDatabase : IDisposable
    {
        private OracleConnection connection;

        /// <summary>
        /// Default constructor which uses the "DefaultConnection" connectionString
        /// </summary>
        public OracleDatabase()
            : this("DefaultConnection")
        {
        }

        /// <summary>
        /// Constructor which takes the connection string name
        /// </summary>
        /// <param name="connectionStringName"></param>
        public OracleDatabase(string connectionStringName)
        {
            var connectionString = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
            connection = new OracleConnection(connectionString);
        }

        /// <summary>
        /// Executes a non-query Oracle statement
        /// </summary>
        /// <param name="commandText">The Oracle query to execute</param>
        /// <param name="parameters">Optional parameters to pass to the query</param>
        /// <returns>The count of records affected by the Oracle statement</returns>
        public int Execute(string commandText, IEnumerable parameters)
        {
            int result;

            if (String.IsNullOrEmpty(commandText))
            {
                throw new ArgumentException("Command text cannot be null or empty.");
            }

            try
            {
                ensureConnectionOpen();
                var command = createCommand(commandText, parameters);
                result = command.ExecuteNonQuery();
            }
            finally
            {
                connection.Close();
            }

            return result;
        }

        /// <summary>
        /// Executes a Oracle query that returns a single scalar value as the result.
        /// </summary>
        /// <param name="commandText">The Oracle query to execute</param>
        /// <param name="parameters">Optional parameters to pass to the query</param>
        /// <returns></returns>
        public object QueryValue(string commandText, IEnumerable parameters)
        {
            object result;

            if (String.IsNullOrEmpty(commandText))
            {
                throw new ArgumentException("Command text cannot be null or empty.");
            }

            try
            {
                ensureConnectionOpen();
                var command = createCommand(commandText, parameters);
                result = command.ExecuteScalar();
            }
            finally
            {
                ensureConnectionClosed();
            }

            return result;
        }

        /// <summary>
        /// Executes a SQL query that returns a list of rows as the result.
        /// </summary>
        /// <param name="commandText">The Oracle query to execute</param>
        /// <param name="parameters">Parameters to pass to the Oracle query</param>
        /// <returns>A list of a Dictionary of Key, values pairs representing the 
        /// ColumnName and corresponding value</returns>
        public List<Dictionary<string, string>> Query(string commandText, IEnumerable parameters)
        {
            List<Dictionary<string, string>> rows;
            if (String.IsNullOrEmpty(commandText))
            {
                throw new ArgumentException("Command text cannot be null or empty.");
            }

            try
            {
                ensureConnectionOpen();
                var command = createCommand(commandText, parameters);
                using (var reader = command.ExecuteReader())
                {
                    rows = new List<Dictionary<string, string>>();
                    while (reader.Read())
                    {
                        var row = new Dictionary<string, string>();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            var columnName = reader.GetName(i);
                            var columnValue = reader.IsDBNull(i) ? null : reader.GetValue(i).ToString();
                            row.Add(columnName, columnValue);
                        }
                        rows.Add(row);
                    }
                }
            }
            finally
            {
                ensureConnectionClosed();
            }

            return rows;
        }

        /// <summary>
        /// Opens a connection if not open
        /// </summary>
        private void ensureConnectionOpen()
        {
            var retries = 3;
            if (connection.State == ConnectionState.Open)
            {
                return;
            }
            while (retries >= 0 && connection.State != ConnectionState.Open)
            {
                connection.Open();
                retries--;
                Thread.Sleep(30);
            }
        }

        /// <summary>
        /// Closes a connection if open
        /// </summary>
        private void ensureConnectionClosed()
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        /// <summary>
        /// Creates a OracleCommand with the given parameters
        /// </summary>
        /// <param name="commandText">The Oracle query to execute</param>
        /// <param name="parameters">Parameters to pass to the Oracle query</param>
        /// <returns></returns>
        private OracleCommand createCommand(string commandText, IEnumerable parameters)
        {
            var command = connection.CreateCommand();
            command.CommandText = commandText;
            addParameters(command, parameters);

            return command;
        }

        /// <summary>
        /// Adds the parameters to a Oracle command
        /// </summary>
        /// <param name="command">The Oracle query to execute</param>
        /// <param name="parameters">Parameters to pass to the Oracle query</param>
        private static void addParameters(OracleCommand command, IEnumerable parameters)
        {
            if (parameters == null) return;

            foreach (var parameter in parameters)
            {
                command.Parameters.Add(parameter);
            }
        }

        /// <summary>
        /// Helper method to return query a string value 
        /// </summary>
        /// <param name="commandText">The Oracle query to execute</param>
        /// <param name="parameters">Parameters to pass to the Oracle query</param>
        /// <returns>The string value resulting from the query</returns>
        public string GetStrValue(string commandText, IEnumerable parameters)
        {
            var value = QueryValue(commandText, parameters) as string;
            return value;
        }

        public void Dispose()
        {
            if (connection == null) return;

            connection.Dispose();
            connection = null;
        }
    }
}
