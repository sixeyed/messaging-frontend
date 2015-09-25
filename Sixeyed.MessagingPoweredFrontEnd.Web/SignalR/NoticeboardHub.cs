using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Sixeyed.MessagingPoweredFrontEnd.Contracts;
using Sixeyed.MessagingPoweredFrontEnd.Web.Handlers;
using Sixeyed.MessagingPoweredFrontEnd.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sixeyed.MessagingPoweredFrontEnd.Web.SignalR
{
    [HubName("noticeboard")]
    public class NoticeboardHub : Hub
    {
        public IEnumerable<MailModel> GetMail()
        {
            return NewMailHandler.MailBag.OrderByDescending(x => x.SentDate);
        }

        public void RegisterUser(string userId)
        {
            ReplyHandler.RegisterUser(userId, Context.ConnectionId);
        }

        public void Send(string sender, string content)
        {
            ReplyHandler.Queue.Publish<SendMailRequest>(new SendMailRequest
            {
                Content = content,
                Sender = sender,
                SentAt = DateTime.Now
            });
        }
    }
}