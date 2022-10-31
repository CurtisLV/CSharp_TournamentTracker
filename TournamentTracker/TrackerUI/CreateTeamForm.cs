using TrackerLibrary;
using TrackerLibrary.Models;

namespace TrackerUI;

public partial class CreateTeamForm : Form
{
    private ITeamRequestor callingForm;
    private List<PersonModel> availableTeamMembers = GlobalConfig.Connection.GetPerson_All();
    private List<PersonModel> selectedTeamMembers = new List<PersonModel>();

    public CreateTeamForm(ITeamRequestor caller)
    {
        InitializeComponent();
        callingForm = caller;
        WireUpLists();
    }

    private void CreateSampleData()
    {
        availableTeamMembers.Add(new PersonModel { FirstName = "Kārlis", LastName = "Eglīte" });
        availableTeamMembers.Add(new PersonModel { FirstName = "Kaža", LastName = "Draugs" });

        selectedTeamMembers.Add(new PersonModel { FirstName = "Dace", LastName = "DabūjaPaAci" });
        selectedTeamMembers.Add(new PersonModel { FirstName = "Twitter", LastName = "Robot" });
    }

    private void WireUpLists()
    {
        // TODO - better refreshing

        // solves dropdownlist not updating
        selectTeamMemberDropDown.DataSource = null;

        selectTeamMemberDropDown.DataSource = availableTeamMembers;
        selectTeamMemberDropDown.DisplayMember = "FullName";

        // solves listBox not updating
        teamMemberListBox.DataSource = null;

        teamMemberListBox.DataSource = selectedTeamMembers;
        teamMemberListBox.DisplayMember = "FullName";

        selectTeamMemberDropDown.Refresh();
    }

    private void createMemberButton_Click(object sender, EventArgs e)
    {
        // validate the form
        if (ValidateForm())
        {
            // take info from all 4 fields, create person model from them and then save to sql or text
            PersonModel p = new PersonModel();
            p.FirstName = firstNameValue.Text;
            p.LastName = lastNameValue.Text;
            p.EmailAddress = emailValue.Text;
            p.PhoneNumber = cellphoneValue.Text;

            GlobalConfig.Connection.CreatePerson(p);

            selectedTeamMembers.Add(p);
            WireUpLists();

            // clear out the form
            firstNameValue.Text = "";
            lastNameValue.Text = "";
            emailValue.Text = "";
            cellphoneValue.Text = "";
        }
        else
        {
            MessageBox.Show("You need to fill in all fields in the form!");
        }
    }

    private bool ValidateForm()
    {
        if (firstNameValue.Text.Length == 0)
        {
            return false;
        }
        if (lastNameValue.Text.Length == 0)
        {
            return false;
        }
        if (emailValue.Text.Length == 0)
        {
            return false;
        }
        if (cellphoneValue.Text.Length == 0)
        {
            return false;
        }
        return true;
    }

    private void addMemberButton_Click(object sender, EventArgs e)
    {
        PersonModel p = (PersonModel)selectTeamMemberDropDown.SelectedItem;

        if (p != null)
        {
            availableTeamMembers.Remove(p);
            selectedTeamMembers.Add(p);

            WireUpLists();
        }
    }

    private void removeSelectedMemberButton_Click(object sender, EventArgs e)
    {
        PersonModel p = (PersonModel)teamMemberListBox.SelectedItem;
        if (p != null)
        {
            selectedTeamMembers.Remove(p);
            availableTeamMembers.Add(p);

            WireUpLists();
        }
    }

    private void createTeamButton_Click(object sender, EventArgs e)
    {
        TeamModel t = new TeamModel();

        t.TeamName = teamNameValue.Text;
        t.TeamMembers = selectedTeamMembers;

        GlobalConfig.Connection.CreateTeam(t);

        callingForm.TeamComplete(t);
        this.Close();
    }
}
