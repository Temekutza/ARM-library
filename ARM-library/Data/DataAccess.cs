using System;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

namespace ARM_library.Data
{
    /// <summary>
    /// Доступ к MySQL через MySql.Data.
    /// </summary>
    public sealed class DataAccess
    {
        private readonly string _connectionString;

        public DataAccess(string connectionStringName = "LibraryDb")
        {
            var cs = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (cs == null)
                throw new ConfigurationErrorsException($"Не найдена строка подключения \"{connectionStringName}\" в App.config.");

            _connectionString = cs.ConnectionString;
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }

        private MySqlConnection CreateOpenConnection()
        {
            var conn = new MySqlConnection(_connectionString);
            conn.Open();
            return conn;
        }

        public int ExecuteNonQuery(string sql, Action<MySqlCommand> bind = null)
        {
            using (var conn = CreateOpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                bind?.Invoke(cmd);
                return cmd.ExecuteNonQuery();
            }
        }

        public object ExecuteScalar(string sql, Action<MySqlCommand> bind = null)
        {
            using (var conn = CreateOpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                bind?.Invoke(cmd);
                return cmd.ExecuteScalar();
            }
        }

        public DataTable ExecuteTable(string sql, Action<MySqlCommand> bind = null)
        {
            using (var conn = CreateOpenConnection())
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.CommandType = CommandType.Text;
                bind?.Invoke(cmd);

                using (var adapter = new MySqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    adapter.Fill(dt);
                    return dt;
                }
            }
        }
    }
}


