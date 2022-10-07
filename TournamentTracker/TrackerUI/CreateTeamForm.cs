namespace TrackerUI;

public partial class CreateTeamForm : Form
{
    public CreateTeamForm()
    {
        InitializeComponent();
    }

    private void createMemberButton_Click(object sender, EventArgs e)
    {
        // validate the form
        if (ValidateForm())
        {
            // take info from all 4 fields, create person model from them and then save to sql or text
        }
        else
        {
            MessageBox.Show("You need to fill in all fields in the form!");
        }

        // clear out the form
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
