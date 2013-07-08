using System;
using BizzyQuote.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BizzyQuote.Tests.Core
{
    [TestClass]
    public class MailerTest
    {
        [TestMethod]
        public void SendTestEmailTest()
        {
            Mailer mail = new Mailer();
            string from = "postmark@bizzyquote.com";
            string to = "john@bizzyquote.com";
            string subject = "Test Email";
            string body = "<h1>This is simply a test email</h1>";
            string replyTo = "tech@bizzyquote.com";

            var result = mail.SendEmail(from, replyTo, subject, body, to);
        }
    }
}
