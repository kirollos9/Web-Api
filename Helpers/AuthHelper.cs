using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DotnetApi.Data;
using DotnetApi.Dtos;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Helpers{
    public class AuthHelper{
        private readonly IConfiguration _config;
        private readonly DataContextDapper _dapper;
        public AuthHelper(IConfiguration config){
            _config=config;
            _dapper=new(config);
        }
        public byte[] GetPasswordHash(string password, byte[] passwordSalt)
        {
            string passwordSaltPlusString = _config.GetSection("AppSettings:PasswordKey").Value
                    + Convert.ToBase64String(passwordSalt);
            byte[] passwordHash = KeyDerivation.Pbkdf2(
                password: password,
                salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 1000000,
                numBytesRequested: 256 / 8
            );
            return passwordHash;

        }
        public string CreateToken(int userId)
        {
            Claim[] claims = new Claim[]{
                new Claim("userId",userId.ToString())
            };
            string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;
            
            SymmetricSecurityKey tokenKey = new(
                    Encoding.UTF8.GetBytes(
                        tokenKeyString ?? ""
                    )
                );
            SigningCredentials credentials = new(tokenKey,
                SecurityAlgorithms.HmacSha512Signature);
            SecurityTokenDescriptor descriptor = new()
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = credentials,
                Expires = DateTime.Now.AddDays(1)
            };
            JwtSecurityTokenHandler tokenHandler = new();
            SecurityToken token = tokenHandler.CreateToken(descriptor);
            return tokenHandler.WriteToken(token);
        }
        public bool SetPassword(UserForLoginDto userForSetPassword){
             byte[] passwordSalt = new byte[128 / 8];
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        rng.GetNonZeroBytes(passwordSalt);
                    };
                    byte[] passwordHash = GetPasswordHash(userForSetPassword.Password, passwordSalt);

                    string sqlAddAuth = @" EXEC dbo.spRegistration_Upsert 
                            @Email = @EmailParam,
                            @PasswordHash = @PasswordHashParam,
                            @PasswordSalt =@PasswordSaltParam";
                    List<SqlParameter> sqlParameters = new List<SqlParameter>();
                    SqlParameter emailParameter = new SqlParameter("@EmailParam", SqlDbType.NVarChar)
                    {
                        Value = userForSetPassword.Email
                    };
                    sqlParameters.Add(emailParameter);

                    SqlParameter passwordHashParameter = new SqlParameter("@PasswordHashParam", SqlDbType.VarBinary)
                    {
                        Value = passwordHash
                    };

                    sqlParameters.Add(passwordHashParameter);

                    SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSaltParam", SqlDbType.VarBinary)
                    {
                        Value = passwordSalt
                    };
                    sqlParameters.Add(passwordSaltParameter);
                    return _dapper.ExecuteSqlWithParameters(sqlAddAuth, sqlParameters);
        } 
    }
}