using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Shared.Infrastructure.Contexts;

public class DapperContext
{
    private readonly string _connectionString;

    public DapperContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentException("A string de conexão com o DB é obrigatória");
    }

    public IDbConnection CreateConnection()
    {
        var connection = new SqlConnection(_connectionString);

        try
        {
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
        }
        catch (Exception e)
        {
            throw new Exception("Erro ao conectar no DB:", e);
        }
        
        return connection;
    }
}