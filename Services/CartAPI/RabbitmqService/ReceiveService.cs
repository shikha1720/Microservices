using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Newtonsoft.Json;
using CartAPI.Models;

namespace CartAPI.RabbitmqService
{
    public class ReceiveService
    {
        private string _receivedMsg;

        public async Task<string> ReceiveProduct()
        {
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq", // RabbitMQ server host
                UserName = "guest",
                Password = "guest",
                Port = 5672
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(
                queue: "product_Queue",
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, e) =>
            {
                var body = e.Body.ToArray();
                if (body != null)
                {
                    var message = Encoding.UTF8.GetString(body);
                    _receivedMsg += message;
                    HandleReceivedMessageAsync(_receivedMsg);

                }
            };

            channel.BasicConsume("product_Queue", true, consumer);
            await HandleReceivedMessageAsync(_receivedMsg);

            return _receivedMsg;

        }

        private async Task HandleReceivedMessageAsync(string message)
        {
            await AnotherAsyncMethod(message);
        }

        private async Task AnotherAsyncMethod(string message)
        {
            await Task.Delay(1000);
        }
    }
}
