using System;
using System.Collections.Generic;
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DotnetApi.Data
{
    public class DataContextDapper
    {
        private readonly IConfiguration _config;
        private readonly string _connectionString;

        public DataContextDapper(IConfiguration config)
        {
            _config = config;
            _connectionString = _config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        // Load multiple records with error handling
        public IEnumerable<T> LoadData<T>(string sql)
        {
            try
            {
                using IDbConnection dbConnection = new SqlConnection(_connectionString);
                return dbConnection.Query<T>(sql);
            }
            catch (SqlException ex)
            {
                // Log or handle SQL exception
                Console.Error.WriteLine($"SQL Error: {ex.Message}");
                throw new Exception("An error occurred while loading data.", ex);
            }
            catch (Exception ex)
            {
                // Log or handle other exceptions
                Console.Error.WriteLine($"Error: {ex.Message}");
                throw new Exception("An error occurred while loading data.", ex);
            }
        }

        // Load a single record with error handling
        public T LoadDataSingle<T>(string sql)
        {
            try
            {
                using IDbConnection dbConnection = new SqlConnection(_connectionString);
                return dbConnection.QuerySingleOrDefault<T>(sql);
            }
            catch (SqlException ex)
            {
                // Log or handle SQL exception
                Console.Error.WriteLine($"SQL Error: {ex.Message}");
                throw new Exception("An error occurred while loading data.", ex);
            }
            catch (Exception ex)
            {
                // Log or handle other exceptions
                Console.Error.WriteLine($"Error: {ex.Message}");
                throw new Exception("An error occurred while loading data.", ex);
            }
        }

        // Execute a command without returning data with error handling
        public bool ExecuteSql(string sql)
        {
            try
            {
                using IDbConnection dbConnection = new SqlConnection(_connectionString);
                return dbConnection.Execute(sql) > 0;
            }
            catch (SqlException ex)
            {
                // Log or handle SQL exception
                Console.Error.WriteLine($"SQL Error: {ex.Message}");
                throw new Exception("An error occurred while executing the SQL command.", ex);
            }
            catch (Exception ex)
            {
                // Log or handle other exceptions
                Console.Error.WriteLine($"Error: {ex.Message}");
                throw new Exception("An error occurred while executing the SQL command.", ex);
            }
        }

        // Execute a command and return affected row count with error handling
        public int ExecuteSqlWithRowCount(string sql)
        {
            try
            {
                using IDbConnection dbConnection = new SqlConnection(_connectionString);
                return dbConnection.Execute(sql);
            }
            catch (SqlException ex)
            {
                // Log or handle SQL exception
                Console.Error.WriteLine($"SQL Error: {ex.Message}");
                throw new Exception("An error occurred while executing the SQL command.", ex);
            }
            catch (Exception ex)
            {
                // Log or handle other exceptions
                Console.Error.WriteLine($"Error: {ex.Message}");
                throw new Exception("An error occurred while executing the SQL command.", ex);
            }
        }

        // Execute a command with parameters and error handling
        public bool ExecuteSqlWithParameters(string sql, List<SqlParameter> sqlParameters)
        {
            try
            {
                using SqlConnection dbConnection = new(_connectionString);
                using SqlCommand commandWithParams = new(sql, dbConnection);

                foreach (SqlParameter sqlParameter in sqlParameters)
                {
                    commandWithParams.Parameters.Add(sqlParameter);
                }

                dbConnection.Open();
                int rowsAffected = commandWithParams.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (SqlException ex)
            {
                // Log or handle SQL exception
                Console.Error.WriteLine($"SQL Error: {ex.Message}");
                throw new Exception("An error occurred while executing the SQL command with parameters.", ex);
            }
            catch (Exception ex)
            {
                // Log or handle other exceptions
                Console.Error.WriteLine($"Error: {ex.Message}");
                throw new Exception("An error occurred while executing the SQL command with parameters.", ex);
            }
        }

        // Load multiple records with parameters and error handling
        public IEnumerable<T> LoadDataWithParameters<T>(string sql, DynamicParameters sqlParameters)
        {
            try
            {
                using IDbConnection dbConnection = new SqlConnection(_connectionString);
                return dbConnection.Query<T>(sql, sqlParameters);
            }
            catch (SqlException ex)
            {
                // Log or handle SQL exception
                Console.Error.WriteLine($"SQL Error: {ex.Message}");
                throw new Exception("An error occurred while loading data with parameters.", ex);
            }
            catch (Exception ex)
            {
                // Log or handle other exceptions
                Console.Error.WriteLine($"Error: {ex.Message}");
                throw new Exception("An error occurred while loading data with parameters.", ex);
            }
        }

        // Load a single record with parameters and error handling
        public T LoadDataSingleWithParameters<T>(string sql, DynamicParameters sqlParameters)
        {
            try
            {
                using IDbConnection dbConnection = new SqlConnection(_connectionString);
                return dbConnection.QuerySingleOrDefault<T>(sql, sqlParameters);
            }
            catch (SqlException ex)
            {
                // Log or handle SQL exception
                Console.Error.WriteLine($"SQL Error: {ex.Message}");
                throw new Exception("An error occurred while loading a single record with parameters.", ex);
            }
            catch (Exception ex)
            {
                // Log or handle other exceptions
                Console.Error.WriteLine($"Error: {ex.Message}");
                throw new Exception("An error occurred while loading a single record with parameters.", ex);
            }
        }
    }
}
