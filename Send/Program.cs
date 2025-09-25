using RabbitMQ.Client;
using System;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        await using var connection = await factory.CreateConnectionAsync();          // 建连[web:356]
        await using var channel = await connection.CreateChannelAsync();          // 建通道[web:356]
        await channel.QueueDeclareAsync("hello", false, false, false, null);        // 声明队列[web:356]

        Console.WriteLine("输入要发送的消息，空行结束：");
        while (true)
        {
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
                break;

            var body = Encoding.UTF8.GetBytes(input);                              // 用户输入转字节
            var props = new BasicProperties();                                     // 消息属性
            props.ContentType = "text/plain";
            await channel.BasicPublishAsync(
                exchange: "",
                routingKey: "hello",
                mandatory: false,
                basicProperties: props,
                body: body
            );                                                                      // 发布消息[web:366]
            Console.WriteLine($"已发送: {input}");
        }
    }
}
