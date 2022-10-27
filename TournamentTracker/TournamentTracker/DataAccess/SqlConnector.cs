using System.Data;
using System.Reflection;
using Dapper;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess;

public class SqlConnector : IDataConnection
{
    private const string db = "Tournaments";

    public PersonModel CreatePerson(PersonModel model)
    {
        // Using statement makes sure that connection is closed at the end of curly brace
        using (
            IDbConnection connection = new System.Data.SqlClient.SqlConnection(
                GlobalConfig.ConnectionString(db)
            )
        )
        {
            var p = new DynamicParameters();
            p.Add("@FirstName", model.FirstName);
            p.Add("@LastName", model.LastName);
            p.Add("@EmailAddress", model.EmailAddress);
            p.Add("@CellPhoneNumber", model.PhoneNumber);
            p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

            // executes stored procedure with above .add() as parametres
            connection.Execute("dbo.spPeople_Insert", p, commandType: CommandType.StoredProcedure);

            model.Id = p.Get<int>("@id");

            return model;
        }
    }

    /// <summary>
    /// Saves a new prize to the database
    /// </summary>
    /// <param name="model"></param>
    /// <returns> The prize information, including the unique identifier </returns>
    public PrizeModel CreatePrize(PrizeModel model)
    {
        // Using statement makes sure that connection is closed at the end of curly brace
        using (
            IDbConnection connection = new System.Data.SqlClient.SqlConnection(
                GlobalConfig.ConnectionString(db)
            )
        )
        {
            var p = new DynamicParameters();
            p.Add("@PlaceNumber", model.PlaceNumber);
            p.Add("@PlaceName", model.PlaceName);
            p.Add("@PrizeAmount", model.PrizeAmount);
            p.Add("@PrizePercentage", model.PrizePercentage);
            p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

            // executes stored procedure with above .add() as parametres
            connection.Execute("dbo.spPrizes_Insert", p, commandType: CommandType.StoredProcedure);

            model.Id = p.Get<int>("@id");

            return model;
        }
    }

    public TeamModel CreateTeam(TeamModel model)
    {
        using (
            IDbConnection connection = new System.Data.SqlClient.SqlConnection(
                GlobalConfig.ConnectionString(db)
            )
        )
        {
            var p = new DynamicParameters();
            p.Add("@TeamName", model.TeamName);
            p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

            // executes stored procedure with above .add() as parametres
            connection.Execute("dbo.spTeams_Insert", p, commandType: CommandType.StoredProcedure);

            model.Id = p.Get<int>("@id");

            foreach (PersonModel tm in model.TeamMembers)
            {
                p = new DynamicParameters();
                p.Add("@TeamId", model.Id);
                p.Add("@PersonId", tm.Id);

                // executes stored procedure with above .add() as parametres
                connection.Execute(
                    "dbo.spTeamMembers_Insert",
                    p,
                    commandType: CommandType.StoredProcedure
                );
            }

            return model;
        }
    }

    public void CreateTournament(TournamentModel model)
    {
        using (
            IDbConnection connection = new System.Data.SqlClient.SqlConnection(
                GlobalConfig.ConnectionString(db)
            )
        )
        {
            SaveTournament(connection, model);
            SaveTournamentPrizes(connection, model);
            SaveTournamentEntries(connection, model);
            SaveTournamentRounds(connection, model);
        }
    }

    private void SaveTournament(IDbConnection connection, TournamentModel model)
    {
        var p = new DynamicParameters();
        p.Add("@TournamentName", model.TournamentName);
        p.Add("@EntryFee", model.EntryFee);
        p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

        connection.Execute("dbo.spTournaments_Insert", p, commandType: CommandType.StoredProcedure);

        model.Id = p.Get<int>("@id");
    }

    private void SaveTournamentPrizes(IDbConnection connection, TournamentModel model)
    {
        foreach (PrizeModel pz in model.Prizes)
        {
            var p = new DynamicParameters();
            p.Add("@TournamentID", model.Id);
            p.Add("@PrizeID", pz.Id);
            p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
            connection.Execute(
                "dbo.spTournamentPrizes_Insert",
                p,
                commandType: CommandType.StoredProcedure
            );
        }
    }

    private void SaveTournamentEntries(IDbConnection connection, TournamentModel model)
    {
        foreach (TeamModel team in model.EnteredTeams)
        {
            var p = new DynamicParameters();
            p.Add("@TournamendID", model.Id);
            p.Add("@TeamID", team.Id);
            p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

            connection.Execute(
                "dbo.spTournamentEntries_Insert",
                p,
                commandType: CommandType.StoredProcedure
            );
        }
    }

