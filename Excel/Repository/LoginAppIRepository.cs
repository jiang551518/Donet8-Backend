using Dapper;
using Excel.EfCoreDb;
using Excel.Repository;
using Excel.VM;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using SqlSugar;
using System.Data;
using System.Text;

namespace Excel.Repository
{
    public class LoginAppEfCoreIRepository : ILoginAppIRepository
    {
        private readonly MyDbContext _context;

        public LoginAppEfCoreIRepository(MyDbContext context) 
        {
            _context = context;
        }

        public async Task<Users> GetUserAsync(string username)
        {
            return await _context.Set<Users>().FirstOrDefaultAsync(x=> x.username == username);
        }
    }
}

public class LoginAppDapperIRepository : MysqlDapperConn, ILoginAppIRepository
{
    public LoginAppDapperIRepository(IConfiguration configuration) :base(configuration)
    {
    }

    public async Task<Users> GetUserAsync(string username)
    {
        StringBuilder sql = new StringBuilder("select * from users where username = @username");
        return await connection.QueryFirstOrDefaultAsync<Users>(sql.ToString(), new { username = username });
    }
}

public class LoginAppSqlSugarIRepository : ILoginAppIRepository
{
    private readonly ISqlSugarClient _sugarClient;

    public LoginAppSqlSugarIRepository(ISqlSugarClient sugarClient)
    {
        _sugarClient = sugarClient;
    }

    public async Task<Users> GetUserAsync(string username)
    {
        return await _sugarClient.Queryable<Users>().Where(x => x.username == username).FirstAsync();
    }
}