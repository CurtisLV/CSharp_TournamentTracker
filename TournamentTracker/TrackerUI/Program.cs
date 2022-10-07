namespace TrackerUI;
using TrackerLibrary;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Initialize database connections
        TrackerLibrary.GlobalConfig.InitializeConnections(DatabaseType.TextFile);

        Application.Run(new CreatePrizeForm());
        //Application.Run(new TournamentDashbordForm());
    }
}
