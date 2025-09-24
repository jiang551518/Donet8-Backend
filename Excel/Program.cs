using Excel.Options;
using Minio;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
var minioOptions = builder.Configuration
    .GetSection("MinioConn")
    .Get<MinioConnOptions>()!;

builder.Services.AddControllers();
builder.Services.AddScoped<Excel.AppService.ExcelAppService>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(s => new MinioClient()
    .WithEndpoint(minioOptions.Endpoint)
    .WithCredentials(minioOptions.AccessKey, minioOptions.SecretKey)
    .WithSSL(false)
    .Build());


builder.Services.AddMinio(opts => opts
    .WithEndpoint(minioOptions.Endpoint)
    .WithCredentials(minioOptions.AccessKey, minioOptions.SecretKey)
    .Build()
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
