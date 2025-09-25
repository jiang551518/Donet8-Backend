using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

// 1. 建立连接工厂
var factory = new ConnectionFactory { HostName = "localhost" };

// 2. 异步创建连接
await using var connection = await factory.CreateConnectionAsync();      // IConnection

// 3. 异步创建通道并声明队列
await using var channel = await connection.CreateChannelAsync();         // IChannel
await channel.QueueDeclareAsync(
    queue: "hello",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null
);

// 4. 注册异步消费者
var consumer = new AsyncEventingBasicConsumer(channel);
consumer.ReceivedAsync += async (model, ea) =>
{
    var msg = Encoding.UTF8.GetString(ea.Body.ToArray());
    Console.WriteLine("Received: " + msg);
    await Task.Yield();
};

// 5. 异步启动消费
await channel.BasicConsumeAsync(
    queue: "hello",
    autoAck: true,
    consumer: consumer
);

Console.WriteLine("Waiting for messages...");
Console.ReadLine();
