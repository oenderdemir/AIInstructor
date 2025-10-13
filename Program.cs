using Couchbase;
using Couchbase.Core.Retry;
using Couchbase.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Enrichers.CallerInfo;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using AIInstructor.src.Context;

using AIInstructor.src.Shared.CurrentUser.Service;
using AIInstructor.src.Shared.Extensions;
using AIInstructor.src.Shared.Filters;
using AIInstructor.src.Shared.Middleware;

using AIInstructor.src.Shared.SignalRHubs;


Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.WithCallerInfo(
        includeFileInfo: true,  // Whether to include file path and line number
        assemblyPrefix: "AIInstructor", // Only include calls from assemblies starting with "VTS"
        filePathDepth: 3       // Number of path segments to include from file path
    )
    .WriteTo.Console(outputTemplate:
        "{Timestamp:dd.MM.yyyy HH:mm:ss.fff zzz} [{Level:u3}] " +
        "[{SourceFile}.{Method} {LineNumber}:{ColumnNumber}] " +
        "{Message:lj}{NewLine}{Exception}"
    )
    .WriteTo.File(
        path: "Logs/vts-app-.log",
        rollingInterval: RollingInterval.Day,
        outputTemplate:
        "{Timestamp:dd.MM.yyyy HH:mm:ss.fff zzz} [{Level:u3}] " +
           "[{SourceFile}.{Method} {LineNumber}:{ColumnNumber}] " +
        "{Message:lj}{NewLine}{Exception}"
    )
    .CreateLogger();
var builder = WebApplication.CreateBuilder(args);


var configuration = builder.Configuration;


if (configuration.GetValue<string>("Logging:LogLevel:Default") == "Debug")
{
    builder.Host.UseSerilog();
}





var allowedOrigins = builder.Configuration
    .GetSection("App:Cors:AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    foreach (var item in allowedOrigins)
    {
        Console.WriteLine("item-->" + item);
    }


    options.AddPolicy("CorsPolicy", policy =>
    {
        policy
            .WithOrigins(allowedOrigins) // '*' YOK; tek tek origin
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
             .WithExposedHeaders("WWW-Authenticate");
    });

});

// Add services to the container.
builder.Services.AddHttpContextAccessor();

builder.Services.AddServicesAndRepositoriesFromAssembly(Assembly.GetExecutingAssembly());
//builder.Services.AddScoped<ICurrentUserService,CurrentUserService>();


builder.Services.AddSignalR();
builder.Services.AddMemoryCache();





builder.Services.AddControllers();

builder.Services.AddDbContext<VTSDbContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("VTSDbConnection"));

    if (configuration.GetValue<string>("Logging:LogLevel:Default") == "Debug")
    {
        options.EnableSensitiveDataLogging()
               .LogTo(Console.WriteLine, LogLevel.Debug);
    }
});



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "VTSApi", Version = "v1" });
    opt.OperationFilter<FileUploadOperationFilter>();
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});


builder.Services.AddAuthentication(options =>
{

    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("UIScheme", o =>
{

    o.RequireHttpsMetadata = false;
    var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
    o.SaveToken = true;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
})
.AddJwtBearer("ServiceScheme", o =>
{

    o.RequireHttpsMetadata = false;
    var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
    o.SaveToken = true;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UIPolicy", policy =>
        policy.RequireAuthenticatedUser().RequireClaim("permission", "KullaniciTipi.Admin", "KullaniciTipi.UIUser").AddAuthenticationSchemes("UIScheme"));

    options.AddPolicy("ServicePolicy", policy =>
        policy.RequireAuthenticatedUser().RequireClaim("permission", "KullaniciTipi.Admin", "KullaniciTipi.ServiceUser").AddAuthenticationSchemes("ServiceScheme"));


});


builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());




var app = builder.Build();

//using (var scope = app.Services.CreateScope())
//{
//    var sp = scope.ServiceProvider;

//    // Cluster ve bucket hazır mı?
//    var clusterProvider = sp.GetRequiredService<IClusterProvider>();
//    var cluster = await clusterProvider.GetClusterAsync();
//    await cluster.WaitUntilReadyAsync(TimeSpan.FromSeconds(30));

//    var namedBucketProvider = sp.GetRequiredService<INamedBucketProvider>();
//    var bucket = await namedBucketProvider.GetBucketAsync();
//    await bucket.WaitUntilReadyAsync(TimeSpan.FromSeconds(30));
//}

app.UseCors("CorsPolicy");
app.UseRouting();
await app.Services.SeedAsync();

// Configure the HTTP request pipeline.



app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "VTS API V1");
    c.RoutePrefix = "swagger"; // swagger url'si => /swagger
});


app.UseMiddleware<JwtTokenLoggingMiddleware>();
app.UseMiddleware<RequestResponseLoggingMiddleware>();

app.MapControllers();
app.MapHub<SystemHub>("/ui/hubs/system");


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {

        var context = services.GetRequiredService<VTSDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {

        Log.Error(ex.ToString());
        // Loglama veya hata yönetimi
    }
}
Log.Information("AIInstructor started");

app.Lifetime.ApplicationStopped.Register(async () =>
{
    try
    {
        var couchbaseLifetime = app.Services.GetService<ICouchbaseLifetimeService>();
        if (couchbaseLifetime != null)
        {
            await couchbaseLifetime.CloseAsync().ConfigureAwait(false);
        }

        var cluster = app.Services.GetService<ICluster>();
        if (cluster != null)
        {
            await cluster.DisposeAsync().ConfigureAwait(false);
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error during Couchbase shutdown");
    }
});

app.Run();
