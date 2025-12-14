using MySql.Data.MySqlClient;
using Org.BouncyCastle.Tls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSA_MidProject
{ 
        internal class DatabaseHelper
        {
            private String serverName = "127.0.0.1";
            private String port = "3306";
            private String databaseName = "midprojectdsa";
            private String databaseUser = "root";
            private String databasePassword = "1234567890-=1234567890-=";

            private DatabaseHelper() { }

            private static DatabaseHelper _instance;
            public static DatabaseHelper Instance
            {
                get
                {
                    if (_instance == null)
                        _instance = new DatabaseHelper();
                    return _instance;
                }
            }
            public MySqlConnection getConnection()
            {
                string connectionString = $"server={serverName};port={port};user={databaseUser};database ={databaseName}; password ={databasePassword}; SslMode = Required; ";
                var connection = new MySqlConnection(connectionString);
                connection.Open();
                return connection;
            }

            public MySqlDataReader getData(string query)
            {
                using (var connection = getConnection())
                {
                    using (var command = new MySqlCommand(query, getConnection()))
                    {
                        return command.ExecuteReader();
                    }
                }

            }

            public int Update(string query)
            {
                using (var connection = getConnection())
                {
                    using (var command = new MySqlCommand(query, getConnection()))
                    {
                        return command.ExecuteNonQuery();
                    }
                }

            }
        public object GetScalarValue(string query)
        {
            using (var connection = getConnection())
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    object result = command.ExecuteScalar();
                    return result != null && result != DBNull.Value ? result : null;
                }
            }
        }

        public int GetScalarValueAsInt(string query)
        {
            object result = GetScalarValue(query);
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public DataTable GetDataTable(string query)
            {
                DataTable dt = new DataTable();
                using (var connection = getConnection())
                {
                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var adapter = new MySqlDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                }
                return dt;
            }
        public int ExecuteNonQuery(string query, MySqlConnection existingConnection = null)
        {
            if (existingConnection != null)
            {
                using (var command = new MySqlCommand(query, existingConnection))
                {
                    return command.ExecuteNonQuery();
                }
            }
            else
            {
                return Update(query);
            }
        }

        public int Update(string query, Dictionary<string, object> parameters)
        {
            using (var connection = getConnection())
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value);
                        }
                    }
                    return command.ExecuteNonQuery();
                }
            }
        }

        public object GetScalarValue(string query, Dictionary<string, object> parameters)
        {
            using (var connection = getConnection())
            {
                using (var command = new MySqlCommand(query, connection))
                {
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue(param.Key, param.Value);
                        }
                    }
                    object result = command.ExecuteScalar();
                    return result != null && result != DBNull.Value ? result : null;
                }
            }
        }

        public MySqlDataReader getData(string query, Dictionary<string, object> parameters)
        {
            var connection = getConnection();
            var command = new MySqlCommand(query, connection);

            if (parameters != null)
            {
                foreach (var param in parameters)
                {
                    command.Parameters.AddWithValue(param.Key, param.Value);
                }
            }
            return command.ExecuteReader(CommandBehavior.CloseConnection);
        }
    }

    }


