using TrackerLibrary.Models;
using TrackerLibrary.DataAccess.TextHelpers;

namespace TrackerLibrary.DataAccess;

public class TextConnector : IDataConnection
{
    private const string PrizesFile = "PrizesModels.csv";
    private const string PeopleFile = "PersonModels.csv";
    private const string TeamFile = "TeamModels.csv";
    private const string TournamentFile = "TournamentModels.csv";

    public PersonModel CreatePerson(PersonModel model)
    {
        List<PersonModel> persons = PeopleFile.FullFilePath().LoadFile().ConvertToPersonModel();
        int currentId = 1;
        if (persons.Count > 0)
        {
            currentId = persons.OrderByDescending(x => x.Id).First().Id + 1;
        }

        model.Id = currentId;
        persons.Add(model);
        persons.SaveToPeopleFile(PeopleFile);

        return model;
    }

    public PrizeModel CreatePrize(PrizeModel model)
    {
        // Load the text file
        // Convert the text to List<PrizeModel>
        List<PrizeModel> prizes = PrizesFile.FullFilePath().LoadFile().ConvertToPrizeModels();

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

        prizes.SaveToPrizeFile(PrizesFile);

        return model;
    }

    public List<PersonModel> GetPerson_All()
    {
        return PeopleFile.FullFilePath().LoadFile().ConvertToPersonModel();
    }

    public TeamModel CreateTeam(TeamModel model)
    {
        List<TeamModel> teams = TeamFile.FullFilePath().LoadFile().ConvertToTeamModels(PeopleFile);

        // Find the max ID (like last row)
        int currentId = 1;
        if (teams.Count > 0)
        {
            currentId = teams.OrderByDescending(x => x.Id).First().Id + 1;
        }

        model.Id = currentId;

        teams.Add(model);

        teams.SaveToTeamFile(TeamFile);

        return model;
    }

    public List<TeamModel> GetTeam_All()
    {
        return TeamFile.FullFilePath().LoadFile().ConvertToTeamModels(PeopleFile);
    }

    public TournamentModel CreateTournament(TournamentModel model)
    {
        List<TournamentModel> tournaments = TournamentFile
            .FullFilePath()
            .LoadFile()
            .ConvertToTournamentModels(TeamFile, PeopleFile, PrizesFile);

        int currentId = 1;
        if (tournaments.Count > 0)
        {
            currentId = tournaments.OrderByDescending(x => x.Id).First().Id + 1;
        }

        model.Id = currentId;

        tournaments.Add(model);

        tournaments.SaveToTournamentsFile(TournamentFile);

        return model;
    }
}
