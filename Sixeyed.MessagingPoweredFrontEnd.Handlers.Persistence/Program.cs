using HBase.Stargate.Client.Api;
using Sixeyed.MessagingPoweredFrontEnd.Contracts;
using Sixeyed.MessagingPoweredFrontEnd.Core.Messaging;
using Sixeyed.MessagingPoweredFrontEnd.Handlers.Persistence.Data;
using System;
using System.Configuration;

namespace Sixeyed.MessagingPoweredFrontEnd.Handlers.Persistence
{
    /// <summary>
    /// Message handler for persisting mail to HBase
    /// </summary>
    /// <remarks>
    /// Run HBase using Docker:
    ///  docker run -d -p 8080:8080 sixeyed/hbase-stargate
    /// </remarks>
    class Program
    {
        private static Queue _Queue = new Queue();
        private static MailTable _MailTable;

        static void Main(string[] args)
        {
            var client = StargateFactory.GetClient();
            client.BootstrapSchema();
            _MailTable = new MailTable(client);

            _Queue.Listen<SendMailRequest>("noticeboard-persist", x => SaveMail(x));

            Console.WriteLine("Listening for messages on: {0}, using Stargate at: {1}", ConfigurationManager.AppSettings["rabbitmq.host"], ConfigurationManager.AppSettings["hbase.cluster.url"]);
            Console.ReadLine();
        }

        private static void SaveMail(SendMailRequest request)
        {            
            Console.WriteLine("Received mail from: {0}, sent at: {1}", request.Sender, request.SentAt);

            try
            {
                _MailTable.PutNew(request.Sender, request.Content, request.SentAt);

                _Queue.Reply<MailSavedEvent>(new MailSavedEvent
                {
                    Sender = request.Sender,
                    SentAt = request.SentAt,
                    SavedAt = DateTime.Now
                });
            }
            catch
            {
                _Queue.Reply<HandlerFailedEvent>(new HandlerFailedEvent
                {
                    Sender = request.Sender,
                    SentAt = request.SentAt,
                    Message = "Save to HBase failed!"
                });
            }
        }
    }
}
