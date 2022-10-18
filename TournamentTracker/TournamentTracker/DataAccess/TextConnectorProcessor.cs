﻿using System.Configuration;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess.TextHelpers;

public static class TextConnectorProcessor
{
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
        List<PersonModel> people = peopleFileName.FullFilePath().LoadFile().ConvertToPersonModel();

        foreach (string line in lines)
        {
            string[] columns = line.Split(',');
            TeamModel t = new TeamModel { Id = int.Parse(columns[0]), TeamName = columns[1] };

            string[] personIds = columns[2].Split('|');
            foreach (string id in personIds)
            {
                t.TeamMembers.Add(people.FirstOrDefault(x => x.Id == int.Parse(id)));
            }
            output.Add(t);
        }
        return output;
    }

    public static List<TournamentModel> ConvertToTournamentModels(
        this List<string> lines,
        string teamFileName,
        string peopleFileName,
        string prizeFileName
    )
    {
        // ID,TournamentName,EntryFee,(id|id|id - Entered Teams), (id|id|id - Prizes), (Rounds - id^id^id|id^id^id|id^id^id)

        List<TournamentModel> output = new List<TournamentModel>();
        List<TeamModel> teams = teamFileName
            .FullFilePath()
            .LoadFile()
            .ConvertToTeamModels(peopleFileName);

        List<PrizeModel> prizes = prizeFileName.FullFilePath().LoadFile().ConvertToPrizeModels();

        foreach (string line in lines)
        {
            string[] columns = line.Split(',');
            TournamentModel tm = new TournamentModel();

            tm.Id = int.Parse(columns[0]);
            tm.TournamentName = columns[1];
            tm.EntryFee = decimal.Parse(columns[2]);

            string[] teamIds = columns[3].Split('|');
            foreach (string team in teamIds)
            {
                tm.EnteredTeams.Add(teams.First(x => x.Id == int.Parse(team)));
            }

            string[] prizeIds = columns[4].Split('|');
            foreach (string prize in prizeIds)
            {
                tm.Prizes.Add(prizes.First(x => x.Id == int.Parse(prize)));
            }

            // TODO - Capture Rounds information

            output.Add(tm);
        }

        return output;
    }

    public static void SaveToPrizeFile(this List<PrizeModel> models, string fileName)
    {
        List<string> lines = new List<string>();
        foreach (PrizeModel p in models)
        {
            lines.Add($"{p.Id},{p.PlaceNumber},{p.PlaceName},{p.PrizeAmount},{p.PrizePercentage}");
        }
        File.WriteAllLines(fileName.FullFilePath(), lines);
    }

    public static void SaveToPeopleFile(this List<PersonModel> models, string fileName)
    {
        List<string> lines = new List<string>();
        foreach (PersonModel p in models)
        {
            lines.Add($"{p.Id},{p.FirstName},{p.LastName},{p.EmailAddress},{p.PhoneNumber}");
        }
        File.WriteAllLines(fileName.FullFilePath(), lines);
    }

    public static void SaveToTeamFile(this List<TeamModel> models, string fileName)
    {
        List<string> lines = new List<string>();

        foreach (TeamModel t in models)
        {
            lines.Add($"{t.Id},{t.TeamName},{ConvertPeopleListToString(t.TeamMembers)}");
        }
        File.WriteAllLines(fileName.FullFilePath(), lines);
    }

    public static void SaveToTournamentFile(this List<TournamentModel> models, string fileName)
    {
        List<string> lines = new List<string>();

        foreach (TournamentModel tm in models)
        {
            lines.Add($"{tm.Id},{tm.TournamentName},{tm.EntryFee},{""}");
        }
    }

    private static string ConvertPeopleListToString(List<PersonModel> people)
    {
        string output = "";
        if (people.Count == 0)
        {
            return output;
        }

        foreach (PersonModel p in people)
        {
            output += $"{p.Id}|";
        }

        output = output.Substring(0, output.Length - 1);
        return output;
    }

    private static string ConvertTeamListToString(List<TeamModel> teams)
    {
        string output = "";
        if (teams.Count == 0)
        {
            return output;
        }

        foreach (TeamModel t in teams)
        {
            output += $"{t.Id}|";
        }

        output = output.Substring(0, output.Length - 1);
        return output;
    }
}
