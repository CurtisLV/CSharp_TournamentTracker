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

    public TournamentModel CreateTournament(TournamentModel model)
    {
        using (
            IDbConnection connection = new System.Data.SqlClient.SqlConnection(
                GlobalConfig.ConnectionString(db)
            )
        )
        {
            SaveTournament(connection, model);

            foreach (PrizeModel pz in model.Prizes)
            {
                p = new DynamicParameters();
                p.Add("@TournamentID", model.Id);
                p.Add("@PrizeID", pz.Id);
                p.Add("@id", 0, dbType: DbType.Int32, direction: ParameterDirection.Output);
                connection.Execute(
                    "dbo.spTournamentPrizes_Insert",
                    p,
                    commandType: CommandType.StoredProcedure
                );
            }

            foreach (TeamModel team in model.EnteredTeams)
            {
                p = new DynamicParameters();
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
        return model;
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
}
