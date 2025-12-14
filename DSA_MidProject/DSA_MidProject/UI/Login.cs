

using DSA_MidProject.BL;
using DSA_MidProject.DL;
using Mysqlx.Crud;


namespace DSA_MidProject
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
            this.ActiveControl = panel1;
        }


        private void SetPlaceholder(TextBox txt, string placeholder, bool isPassword = false)
        {
            txt.Tag = placeholder;
            txt.Text = placeholder;
            txt.ForeColor = Color.Gray;

            if (isPassword)
                txt.UseSystemPasswordChar = false;

            txt.Enter += (s, e) =>
            {
                if (txt.Text == placeholder)
                {
                    txt.Text = "";
                    txt.ForeColor = Color.Black;

                    if (isPassword)
                        txt.UseSystemPasswordChar = true;
                }
            };

            txt.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txt.Text))
                {
                    txt.Text = placeholder;
                    txt.ForeColor = Color.Gray;

                    if (isPassword)
                        txt.UseSystemPasswordChar = false;
                }
            };
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetPlaceholder(textBox1, "Enter your user name");
            SetPlaceholder(textBox2, "Enter your password", true);
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Signup other = new Signup();
            NavigationManager.NavigateTo(other, this);

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ForgetPassword other = new ForgetPassword();
            NavigationManager.NavigateTo(other, this);

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (textBox2.Text != textBox2.Tag.ToString())
            {
                textBox2.UseSystemPasswordChar = !checkBox1.Checked;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string username = textBox1.Text;
            string password = textBox2.Text;

            (bool isLoggedIn, int userID, string email,string contact,string profilePicture,DateTime createdAt) = Program.AppData.userCrud.Login(username, password);

            if (isLoggedIn)
            {
                LoggedInUser.userName = username;
                LoggedInUser.userID = userID;
                LoggedInUser.email = email;
                LoggedInUser.profilePicture = profilePicture;
                LoggedInUser.createdAt = createdAt;
                LoggedInUser.contact = contact;

                MessageBox.Show(LoggedInUser.userName + " !! Your Login Is Successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                UserMenu other= new UserMenu();
                NavigationManager.NavigateTo(other, this);

            }

        }
       
    }
}
