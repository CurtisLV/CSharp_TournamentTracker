using TrackerLibrary;
using TrackerLibrary.Models;

namespace TrackerUI;

public partial class CreateTeamForm : Form
{
    private List<PersonModel> availableTeamMembers = GlobalConfig.Connection.GetPerson_All();
    private List<PersonModel> selectedTeamMembers = new List<PersonModel>();

    public CreateTeamForm()
    {
        InitializeComponent();
        //CreateSampleData();
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
        selectTeamMemberDropDown.DataSource = availableTeamMembers;
        selectTeamMemberDropDown.DisplayMember = "FullName";

        teamMemberListBox.DataSource = selectedTeamMembers;
        teamMemberListBox.DisplayMember = "FullName";
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
        // TODO add validation to the form
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
}
