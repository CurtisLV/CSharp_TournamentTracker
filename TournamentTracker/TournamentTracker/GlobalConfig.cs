using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrackerLibrary.DataAccess;

namespace TrackerLibrary;

public static class GlobalConfig
{
    public static IDataConnection Connection { get; private set; }

    public static void InitializeConnections(DatabaseType db)
    {
        if (db == DatabaseType.Sql)
        {
            // TODO - Set up the SQL connector properly
            SqlConnector sql = new SqlConnector();
            Connection = sql;
        }
        else if (db == DatabaseType.TextFile)
        {
            // TODO - Set up text connector properly
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
