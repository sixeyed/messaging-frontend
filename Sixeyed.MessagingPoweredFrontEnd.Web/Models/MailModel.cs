using System;

namespace Sixeyed.MessagingPoweredFrontEnd.Web.Models
{
    public class MailModel
    {
        public string Sender { get; set; }

        public string Content { get; set; }

        public string SentAt { get; set; }

        public DateTime SentDate { get; set; }
    }
}