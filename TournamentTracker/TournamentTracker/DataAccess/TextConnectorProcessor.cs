using System.Configuration;

namespace TrackerLibrary.DataAccess.TextConnector;

public static class TextConnectorProcessor
{
    // * Load the text file

    // Convert the text to List<PrizeModel>

    // Find the max ID (like last row)

    // Add new record with the new ID

    // Convert prizes to List<string>

    // Save the List<string> to the text file

    public static string FullFilePath(this string fileName) // PrizeModel.csv
    {
        // C:\Users\karli\Desktop\GitHub\CSharp_TournamentTracker\Text\PrizeModels.csv
        return $"{ConfigurationManager.AppSettings["filePath"]}\\{fileName}";
    }

    /// <summary>
    /// Load the text file
    /// </summary>
    /// <param name="file"></param>
    /// <returns> List of all lines </returns>
    public static List<string> LoadFile(this string file)
    {
        if (!File.Exists(file))
        {
            return new List<string>();
        }
        return File.ReadAllLines(file).ToList();
    }
}
