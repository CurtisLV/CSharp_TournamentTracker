using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary;

public class PrizeModel
{
    /// <summary>
    /// The unique identifier for the prize
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Numeric identifier for the place (2 for second place etc..)
    /// </summary>
    public int PlaceNumber { get; set; }

    /// <summary>
    /// Friendly name for the place (second place, first runner up, etc.)
    /// </summary>
    public string PlaceName { get; set; }

    /// <summary>
    /// Fixed amount this place earns or zero if it is not used
    /// </summary>
    public decimal PrizeAmount { get; set; }

    /// <summary>
    /// The number that represents % of overall take or
    /// zero if it is not used. % is a fraction of 1 (so 0.5 for 50%)
    /// </summary>
    public double PrizePercentage { get; set; }

    public PrizeModel()
    {
        //
    }

    public PrizeModel(
        string placeNumber,
        string placeName,
        string prizeAmount,
        string prizePercentage
    )
    {
        // Parse all string to numbers where necessary
        PlaceName = placeName;

        int placeNumberValue = 0;
        int.TryParse(placeNumber, out placeNumberValue);
        PlaceNumber = placeNumberValue;

        decimal prizeAmountValue = 0;
        decimal.TryParse(prizeAmount, out prizeAmountValue);
        PrizeAmount = prizeAmountValue;

        double prizePerecentageValue = 0;
        double.TryParse(prizePercentage, out prizePerecentageValue);
        PrizePercentage = prizePerecentageValue;
    }
}
