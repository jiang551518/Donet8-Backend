using Excel.IService;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace Excel.AppService
{
    public class RabbitMqService : IRabbitMqService, IAsyncDisposable
    {
        private readonly IConnection _connection;

        public RabbitMqService(IConfiguration cfg)
        {
            // 读取配置并建立持久连接
            var factory = new ConnectionFactory
            {
                HostName = cfg["RabbitMQ:Host"],
                UserName = cfg["RabbitMQ:User"],
                Password = cfg["RabbitMQ:Password"]
            };
            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();  // 建连[web:356]
        }

        public async Task<IChannel> CreateChannelAsync()
        {
            // 异步新建通道
            return await _connection.CreateChannelAsync();
        }

        public async Task PublishAsync<T>(T message, string queueName)
        {
            // 序列化并发布
            await using var channel = await CreateChannelAsync();
            await channel.QueueDeclareAsync(queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);  // 声明队列[web:342]

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);
            var props = new BasicProperties { ContentType = "application/json" };  // 属性

            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: queueName,
                mandatory: false,
                basicProperties: props,
                body: body
            );  // 发布[web:366]
        }

        public async ValueTask DisposeAsync()
        {
            // 如果连接仍打开，则先关闭，指定超时时间
            if (_connection.IsOpen)
            {
                // CloseAsync(ushort reasonCode, string reasonText, TimeSpan timeout, bool abort, ...)
                await _connection.CloseAsync(
                    reasonCode: 200,
                    reasonText: "dispose",
                    timeout: TimeSpan.FromSeconds(5),
                    abort: false
                );  // [web:391]
            }
            // 然后再释放连接对象
            await _connection.DisposeAsync();  // 释放 IAsyncDisposable 资源 [web:353]
        }
    }
}
