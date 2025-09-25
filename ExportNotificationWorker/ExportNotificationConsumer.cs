using Excel.AppService;
using Excel.IService;
using Excel.VM;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExportNotificationWorker
{
    public class ExportNotificationConsumer : BackgroundService
    {
        private readonly INotificationService _notificationService;
        private readonly IRabbitMqService _rabbitMqService;
        public ExportNotificationConsumer(IRabbitMqService rabbitMqService, INotificationService notificationService)
        {
            _notificationService = notificationService;
            _rabbitMqService = rabbitMqService;

        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // 1. 创建通道并声明队列
            var channel = await _rabbitMqService.CreateChannelAsync();
            // 1. 声明两个队列
            await channel.QueueDeclareAsync("excel.export.completed", false, false, false, null);
            await channel.QueueDeclareAsync("login.login", false, false, false, null);

            // 2. 创建一个 AsyncEventingBasicConsumer 并订阅同一回调
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (sender, ea) =>
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                if (ea.RoutingKey == "excel.export.completed")
                {
                    // Excel 导出通知
                    var note = JsonConvert.DeserializeObject<Notification>(json);
                    await _notificationService.NotifyUserAsync(note);
                }
                else if (ea.RoutingKey == "login.login")
                {
                    // 登录通知（可定义新 DTO）
                    var loginNote = JsonConvert.DeserializeObject<Notification>(json);
                    await _notificationService.NotifyUserAsync(loginNote);
                }
                return;
            };

            // 3. 分别启动对两个队列的消费
            await channel.BasicConsumeAsync("excel.export.completed", autoAck: true, consumer: consumer);
            await channel.BasicConsumeAsync("login.login", autoAck: true, consumer: consumer);

            // 4. 阻塞保持运行
            await Task.Delay(Timeout.Infinite, stoppingToken);

        }

    }

}
