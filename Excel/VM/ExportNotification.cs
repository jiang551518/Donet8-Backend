namespace Excel.VM
{
    public class ExportNotification
    {
        public Guid UserId { get; set; }

        public string FileName { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
