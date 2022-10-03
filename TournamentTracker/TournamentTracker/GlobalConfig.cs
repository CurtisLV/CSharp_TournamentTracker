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
    public static List<IDataConnection> Connections { get; private set; } =
        new List<IDataConnection>();

    public static void InitializeConnections(bool database, bool textfiles)
    {
        if (database)
        {
            // TODO - Set up the SQL connector properly
            SqlConnector sql = new SqlConnector();
            Connections.Add(sql);
        }
        if (textfiles)
        {
            // TODO - Set up text connector properly
            TextConnector text = new TextConnector();
            Connections.Add(text);
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
