using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Configuration;
using System.Text;

namespace Sixeyed.MessagingPoweredFrontEnd.Core.Messaging
{
    /// <summary>
    /// Client wrapper for RabbitMQ access
    /// </summary>
    /// <remarks>
    /// Run Rabbit using Docker:
    ///  docker run -d --hostname rabbit-mq --name rabbit-mq -p 15672:15672 -p 5672:5672 rabbitmq:3-management
    /// </remarks>
    public class Queue : IDisposable
    {
        private IModel _model;
        private string _lastMessageReplyQueueName;

        public string ReplyQueueName { get; private set; }        

        public Queue(bool createReplyQueue = false)
        {
            var host = ConfigurationManager.AppSettings["rabbitmq.host"];
            var factory = new ConnectionFactory();
            factory.Uri = "amqp://guest:guest@" + host + ":5672/";

            var connection = factory.CreateConnection();
            _model = connection.CreateModel();

            //set up known queues & topics:
            _model.ExchangeDeclare("noticeboard", ExchangeType.Fanout);
            _model.QueueDeclare("noticeboard-broadcast", false, false, false, null);
            _model.QueueBind("noticeboard-broadcast", "noticeboard", "");
            _model.QueueDeclare("noticeboard-persist", false, false, false, null);
            _model.QueueBind("noticeboard-persist", "noticeboard", "");

            if (createReplyQueue)
            {
                var result = _model.QueueDeclare("reply-" + Environment.MachineName, false, false, true, null);
                ReplyQueueName = result.QueueName;
            }
        }        
        
        public void Publish<TMessage>(TMessage message)
        {
            BasicPublish<TMessage>(message, "noticeboard", "");
        }

        public void Reply<TMessage>(TMessage message)
        {
            BasicPublish<TMessage>(message, "", _lastMessageReplyQueueName);
        }

        private void BasicPublish<TMessage>(TMessage message, string exchangeName, string queueName)
        {
            var properties = _model.CreateBasicProperties();
            properties.Type = typeof(TMessage).Name;
            if (!string.IsNullOrEmpty(ReplyQueueName))
            {
                properties.ReplyTo = ReplyQueueName;
            }

            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);

            _model.BasicPublish(exchangeName, queueName, properties, body);
        }

        public void Listen<TMessage>(string queueName, Action<TMessage> action)
        {
            var consumer = new EventingBasicConsumer(_model);
            consumer.Received += (ch, ea) =>
            {
                _lastMessageReplyQueueName = ea.BasicProperties.ReplyTo;
                if (ea.BasicProperties.Type == typeof(TMessage).Name)
                {
                    var body = ea.Body;
                    var json = Encoding.UTF8.GetString(body);
                    var message = JsonConvert.DeserializeObject<TMessage>(json);
                    action(message);
                    _model.BasicAck(ea.DeliveryTag, false);
                }
                else
                {
                    _model.BasicNack(ea.DeliveryTag, false, true);
                }
            };
            _model.BasicConsume(queueName, false, consumer);
        }
        
        public void Dispose()
        {
            if (_model != null)
            {
                Dispose(true);
            }
        }

        protected void Dispose(bool disposing)
        {
            _model.Close();
            _model.Dispose();
            _model = null;
        }
    }
}
