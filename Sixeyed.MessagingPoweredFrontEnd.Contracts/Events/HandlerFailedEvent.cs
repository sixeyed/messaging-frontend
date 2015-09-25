using System;

namespace Sixeyed.MessagingPoweredFrontEnd.Contracts
{
    public class HandlerFailedEvent
    {
        public string Sender { get; set; }

        public DateTime SentAt { get; set; }

        public string Message { get; set; }
    }
}
