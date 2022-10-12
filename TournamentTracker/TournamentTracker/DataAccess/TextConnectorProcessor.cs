using System.Configuration;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess.TextHelpers;

public static class TextConnectorProcessor
{
    // * Load the text file

    // * Convert the text to List<PrizeModel>

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

    // Convert the text to List<PrizeModel>
    public static List<PrizeModel> ConvertToPrizeModels(this List<string> lines)
    {
        List<PrizeModel> output = new List<PrizeModel>();

        foreach (string line in lines)
        {
            string[] columns = line.Split(',');

            PrizeModel p = new PrizeModel();
            p.Id = int.Parse(columns[0]);
            p.PlaceNumber = int.Parse(columns[1]);
            p.PlaceName = columns[2];
            p.PrizeAmount = decimal.Parse(columns[3]);
            p.PrizePercentage = double.Parse(columns[4]);

            output.Add(p);
        }
        return output;
    }

    public static List<PersonModel> ConvertToPersonModel(this List<string> lines)
    {
        List<PersonModel> output = new List<PersonModel>();

        foreach (string line in lines)
        {
            string[] columns = line.Split(',');

            PersonModel p = new PersonModel();
            p.Id = int.Parse(columns[0]);
            p.FirstName = columns[1];
            p.LastName = columns[2];
            p.EmailAddress = columns[3];
            p.PhoneNumber = columns[4];

            output.Add(p);
        }
        return output;
    }

    public static List<TeamModel> ConvertToTeamModels(
        this List<string> lines,
        string peopleFileName
    )
    {
        // id, team name, list of ids, separated by pipe |
        // 3, Tim's team, 1|3|5

        List<TeamModel> output = new List<TeamModel>();
        List<PersonModel> persons = peopleFileName.FullFilePath().LoadFile().ConvertToPersonModel();

        foreach (string line in lines)
        {
            string[] columns = line.Split(',');

            TeamModel t = new TeamModel();

            t.Id = int.Parse(columns[0]);
            t.TeamName = columns[1];

            string[] personIds = columns[2].Split('|');
        }
    }

    public static void SaveToPrizeFile(this List<PrizeModel> models, string fileName)
    {
        List<string> lines = new List<string>();
        foreach (PrizeModel p in models)
        {
            lines.Add($"{p.Id},{p.PlaceNumber},{p.PlaceName},{p.PrizeAmount},{p.PrizePercentage}");

            File.WriteAllLines(fileName.FullFilePath(), lines);
        }
    }

    public static void SaveToPeopleFile(this List<PersonModel> models, string fileName)
    {
        List<string> lines = new List<string>();
        foreach (PersonModel p in models)
        {
            lines.Add($"{p.Id},{p.FirstName},{p.LastName},{p.EmailAddress},{p.PhoneNumber}");
            File.WriteAllLines(fileName.FullFilePath(), lines);
        }
    }
}
