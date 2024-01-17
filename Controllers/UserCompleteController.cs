using System.Data;
using Dapper;
using DotnetApi.Data;
using DotnetApi.Dtos;
using DotnetApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserCompleteController : ControllerBase
{
    DataContextDapper _dapper;
    public UserCompleteController(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }

    [HttpGet("TestConnection")]
    public DateTime TestConnection()
    {
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    }

    [HttpGet("GetUsers/{userId}/{isActive}")]
    // public IEnumerable<User> GetUsers()
    public IEnumerable<UserComplete> GetUsers(int userId, bool isActive)
    {
        string sql = @"EXEC TutorialAppSchema.spUsers_Get";
        DynamicParameters sqlParameters=new();

        if (userId != 0)
        {
            sql += " @UserId =@UserIdParam " ;
            sqlParameters.Add("@UserIdParam",userId,DbType.Int32);
        }
        if (isActive && userId<=0)
        {
            sql += " @Active = @ActiveParam" ;
            sqlParameters.Add("@ActiveParam",isActive,DbType.Boolean);
        }
        if (isActive && userId>0)
        {
            sql += " , @Active = @ActiveParam" ;
            sqlParameters.Add("@ActiveParam",isActive,DbType.Boolean);
        }
        IEnumerable<UserComplete> users = _dapper.LoadDataWithParameters<UserComplete>(sql,sqlParameters);
        return users;
    }



    [HttpPut("UpsertUser")]
    public IActionResult UpsertUser(UserComplete user)
    {

        string sql = @"EXEC TutorialAppSchema.spUser_Upsert
            @FirstName = '" + user.FirstName +
            "', @LastName = '" + user.LastName +
            "',@Email = '" + user.Email +
            "', @Gender = '" + user.Gender +
            "', @Active = '" + user.Active +
            "', @JobTitle = '" + user.JobTitle +
            "', @Department = '" + user.Department +
            "', @Salary = '" + user.Salary +
            "' , @UserId = " + user.UserId;

        Console.WriteLine(sql);

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to Update User");
    }


    
    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUser(int userId)
    {
        string sql = @"TutorialAppSchema.Users @UserId = " + userId.ToString();

        Console.WriteLine(sql);

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to Delete User");
    }

  
}