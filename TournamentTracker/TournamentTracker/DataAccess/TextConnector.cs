using TrackerLibrary.Models;
using TrackerLibrary.DataAccess.TextHelpers;

namespace TrackerLibrary.DataAccess;

public class TextConnector : IDataConnection
{
    public void CreatePerson(PersonModel model)
    {
        List<PersonModel> persons = GlobalConfig.PeopleFile
            .FullFilePath()
            .LoadFile()
            .ConvertToPersonModel();
        int currentId = 1;
        if (persons.Count > 0)
        {
            currentId = persons.OrderByDescending(x => x.Id).First().Id + 1;
        }

        model.Id = currentId;
        persons.Add(model);
        persons.SaveToPeopleFile();
    }

    public void CreatePrize(PrizeModel model)
    {
        // Load the text file
        // Convert the text to List<PrizeModel>
        List<PrizeModel> prizes = GlobalConfig.PrizesFile
            .FullFilePath()
            .LoadFile()
            .ConvertToPrizeModels();

        // Find the max ID (like last row)
        int currentId = 1;
        if (prizes.Count > 0)
        {
            currentId = prizes.OrderByDescending(x => x.Id).First().Id + 1;
        }

        model.Id = currentId;

        // Add new record with the new ID
        prizes.Add(model);

        // Convert prizes to List<string>
        // Save the List<string> to the text file

        prizes.SaveToPrizeFile();
    }

    public List<PersonModel> GetPerson_All()
    {
        return GlobalConfig.PeopleFile.FullFilePath().LoadFile().ConvertToPersonModel();
    }

    public void CreateTeam(TeamModel model)
    {
        List<TeamModel> teams = GlobalConfig.TeamFile
            .FullFilePath()
            .LoadFile()
            .ConvertToTeamModels();

        // Find the max ID (like last row)
        int currentId = 1;
        if (teams.Count > 0)
        {
            currentId = teams.OrderByDescending(x => x.Id).First().Id + 1;
        }

        model.Id = currentId;

        teams.Add(model);

        teams.SaveToTeamFile();
    }

    public List<TeamModel> GetTeam_All()
    {
        return GlobalConfig.TeamFile.FullFilePath().LoadFile().ConvertToTeamModels();
    }

    public void CreateTournament(TournamentModel model)
    {
        List<TournamentModel> tournaments = GlobalConfig.TournamentFile
            .FullFilePath()
            .LoadFile()
            .ConvertToTournamentModels();

        int currentId = 1;
        if (tournaments.Count > 0)
        {
            currentId = tournaments.OrderByDescending(x => x.Id).First().Id + 1;
        }

        model.Id = currentId;

        model.SaveRoundsToFile();

        tournaments.Add(model);

        tournaments.SaveToTournamentFile();
    }

    public List<TournamentModel> GetTournament_All()
    {
        return GlobalConfig.TournamentFile.FullFilePath().LoadFile().ConvertToTournamentModels();
    }

    public void UpdateMatchup(MatchupModel model)
    {
        model.UpdateMatchupToFile();
    }
}
