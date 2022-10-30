using System.Configuration;
using TrackerLibrary.DataAccess;

namespace TrackerLibrary;

public static class GlobalConfig
{
    public const string PrizesFile = "PrizesModels.csv";
    public const string PeopleFile = "PersonModels.csv";
    public const string TeamFile = "TeamModels.csv";
    public const string TournamentFile = "TournamentModels.csv";
    public const string MatchupFile = "MatchupModels.csv";
    public const string MatchupEntryFile = "MatchupEntryModels.csv";

    public static IDataConnection Connection { get; private set; }

    public static void InitializeConnections(DatabaseType db)
    {
        if (db == DatabaseType.Sql)
        {
            SqlConnector sql = new SqlConnector();
            Connection = sql;
        }
        else if (db == DatabaseType.TextFile)
        {
            TextConnector text = new TextConnector();
            Connection = text;
        }
    }

    /// <summary>
    /// To get connectionString back from app.config
    /// </summary>
    /// <param name="name"></param>
    /// <returns> Returns full connectionString parameter (server name) from app.config </returns>
    public static string ConnectionString(string name)
    {
        return ConfigurationManager.ConnectionStrings[name].ConnectionString;
    }
}
