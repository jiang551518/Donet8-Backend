using Excel.EfCoreDb;
using Excel.VM;

namespace Excel.IService
{
    public interface ILoginAppService
    {
        Task<UserResultVM> GetUserAsync(string username, string orm);
    }
}
