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
    public static IDataConnection Connections { get; private set; }

    public static void InitializeConnections(string connectionType)
    {
        if (connectionType == "sql")
        {
            // TODO - Set up the SQL connector properly
            SqlConnector sql = new SqlConnector();
            Connections = sql;
        }
        else if (connectionType == "text")
        {
            // TODO - Set up text connector properly
            TextConnector text = new TextConnector();
            Connections = text;
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
