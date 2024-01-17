using System.Data;
using Dapper;

using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
namespace DotnetApi.Data
{
    public class DataContextDapper
    {
        private IConfiguration _config;
        private string? _connectionServer;
        public DataContextDapper(IConfiguration config)
        {
            _config = config;
            _connectionServer = config.GetConnectionString("DefaultConnection");

        }
        //private string _connectionServer = "Server=localhost;Database=DotNetCourseDatabase;TrustServerCertificate=true;Trusted_Connection=true;";

        public IEnumerable<T> LoadData<T>(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_connectionServer);
            return dbConnection.Query<T>(sql);

        }

        public T LoadDataSingle<T>(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_connectionServer);
            return dbConnection.QuerySingle<T>(sql);

        }
        public bool ExecuteSql(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_connectionServer);
            return (dbConnection.Execute(sql) > 0);

        }
        public int ExecuteSqlWithRowCount(string sql)
        {
            IDbConnection dbConnection = new SqlConnection(_connectionServer);
            return dbConnection.Execute(sql);

        }
        public bool ExecuteSqlWithParameters(string sql,List<SqlParameter> sqlParameters)
        {
            SqlCommand commandWithParams= new(sql);
            foreach(SqlParameter sqlParameter in sqlParameters){
                commandWithParams.Parameters.Add(sqlParameter);
            }
            SqlConnection dbConnection = new SqlConnection(_connectionServer);
            dbConnection.Open();
            commandWithParams.Connection=dbConnection;
            int rowsAffected =commandWithParams.ExecuteNonQuery();
            dbConnection.Close();
            return rowsAffected > 0;

        }
         public IEnumerable<T> LoadDataWithParameters<T>(string sql,DynamicParameters sqlParameters)
        {
            IDbConnection dbConnection = new SqlConnection(_connectionServer);
            return dbConnection.Query<T>(sql,sqlParameters);

        }

        public T LoadDataSingleWithParameters<T>(string sql,DynamicParameters sqlParameters)
        {
            IDbConnection dbConnection = new SqlConnection(_connectionServer);
            return dbConnection.QuerySingle<T>(sql,sqlParameters);

        }

    }
}