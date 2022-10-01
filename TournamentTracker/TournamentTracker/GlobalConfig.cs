using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary;

internal static class GlobalConfig
{
    public static List<IDataConnection> Connections { get; private set; }

    public static void InitializeConnections(bool database, bool Textfiles)
    {
        Connections = new List<IDataConnection>();
    }
}
