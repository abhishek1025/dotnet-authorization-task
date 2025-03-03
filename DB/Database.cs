using System.Data;
using Npgsql;

namespace authorization_project.DB;

public class Database: IDatabase
{
    private readonly IConfiguration _configuration;

    public Database(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public NpgsqlConnection CreateConnection()
    {
        return new NpgsqlConnection(Environment.GetEnvironmentVariable("DB_URL"));
    }
}