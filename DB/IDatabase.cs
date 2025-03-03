using Npgsql;

namespace authorization_project.DB;

public interface IDatabase
{
    public NpgsqlConnection CreateConnection();
}