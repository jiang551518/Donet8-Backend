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
            await channel.QueueDeclareAsync(
                queue: "excel.export.completed",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );  // 声明队列[web:411]

            // 2. 使用 AsyncEventingBasicConsumer 并订阅 ReceivedAsync
            var consumer = new AsyncEventingBasicConsumer(channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var notification = JsonConvert.DeserializeObject<ExportNotification>(json);
                await _notificationService.NotifyUserAsync(notification);  // 处理通知[web:410]
                return;
            };

            // 3. 异步启动消费
            await channel.BasicConsumeAsync(
                queue: "excel.export.completed",
                autoAck: true,
                consumer: consumer
            );  // 启动消费[web:411]

            // 4. 阻塞直到停止令牌触发，保持后台服务执行
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

    }

}
