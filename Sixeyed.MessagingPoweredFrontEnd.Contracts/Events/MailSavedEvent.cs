using System;

namespace Sixeyed.MessagingPoweredFrontEnd.Contracts
{
    public class MailSavedEvent
    {
        public string Sender { get; set; }

        public DateTime SentAt { get; set; }

        public DateTime SavedAt { get; set; }
    }
}