    private void SaveTournamentRounds(IDbConnection connection, TournamentModel model)
    {
        foreach (List<MatchupModel> round in model.Rounds)
        {
            foreach (MatchupModel matchup in round)
            {
                var p = new DynamicParameters();
                p.Add("@MatchupRound", matchup.MatchupRound);
                p.Add("@TournamentId", model.Id);
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                connection.Execute(
                    "dbo.spMatchups_Insert",
                    p,
                    commandType: CommandType.StoredProcedure
                );
                matchup.Id = p.Get<int>("@id");

                foreach (MatchupEntryModel entry in matchup.Entries)
                {
                    p = new DynamicParameters();
                    p.Add("@MatchupId", matchup.Id);

                    if (entry.ParentMatchup == null)
                    {
                        p.Add("@ParentMatchupId", null);
                    }
                    else
                    {
                        p.Add("@ParentMatchupId", entry.ParentMatchup.Id);
                    }

                    if (entry.TeamCompeting == null)
                    {
                        p.Add("@TeamCompetingId", null);
                    }
                    else
                    {
                        p.Add("@TeamCompetingId", entry.TeamCompeting.Id);
                    }
                    p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);

                    connection.Execute(
                        "dbo.spMatchupEntries_Insert",
                        p,
                        commandType: CommandType.StoredProcedure
                    );
                }
            }
        }
    }

    public List<PersonModel> GetPerson_All()
    {
        List<PersonModel> output;
        using (
            IDbConnection connection = new System.Data.SqlClient.SqlConnection(
                GlobalConfig.ConnectionString(db)
            )
        )
        {
            output = connection.Query<PersonModel>("dbo.spPeople_GetAll").ToList();
        }
        return output;
    }

    public List<TeamModel> GetTeam_All()
    {
        List<TeamModel> output;
        using (
            IDbConnection connection = new System.Data.SqlClient.SqlConnection(
                GlobalConfig.ConnectionString(db)
            )
        )
        {
            output = connection.Query<TeamModel>("dbo.spTeams_GetAll").ToList();

            foreach (TeamModel team in output)
            {
                var p = new DynamicParameters();
                p.Add("@TeamId", team.Id);
                team.TeamMembers = connection
                    .Query<PersonModel>(
                        "dbo.spTeamMembers_GetByTeam",
                        p,
                        commandType: CommandType.StoredProcedure
                    )
                    .ToList();
            }
        }
        return output;
    }

    public List<TournamentModel> GetTournament_All()
    {
        List<TournamentModel> output;
        using (
            IDbConnection connection = new System.Data.SqlClient.SqlConnection(
                GlobalConfig.ConnectionString(db)
            )
        )
        {
            var p = new DynamicParameters();
            output = connection.Query<TournamentModel>("dbo.spTournaments_GetAll").ToList();

            foreach (TournamentModel t in output)
            {
                // Populate prizes
                t.Prizes = connection.Query<PrizeModel>("dbo.spPrizes_GetByTournament").ToList();
                // Populate teams
                t.EnteredTeams = connection.Query<TeamModel>("dbo.spTeam_GetByTournament").ToList();

                foreach (TeamModel team in t.EnteredTeams)
                {
                    p = new DynamicParameters();
                    p.Add("@TeamId", team.Id);
                    team.TeamMembers = connection
                        .Query<PersonModel>(
                            "dbo.spTeamMembers_GetByTeam",
                            p,
                            commandType: CommandType.StoredProcedure
                        )
                        .ToList();
                }
                // Populate rounds spMatchups_GetByTournament

                p = new DynamicParameters();
                p.Add("@TournamentId", t.Id);
                List<MatchupModel> matchups = connection
                    .Query<MatchupModel>(
                        "dbo.spMatchups_GetByTournament",
                        p,
                        commandType: CommandType.StoredProcedure
                    )
                    .ToList();

                foreach (MatchupModel m in matchups)
                {
                    p = new DynamicParameters();
                    p.Add("@MatchupId", m.Id);
                    m.Entries = connection
                        .Query<MatchupEntryModel>(
                            "dbo.spMatchupEntries_GetByMatchup",
                            p,
                            commandType: CommandType.StoredProcedure
                        )
                        .ToList();

                    //Populate each entry 2models
                    // Populate each matchup 1 model

                    List<TeamModel> allTeams = GetTeam_All();

                    if (m.WinnerId > 0)
                    {
                        m.Winner = allTeams.First(x => x.Id == m.WinnerId);
                    }

                    foreach (var me in m.Entries)
                    {
                        if (me.TeamCompetingId > 0)
                        {
                            me.TeamCompeting = allTeams.First(x => x.Id == me.TeamCompetingId);
                        }

                        if (me.ParentMatchupId > 0)
                        {
                            me.ParentMatchup = matchups.First(x => x.Id == me.ParentMatchupId);
                        }
                    }
                }

                // List<List<<MatchupModel>>
                List<MatchupModel> currRow = new List<MatchupModel>();
                int currRound = 1;
                foreach (MatchupModel m in matchups)
                {
                    if (m.MatchupRound > currRound)
                    {
                        t.Rounds.Add(currRow);
                    }
                }
            }
        }
        return output;
    }
}
