using Excel.Repository;

namespace Excel.Factory
{
    public interface IOrmServiceFactory
    {
        ILoginAppIRepository Get(string orm);
    }
}
