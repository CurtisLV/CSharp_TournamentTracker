﻿namespace TrackerLibrary.Models;

/// <summary>
/// Represents one match in the tournament
/// </summary>
public class MatchupModel
{
    /// <summary>
    /// The unique identifier for the matchup
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Set of teams involved in this match
    /// </summary>
    public List<MatchupEntryModel> Entries { get; set; } = new List<MatchupEntryModel>();

    /// <summary>
    /// The ID from Db that will be used to identify the winner
    /// </summary>
    public int WinnerId { get; set; }

    /// <summary>
    /// Match winner
    /// </summary>
    public TeamModel Winner { get; set; }

    /// <summary>
    /// Which round this match is a part of
    /// </summary>
    public int MatchupRound { get; set; }
}
