using System;

namespace Sixeyed.MessagingPoweredFrontEnd.Contracts
{
    public class MailBroadcastEvent
    {
        public string Sender { get; set; }

        public DateTime SentAt { get; set; }

        public DateTime BroadcastAt { get; set; }
    }
}
