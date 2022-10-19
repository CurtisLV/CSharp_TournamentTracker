﻿using TrackerLibrary.Models;

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
    }

    private static List<TeamModel> RandomizeTeamOrder(List<TeamModel> teams)
    {
        return teams.OrderBy(x => Guid.NewGuid()).ToList();
    }
}
