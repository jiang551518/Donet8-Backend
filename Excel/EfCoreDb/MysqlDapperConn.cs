using MySql.Data.MySqlClient;

namespace Excel.EfCoreDb
{
    public class MysqlDapperConn
    {
        protected MySqlConnection connection;
        public IConfiguration _configuration { get; set; }
        public MysqlDapperConn(IConfiguration configuration)
        {
            _configuration = configuration;
            connection = new MySqlConnection(_configuration.GetConnectionString("DefaultConnection"));
        }
    }
}
