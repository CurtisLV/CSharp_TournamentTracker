using System.Configuration;

namespace TrackerLibrary.DataAccess.TextConnector;

public static class TextConnectorProcessor
{
    //
    public static string FullFilePath(string fileName) // PrizeModel.csv
    {
        // C:\Users\karli\Desktop\GitHub\CSharp_TournamentTracker\Text\PrizeModels.csv
        return $"{ConfigurationManager.AppSettings["filePath"]}\\{fileName}";
    }
}
