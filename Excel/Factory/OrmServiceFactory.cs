using Excel.Repository;

namespace Excel.Factory;

public class OrmServiceFactory(IServiceProvider serviceProvider) : IOrmServiceFactory
{
    public ILoginAppIRepository Get(string orm)
    {
        return orm switch
        {
            "Dapper" => serviceProvider.GetRequiredService<LoginAppDapperIRepository>(),
            "EFCore" => serviceProvider.GetRequiredService<LoginAppEfCoreIRepository>(),
            "SqlSugar" => serviceProvider.GetRequiredService<LoginAppSqlSugarIRepository>(),
            _ => throw new NotSupportedException($"不支持的orm: {orm}")
        };
    }
}
