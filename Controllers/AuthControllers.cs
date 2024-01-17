using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Dapper;
using DotnetApi.Data;
using DotnetApi.Dtos;
using DotnetApi.Models;
using DotnetAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly IConfiguration _config;
        private readonly AuthHelper _authHelper;

        public AuthController(IConfiguration config)
        {

            _dapper = new DataContextDapper(config);
            _config = config;
            _authHelper = new(config);
        }
        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserForRegistrationDto userForRegistration)
        {
            if (userForRegistration.Password == userForRegistration.PasswordConfirm)
            {
                string sqlCheckUserExists = @$"
                SELECT Email FROM TutorialAppSchema.Auth
                WHERE Email='{userForRegistration.Email}'";
                IEnumerable<string> existingUsers = _dapper.LoadData<string>(sqlCheckUserExists);
                if (existingUsers.Count() == 0)
                {
                    UserForLoginDto userForSetPassword = new()
                    {
                        Email = userForRegistration.Email,
                        Password = userForRegistration.Password
                    };
                    if (_authHelper.SetPassword(userForSetPassword))
                    {
                        Console.WriteLine("here is done");
                        string sqlAddUser = @"EXEC TutorialAppSchema.spUser_Upsert
                            @FirstName = '" + userForRegistration.FirstName +
                            "', @LastName = '" + userForRegistration.LastName +
                            "',@Email = '" + userForRegistration.Email +
                            "', @Gender = '" + userForRegistration.Gender +
                            "', @Active = 1" +
                            ", @JobTitle = '" + userForRegistration.JobTitle +
                            "', @Department = '" + userForRegistration.Department +
                            "', @Salary = '" + userForRegistration.Salary + "'";


                        if (_dapper.ExecuteSql(sqlAddUser))
                        {
                            return Ok();
                        }
                        throw new Exception("Failed to Add User!");

                    }


                    throw new Exception("Failed to Register User!");


                }
                throw new Exception("user with this email  is already exists!");

            }
            throw new Exception("password is not match!");

        }
        [HttpPut("ResetPassword")]
        public IActionResult ResetPassword(UserForLoginDto userForSetPassword)
        {
            if (_authHelper.SetPassword(userForSetPassword))
            {
                return Ok();
            }
            throw new Exception("Failed to reset Password!");

        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto userForLogin)
        {
            string sqlForHashAndSalt = @$"EXEC TutorialAppSchema.spLoginConfirmation_Get
                @Email=@EmailParam";
                DynamicParameters sqlParameters=new();
                sqlParameters.Add("@EmailParam",userForLogin.Email,DbType.String);
                // List<SqlParameter> sqlParameters=new();
                // SqlParameter emailParam=new("@EmailParam",SqlDbType.NVarChar){
                //     Value=userForLogin.Email
                // };
                // sqlParameters.Add(emailParam);
            UserForLoginConfirmationDto userForLoginConfirmation = _dapper
                .LoadDataSingleWithParameters<UserForLoginConfirmationDto>(sqlForHashAndSalt,sqlParameters);
            byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForLoginConfirmation.PasswordSalt);
            //now we want to compare passwordHash and userForLoginConfirmation.passwordhash
            // it cannot be done because they are object
            //we should make for loog to compare the bye

            for (int i = 0; i < passwordHash.Length; i++)
            {
                if (passwordHash[i] != userForLoginConfirmation.PasswordHash[i])
                {
                    return StatusCode(401, "Incorrect Password!");
                }
            }
            int userId;
            string userIdSql = @$"
                SELECT UserId FROM TutorialAppSchema.Users
                WHERE Email= '{userForLogin.Email}'
             ";
            userId = _dapper.LoadDataSingle<int>(userIdSql);

            return Ok(new Dictionary<string, string>{
                {"token",_authHelper.CreateToken(userId)}
            });
        }
        [HttpGet("RefreshToken")]
        public string RefreshToken()
        {
            string userIdSql = @$"
                SELECT UserId FROM TutorialAppSchema.Users
                WHERE UserId= " + "'" + User.FindFirst("userId")?.Value + "'";
            int userId = _dapper.LoadDataSingle<int>(userIdSql);

            return _authHelper.CreateToken(userId);
        }

    }
}