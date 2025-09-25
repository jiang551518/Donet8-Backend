using RabbitMQ.Client;

namespace Excel.IService
{
    public interface IRabbitMqService
    {
        /// <summary>异步创建并返回 IChannel</summary>
        Task<IChannel> CreateChannelAsync();

        /// <summary>将对象序列化为 JSON 并推送到指定队列</summary>
        Task PublishAsync<T>(T message, string queueName);
    }

}
