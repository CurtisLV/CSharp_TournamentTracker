using System.Configuration;
using System.Text;
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

    public static void UpdateTournamentsResults(TournamentModel model)
    {
        int startingRound = model.CheckCurrentRound();
        List<MatchupModel> toScore = new List<MatchupModel>();
        foreach (List<MatchupModel> round in model.Rounds)
        {
            foreach (MatchupModel rm in round)
            {
                if (
                    rm.Winner == null
                    && (rm.Entries.Any(x => x.Score != 0) || rm.Entries.Count == 1)
                )
                {
                    toScore.Add(rm);
                }
            }
        }

        MarkWinnersInMatchups(toScore);

        AdvanceWinners(toScore, model);

        toScore.ForEach(x => GlobalConfig.Connection.UpdateMatchup(x));
        int endingRound = model.CheckCurrentRound();

        if (endingRound > startingRound)
        {
            // where we alert users - email

            model.AlertUsersToNewRound();
        }
    }

    public static void AlertUsersToNewRound(this TournamentModel model)
    {
        int currentRoundNum = model.CheckCurrentRound();
        List<MatchupModel> currentRound = model.Rounds.First(
            x => x.First().MatchupRound == currentRoundNum
        );

        foreach (MatchupModel matchup in currentRound)
        {
            foreach (MatchupEntryModel me in matchup.Entries)
            {
                foreach (PersonModel p in me.TeamCompeting.TeamMembers)
                {
                    AlertPersonToNewRound(
                        p,
                        me.TeamCompeting.TeamName,
                        matchup.Entries.FirstOrDefault(x => x.TeamCompeting != me.TeamCompeting)
                    );
                    ;
                }
            }
        }
    }

    private static void AlertPersonToNewRound(
        PersonModel p,
        string teamName,
        MatchupEntryModel? competitor
    )
    {
        if (p.EmailAddress.Length == 0)
        {
            return;
        }

        string to = "";
        string subject = "";
        StringBuilder body = new StringBuilder();

        if (competitor != null)
        {
            subject = $"You have a new matchup with {competitor.TeamCompeting.TeamName}";

            body.AppendLine("<h1>You have a new matchup</h1>");
            body.Append("<strong>Competitor: </strong>");
            body.Append(competitor.TeamCompeting.TeamName);
            body.AppendLine();
            body.AppendLine();
            body.AppendLine("Have a great time!");
            body.AppendLine("~ Tournament Tracker ~");
        }
        else
        {
            subject = "Yiu have a bye week this round!";
            body.AppendLine("Enjoy your round off!");
            body.AppendLine("~ Tournament Tracker ~");
        }

        to = p.EmailAddress;

        EmailLogic.SendEmail(to, subject, body.ToString());
    }

    private static int CheckCurrentRound(this TournamentModel model)
    {
        int output = 1;

        foreach (List<MatchupModel> round in model.Rounds)
        {
            if (round.All(x => x.Winner != null))
            {
                output += 1;
            }
            else
            {
                return output;
            }
        }

        // Tournament is complete
        CompleteTournament(model);

        return output - 1;
    }

    private static void CompleteTournament(TournamentModel model)
    {
        GlobalConfig.Connection.CompleteTournament(model);

        TeamModel winners = model.Rounds.Last().First().Winner;
        TeamModel runnerUp = model.Rounds
            .Last()
            .First()
            .Entries.First(x => x.TeamCompeting != winners)
            .TeamCompeting;

        decimal winnerPrize = 0;
        decimal runnerUpPrize = 0;

        if (model.Prizes.Count > 0)
        {
            decimal totalIncome = model.EnteredTeams.Count * model.EntryFee;

            PrizeModel firstPlacePrize = model.Prizes.FirstOrDefault(x => x.PlaceNumber == 1);
            PrizeModel secondPlacePrize = model.Prizes.FirstOrDefault(x => x.PlaceNumber == 2);

            if (firstPlacePrize != null)
            {
                // calculate value
                winnerPrize = firstPlacePrize.CalculatePrizePayout(totalIncome);
            }
            if (secondPlacePrize != null)
            {
                // calculate value
                runnerUpPrize = secondPlacePrize.CalculatePrizePayout(totalIncome);
            }
        }

        // send email to all tournament

        string subject = "";
        StringBuilder body = new StringBuilder();

        subject = $"In {model.TournamentName},  {winners.TeamName} has won!";

        body.AppendLine("<h1>We have a winner!</h1>");
        body.AppendLine("<p>Congratz to our winner on a great tournament!</p>");
        body.AppendLine("<br />");
        if (winnerPrize > 0)
        {
            body.AppendLine(
                $"<p>{winners.TeamName} will receive ${winnerPrize} for first place!</p>"
            );
        }
        if (runnerUpPrize > 0)
        {
            body.AppendLine(
                $"<p>{runnerUp.TeamName} will receive ${runnerUpPrize} for second place!</p>"
            );
        }
        body.AppendLine("<p>Thanks for a great tournament, everyone!</p>");
        body.AppendLine("~ Tournament Tracker ~");

        List<string> bcc = new List<string>();

        foreach (TeamModel t in model.EnteredTeams)
        {
            foreach (PersonModel p in t.TeamMembers)
            {
                if (p.EmailAddress.Length > 0)
                {
                    bcc.Add(p.EmailAddress);
                }
            }
        }

        EmailLogic.SendEmail(new List<string>(), bcc, subject, body.ToString());

        // Complete Tournament
        model.CompleteTournament();
    }

    private static decimal CalculatePrizePayout(this PrizeModel prize, decimal totalIncome)
    {
        decimal output = 0;
        if (prize.PrizeAmount > 0)
        {
            output = prize.PrizeAmount;
        }
        else
        {
            output = Decimal.Multiply(totalIncome, Convert.ToDecimal(prize.PrizePercentage / 100));
        }

        return output;
    }

    private static void AdvanceWinners(List<MatchupModel> models, TournamentModel tournament)
    {
        foreach (MatchupModel m in models)
        {
            foreach (List<MatchupModel> round in tournament.Rounds)
            {
                foreach (MatchupModel rm in round)
                {
                    foreach (MatchupEntryModel me in rm.Entries)
                    {
                        if (me.ParentMatchup != null)
                        {
                            if (me.ParentMatchup.Id == m.Id)
                            {
                                me.TeamCompeting = m.Winner;
                                GlobalConfig.Connection.UpdateMatchup(rm);
                            }
                        }
                    }
                }
            }
        }
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
