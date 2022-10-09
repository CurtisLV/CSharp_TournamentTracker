using TrackerLibrary.Models;
using TrackerLibrary.DataAccess.TextHelpers;

namespace TrackerLibrary.DataAccess;

public class TextConnector : IDataConnection
{
    private const string PrizesFile = "PrizesModels.csv";
    private const string PeopleFile = "PersonModels.csv";

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
        throw new NotImplementedException();
    }
}
