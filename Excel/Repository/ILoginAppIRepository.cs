using Excel.VM;

namespace Excel.Repository
{
    public interface ILoginAppIRepository
    {
        Task<Users> GetUserAsync(string username);
    }
}
