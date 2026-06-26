
using System.Reflection;
using DbUp;
using Microsoft.Extensions.Configuration;

namespace Shared.Infrastructure.Configurations;

public static class DbUpConfiguration
{
    public static void RunMigrations(this IConfiguration configuration, Assembly migrationAssembly)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
            throw new ArgumentException("A string de conexão com o DB é obrigatória");

        EnsureDatabase.For.SqlDatabase(connectionString);

        var upgrader =
            DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(migrationAssembly)
                .LogToConsole()
                .Build();

        var result = upgrader.PerformUpgrade();

        if (!result.Successful)
        {
            throw new Exception($"Algo deu errado ao tentar se conectar com o DB: {result.Error}");
        }
    }
}