using Excel.Factory;
using Excel.IService;
using Excel.Repository;
using Excel.VM;

namespace Excel.AppService
{
    public class LoginAppService : ILoginAppService
    {
        private readonly IOrmServiceFactory _ormServiceFactory;
        public LoginAppService(IOrmServiceFactory ormServiceFactory)
        {
            _ormServiceFactory = ormServiceFactory;
        }

        public async Task<Users> GetUserAsync(string username, string orm)
        {
            var sslService = _ormServiceFactory.Get(orm);
            return await sslService.GetUserAsync(username);
        }
    }
}
