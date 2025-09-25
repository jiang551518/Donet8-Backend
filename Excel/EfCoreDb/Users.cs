namespace Excel.EfCoreDb
{
    public class Users
    {
        public Guid id { get; set; }

        public string username { get; set; }

        public string password { get; set; }

        public bool isenabled { get; set; }

        public bool isdeleted { get; set; }
    }
}
