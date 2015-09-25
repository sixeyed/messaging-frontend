using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Sixeyed.MessagingPoweredFrontEnd.Contracts;
using Sixeyed.MessagingPoweredFrontEnd.Core.Messaging;
using Sixeyed.MessagingPoweredFrontEnd.Web.Models;
using Sixeyed.MessagingPoweredFrontEnd.Web.SignalR;
using System;
using System.Collections.Concurrent;

namespace Sixeyed.MessagingPoweredFrontEnd.Web.Handlers
{
    public static class ReplyHandler
    {
        private static ConcurrentDictionary<string, string> _UserConnectionIds = new ConcurrentDictionary<string, string>();
        public static Queue Queue = new Queue(createReplyQueue: true);
        private static IHubConnectionContext<dynamic> _Clients;       
        
        public static void Init()
        {
            Queue.Listen<MailBroadcastEvent>(Queue.ReplyQueueName, x => MailBroadcast(x));
            Queue.Listen<MailSavedEvent>(Queue.ReplyQueueName, x => MailSaved(x));
            _Clients = GlobalHost.ConnectionManager.GetHubContext<NoticeboardHub>().Clients;
        }

        private static void MailSaved(MailSavedEvent mailSaved)
        {
            Send(mailSaved.Sender, () => new ResponseModel
            {
                Sender = mailSaved.Sender,
                SentAt = mailSaved.SentAt.ToString("HH:mm:ss.fff"),
                EventAt = mailSaved.SavedAt.ToString("HH:mm:ss.fff"),
                Event = "saved"
            });
        }

        private static void MailBroadcast(MailBroadcastEvent mailBroadcast)
        {
            Send(mailBroadcast.Sender, () => new ResponseModel
            {
                Sender = mailBroadcast.Sender,
                SentAt = mailBroadcast.SentAt.ToString("HH:mm:ss.fff"), 
                EventAt = mailBroadcast.BroadcastAt.ToString("HH:mm:ss.fff"),
                Event = "broadcast"
            });
        }

        private static void Send(string userId, Func<ResponseModel> modelBuilder)
        {
            if (!_UserConnectionIds.ContainsKey(userId))
            {
                return;
            }

            var connectionId = _UserConnectionIds[userId];
            var model = modelBuilder();

            _Clients.Client(connectionId).showResponse(model);
        }

        public static void RegisterUser(string userId, string connectionId)
        {
            _UserConnectionIds[userId] = connectionId;
        }
    }
}