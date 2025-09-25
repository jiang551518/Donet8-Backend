using Excel.EfCoreDb;
using Excel.Factory;
using Excel.IService;
using Excel.Repository;
using Excel.VM;
using Mapster;

namespace Excel.AppService
{
    public class LoginAppService : ILoginAppService
    {
        private readonly IOrmServiceFactory _ormServiceFactory;
        public LoginAppService(IOrmServiceFactory ormServiceFactory)
        {
            _ormServiceFactory = ormServiceFactory;
        }

        public async Task<UserResultVM> GetUserAsync(string username, string orm)
        {
            var sslService = _ormServiceFactory.Get(orm);
            var result = await sslService.GetUserAsync(username);
            return result.Adapt<UserResultVM>();
        }
    }
}
