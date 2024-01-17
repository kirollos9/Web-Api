using System.Text;
using DotnetAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors((options) =>
{
    options.AddPolicy(
    "DevCors", (corsBuilder) =>
    {
        
            corsBuilder.WithOrigins("http://localhost:4200", "http://localhost:3000", "http://localhost:8000")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
        
   });
//    options.AddPolicy(
//     "ProdCors", (corsBuilder) =>
//     {
        
//             corsBuilder.WithOrigins("put here the link for the hostage website").AllowAnyMethod()
//             .AllowAnyHeader()
//             .AllowCredentials();
        
//    });
});

builder.Services.AddScoped<IUserRepository,UserRepository>();
/*
JWT Validation 
**/
string? tokenKeyString = builder.Configuration.GetSection("AppSettings:TokenKey").Value;
 
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters() 
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    tokenKeyString != null ? tokenKeyString : ""
                )),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseCors("Devcors");
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}
/*
these two are for Authentication jwt token
*/
/*
do not forget to put useAuthentication before Autherization
*/
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();


