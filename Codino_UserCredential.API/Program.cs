using System.Text;
using Codino_UserCredential.API.Operations;
using Codino_UserCredential.API.Operations.Interfaces;
using Codino_UserCredential.Business.Concrete;
using Codino_UserCredential.Business.Concrete.Interfaces;
using Codino_UserCredential.Core.Functions;
using Codino_UserCredential.Repository.Context;
using Codino_UserCredential.Repository.Repositories;
using Codino_UserCredential.Repository.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

startup.Configure(app, app.Environment);

app.Run();