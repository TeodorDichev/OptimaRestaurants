using System.Security.Policy;

namespace webapi.DTOs.Account
{
    public class EmailSendDto
    {
        public string To { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
        public EmailSendDto(string to, string body, string subject)
        {
            To = to;
            Body = body;
            Subject = subject;
        }
    }
}
