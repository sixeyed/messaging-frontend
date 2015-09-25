using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Sixeyed.MessagingPoweredFrontEnd.Contracts;
using Sixeyed.MessagingPoweredFrontEnd.Core.Messaging;
using Sixeyed.MessagingPoweredFrontEnd.Web.Models;
using Sixeyed.MessagingPoweredFrontEnd.Web.SignalR;
using System;
using System.Collections.Generic;

namespace Sixeyed.MessagingPoweredFrontEnd.Web.Handlers
{
    public static class NewMailHandler
    {
        public static List<MailModel> MailBag { get; private set; }
        private static Queue _Queue;
        private static IHubConnectionContext<dynamic> _Clients;

        static NewMailHandler()
        {
            MailBag = new List<MailModel>();
            _Queue = new Queue();            
        }

        public static void Init()
        {
            _Queue.Listen<SendMailRequest>("noticeboard-broadcast", x => NewMail(x));
            _Clients = GlobalHost.ConnectionManager.GetHubContext<NoticeboardHub>().Clients;
        }
        
        private static void NewMail(SendMailRequest request)
        {
            try
            {
                var model = new MailModel
                {
                    Content = request.Content,
                    Sender = request.Sender,
                    SentAt = request.SentAt.ToString("HH:mm.ss"),
                    SentDate = request.SentAt
                };

                MailBag.Add(model);
                _Clients.All.newMail(model);

                _Queue.Reply<MailBroadcastEvent>(new MailBroadcastEvent
                {
                    Sender = request.Sender,
                    SentAt = request.SentAt,                    
                    BroadcastAt = DateTime.Now
                });
            }
            catch
            {
                _Queue.Reply<HandlerFailedEvent>(new HandlerFailedEvent
                {
                    Sender = request.Sender,
                    SentAt = request.SentAt,
                    Message = "Broadcast failed!"
                });
            }
        }
    }
}