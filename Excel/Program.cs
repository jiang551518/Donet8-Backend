using Excel.AppService;
using Excel.EfCoreDb;
using Excel.Factory;
using Excel.IService;
using Excel.Middleware_Filter;
using Excel.Options;
using Excel.Repository;
using Excel.TateFilter;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Minio;
using RabbitMQ.Client;
using Serilog;
using Serilog.Sinks.Network;
using SqlSugar;
using System.Reflection;
using System.Text;


var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
var minioOptions = builder.Configuration
    .GetSection("MinioConn")
    .Get<MinioConnOptions>()!;

var jwt = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddControllers();
builder.Services.AddScoped<Excel.AppService.ExcelAppService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMinio(opts => opts
    .WithEndpoint(minioOptions.Endpoint)
    .WithCredentials(minioOptions.AccessKey, minioOptions.SecretKey)
    .Build()
);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwt.Issuer,
            ValidateAudience = true,
            ValidAudience = jwt.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt.SecretKey)),
            ValidateLifetime = true
        };
    });

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API文档", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "直接输入JWT Token，无需加Bearer前缀",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,   // 这里改为 Http
        Scheme = "bearer",                // 全小写
        BearerFormat = "JWT"
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
            Array.Empty<string>()
        }
    });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

});

builder.Services.AddControllers(options =>
{
    options.Filters.Add<UserStateFilter>();
    options.Filters.Add<ApiResultFilter>();
    options.Filters.Add<RequestLoggingFilter>();
});

var elkaddress = builder.Configuration.GetSection("ElkAddress").Get<string>() ?? string.Empty;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.TCPSink(elkaddress)
    .CreateLogger(); //连接elk配置

builder.Host.UseSerilog(); //替换内置日志，使用elk


builder.Services.AddDbContext<MyDbContext>(options =>
    options.UseMySql(
    builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 28))  // 根据MySQL版本调整
));


builder.Services.AddScoped<ISqlSugarClient>(provider =>
    new SqlSugarClient(new SqlSugar.ConnectionConfig
    {
        ConnectionString = builder.Configuration.GetConnectionString("SqlSugarConnection"),
        DbType = SqlSugar.DbType.MySql, // MySQL 替换为 DbType.MySql
        IsAutoCloseConnection = true
    }));


builder.Services.AddScoped<ILoginAppService, LoginAppService>();
builder.Services.AddScoped<IOrmServiceFactory, OrmServiceFactory>();
builder.Services.AddScoped<LoginAppDapperIRepository>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    return new LoginAppDapperIRepository(configuration);
});

builder.Services.AddScoped<LoginAppEfCoreIRepository>();
builder.Services.AddScoped<LoginAppSqlSugarIRepository>();
builder.Services.AddSingleton<IRabbitMqService, RabbitMqService>();

//mapster
builder.Services.AddMapsterCustomize();

//rabbitmq
builder.Services.AddSingleton(sp =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var factory = new ConnectionFactory
    {
        HostName = cfg["RabbitMQ:Host"],
        UserName = cfg["RabbitMQ:User"],
        Password = cfg["RabbitMQ:Password"]
    };
    return factory.CreateConnectionAsync();
});



var app = builder.Build();
app.UseMiddleware<ApiExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
