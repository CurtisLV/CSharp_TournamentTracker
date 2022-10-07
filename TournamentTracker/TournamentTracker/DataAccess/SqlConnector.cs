using Dapper;
using System.Data;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess;

public class SqlConnector : IDataConnection
{
    public PersonModel CreatePerson(PersonModel model)
    {
        throw new NotImplementedException();
    }

    // TODO - Make the CreatePrize method actually save to the database
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
                GlobalConfig.ConnectionString("Tournaments")
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
}
