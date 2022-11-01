﻿using System.Configuration;
using TrackerLibrary.Models;

namespace TrackerLibrary;

public static class TournamentLogic
{
    // Order our list randomly of teams
    // Check if the list is big enough, if not - add byes (automatic win)
    // Create our first round of matchups
    // Create every round after that - 8/4/2 matchups, 1 final

    public static void CreateRounds(TournamentModel model)
    {
        List<TeamModel> randomizedTeams = RandomizeTeamOrder(model.EnteredTeams);
        int rounds = FindNumberOfRounds(randomizedTeams.Count);
        int byes = NumberOfByes(rounds, randomizedTeams.Count);

        model.Rounds.Add(CreateFirstRound(byes, randomizedTeams));

        CreateOtherRounds(model, rounds);
    }

    public static void UpdateTournamentResults(TournamentModel model)
    {
        List<MatchupModel> toScore = new List<MatchupModel>();
        foreach (List<MatchupModel> round in model.Rounds)
        {
            foreach (MatchupModel rm in round)
            {
                if (
                    rm.Winner != null
                    && (rm.Entries.Any(x => x.Score != 0) || rm.Entries.Count == 1)
                )
                {
                    toScore.Add(rm);
                }
            }
        }

        MarkWinnersInMatchups(toScore);

        AdvanceWinners(toScore);

        //GlobalConfig.Connection.UpdateMatchup(m);
    }

    private static void AdvanceWinners(List<MatchupModel> models)
    {
        //foreach (List<MatchupModel> round in model.Rounds)
        //{
        //    foreach (MatchupModel rm in round)
        //    {
        //        foreach (MatchupEntryModel me in rm.Entries)
        //        {
        //            if (me.ParentMatchup != null)
        //            {
        //                if (me.ParentMatchup.Id == m.Id)
        //                {
        //                    me.TeamCompeting = m.Winner;
        //                    GlobalConfig.Connection.UpdateMatchup(rm);
        //                }
        //            }
        //        }
        //    }
        //}

       
    }

    private static void MarkWinnersInMatchups(List<MatchupModel> models)
    {
        string greaterWins = ConfigurationManager.AppSettings["greaterWins"];

        // 0 = false - low score wins (like golf), anything else - high score wins

        foreach (MatchupModel m in models)
        {
            // checks for the byes / auto win
            if (m.Entries.Count == 1)
            {
                m.Winner = m.Entries[0].TeamCompeting;
                continue;
            }

            if (greaterWins == "0")
            {
                if (m.Entries[0].Score < m.Entries[1].Score)
                {
                    m.Winner = m.Entries[0].TeamCompeting;
                }
                else if (m.Entries[1].Score < m.Entries[0].Score)
                {
                    m.Winner = m.Entries[1].TeamCompeting;
                }
                else
                {
                    throw new Exception("We do not allow ties in this application!");
                }
            }
            else
            {
                // 1 = true, or high score wins}
                if (m.Entries[0].Score > m.Entries[1].Score)
                {
                    m.Winner = m.Entries[0].TeamCompeting;
                }
                else if (m.Entries[1].Score > m.Entries[0].Score)
                {
                    m.Winner = m.Entries[1].TeamCompeting;
                }
                else
                {
                    throw new Exception("We do not allow ties in this application!");
                }
            }

            //if (teamOneScore > teamTwoScore)
            //{
            //    // Team one wins
            //    m.Winner = m.Entries[0].TeamCompeting;
            //}
            //else if (teamOneScore < teamTwoScore)
            //{
            //    // Team two wins
            //    m.Winner = m.Entries[1].TeamCompeting;
            //}
            //else
            //{
            //    MessageBox.Show("I do not handle tie games!");
            //}
        }
    }

    private static void CreateOtherRounds(TournamentModel model, int rounds)
    {
        int round = 2;
        List<MatchupModel> prevRound = model.Rounds[0];
        List<MatchupModel> currRound = new List<MatchupModel>();
        MatchupModel currMatchup = new MatchupModel();

        while (round <= rounds)
        {
            foreach (MatchupModel match in prevRound)
            {
                currMatchup.Entries.Add(new MatchupEntryModel { ParentMatchup = match });

                if (currMatchup.Entries.Count > 1)
                {
                    currMatchup.MatchupRound = round;
                    currRound.Add(currMatchup);
                    currMatchup = new MatchupModel();
                }
            }
            model.Rounds.Add(currRound);
            prevRound = currRound;
            currRound = new List<MatchupModel>();
            round++;
        }
    }

    private static List<MatchupModel> CreateFirstRound(int byes, List<TeamModel> teams)
    {
        List<MatchupModel> output = new List<MatchupModel>();
        MatchupModel curr = new MatchupModel();

        foreach (TeamModel team in teams)
        {
            curr.Entries.Add(new MatchupEntryModel { TeamCompeting = team });

            if (byes > 0 || curr.Entries.Count > 1)
            {
                curr.MatchupRound = 1;
                output.Add(curr);
                curr = new MatchupModel();

                if (byes > 0)
                {
                    byes -= 1;
                }
            }
        }
        return output;
    }

    private static int NumberOfByes(int rounds, int numberOfTeams)
    {
        int output = 0;
        int totalTeams = 1;
        for (int i = 1; i <= rounds; i++)
        {
            totalTeams *= 2;
        }

        output = totalTeams - numberOfTeams;
        return output;
    }

    private static int FindNumberOfRounds(int teamCount)
    {
        int output = 1;
        int val = 2;

        while (val < teamCount)
        {
            output += 1;
            val *= 2;
        }

        return output;
    }

    private static List<TeamModel> RandomizeTeamOrder(List<TeamModel> teams)
    {
        return teams.OrderBy(x => Guid.NewGuid()).ToList();
    }
}
