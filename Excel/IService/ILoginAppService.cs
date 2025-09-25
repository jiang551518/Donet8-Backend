using Excel.VM;

namespace Excel.IService
{
    public interface ILoginAppService
    {
        Task<Users> GetUserAsync(string username, string orm);
    }
}
