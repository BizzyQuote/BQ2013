using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using PostmarkDotNet;

namespace BizzyQuote.Core
{
    public class Mailer
    {
        public bool SendEmail(string from, string replyTo, string subject, string body, string to)
        {
            try
            {
                const string postmarkApiKey = "30ddd0b0-0b9a-432e-a892-3c8739aabf0c";

                var message = new PostmarkMessage
                {
                    From = from.Trim(),
                    To = to.Trim(),
                    Subject = subject,
                    HtmlBody = body,
                    TextBody = body,
                    ReplyTo = replyTo ?? from
                };

                var client = new PostmarkClient(postmarkApiKey.Trim());

                var response = client.SendMessage(message);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
