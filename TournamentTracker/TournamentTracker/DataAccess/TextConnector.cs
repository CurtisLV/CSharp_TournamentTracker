using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess;

public class TextConnector : IDataConnection
{
    // TODO - Wire up the CreatePrize for text files
    public PrizeModel CreatePrize(PrizeModel model)
    {
        // Load the text file

        // Convert the text to List<PrizeModel>

        // Find the max ID (like last row)

        // Add new record with the new ID

        // Convert prizes to List<string>

        // Save the List<string> to the text file
    }
}
