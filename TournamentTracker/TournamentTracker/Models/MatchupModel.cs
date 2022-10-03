using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackerLibrary.Models;

/// <summary>
/// Represents one match in the tournament
/// </summary>
public class MatchupModel
{
    /// <summary>
    /// Set of teams involved in this match
    /// </summary>
    public List<MatchupEntryModel> Entries { get; set; } = new List<MatchupEntryModel>();

    /// <summary>
    /// Match winner
    /// </summary>
    public TeamModel Winner { get; set; }

    /// <summary>
    /// Which round this match is a part of
    /// </summary>
    public int MatchupRound { get; set; }
}
