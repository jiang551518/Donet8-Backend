using Excel.AppService;
using Excel.IService;
using ExportNotificationWorker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

// 2. 注册 RabbitMQ 依赖
builder.Services.AddSingleton<IRabbitMqService, RabbitMqService>();           // 自定义发布服务[web:402]

// 3. 注册通知逻辑所需的服务
builder.Services.AddSingleton<INotificationService, NotificationService>();    // 用户通知服务

// 4. 注册你的消费者 BackgroundService
builder.Services.AddHostedService<ExportNotificationConsumer>();

var host = builder.Build();
host.Run();
