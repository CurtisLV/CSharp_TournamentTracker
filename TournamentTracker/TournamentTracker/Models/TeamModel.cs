namespace TrackerLibrary.Models;

public class TeamModel
{
    public int Id { get; set; }
    public string TeamName { get; set; }

    /// <summary>
    /// Represents team with team members
    /// </summary>
    public List<PersonModel> TeamMembers { get; set; } = new List<PersonModel>();
}
