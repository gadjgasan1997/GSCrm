namespace GSCrm.Models
{
    public class EmailNotification
    {
        public string Subject { get; set; }
        public string Message { get; set; }
        public string Sender { get; set; }
        public string SenderPassword { get; set; }
        public string RecipientAddress { get; set; }
        public string RecipientName { get; set; }
        public string Header { get; set; }
    }
}
