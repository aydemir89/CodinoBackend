using System.Text;
using Codino_UserCredential.API.Operations;
using Codino_UserCredential.API.Operations.Interfaces;
using Codino_UserCredential.Business.Concrete;
using Codino_UserCredential.Business.Concrete.Interfaces;
using Codino_UserCredential.Core.Functions;
using Codino_UserCredential.Repository.Context;
using Codino_UserCredential.Repository.Models;
using Codino_UserCredential.Repository.Repositories;
using Codino_UserCredential.Repository.Repositories.Content;
using Codino_UserCredential.Repository.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // PostgreSQL veritabanı bağlantısı
        services.AddDbContext<CodinoDbContext>(options =>
            options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));

        services.AddHttpContextAccessor();

        // UnitOfWork için DI kaydı
        services.AddScoped<IUnitOfWork<CodinoDbContext>, UnitOfWork<CodinoDbContext>>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserLoginRequestRepository, UserLoginRequestRepository>();
        services.AddScoped<IWorldMapRepository, WorldMapRepository>();
        services.AddScoped<IBiomeRepository, BiomeRepository>();
        services.AddScoped<IToyRepository, ToyRepository>();
        services.AddScoped<ITaskRepository, TaskRepository>();
        services.AddScoped<ITaskSubmissionRepository, TaskSubmissionRepository>();

        // Business kayıtları
        services.AddScoped<IUserBusiness, UserBusiness>();
        services.AddScoped<IContentBusiness, ContentBusiness>();

        // Operations kayıtları
        services.AddScoped<IUserOperations, UserOperations>();
        services.AddScoped<IContentOperations, ContentOperations>();
        // Repository kayıtları
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserLoginRequestRepository, UserLoginRequestRepository>();

        // Business kayıtları
        services.AddScoped<IUserBusiness, UserBusiness>();

        // Operations kayıtları
        services.AddScoped<IUserOperations, UserOperations>();

        // Localization
        services.AddLocalization(options => options.ResourcesPath = "Resources");

        // JWT kimlik doğrulama
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });

        // Controller'ları ekle
        services.AddControllers();

        // Swagger - paket eklenmişse bu satırları kullanın, değilse yorum satırına alın veya kaldırın
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Codino User Credential API", Version = "v1" });
            
            // JWT için Swagger ayarları
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme (Example: 'Bearer 12345abcdef')",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });

        // LoadTest sınıfını başlat - ancak burada sadece metodu çağırmak yerine Initialize kullanılmalı
        try
        {
            // Static sınıf başlatılırken hata olmaması için try-catch içine alalım
            LoadTest.IsLoadTestMode();
        }
        catch (Exception ex)
        {
            // Hata loglanabilir
            Console.WriteLine($"LoadTest initialization error: {ex.Message}");
        }
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Development ortamı için Swagger
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Codino User Credential API v1"));
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        // HTTP request pipeline
        app.UseHttpsRedirection();
        app.UseRouting();

        // Kimlik doğrulama ve yetkilendirme
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}