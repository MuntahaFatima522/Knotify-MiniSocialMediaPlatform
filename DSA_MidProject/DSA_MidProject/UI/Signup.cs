using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DSA_MidProject.BL;
using DSA_MidProject.DL;
using MySql.Data.MySqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;


namespace DSA_MidProject
{
    public partial class Signup : Form
    {
        public Signup()
        {
            InitializeComponent();
            this.ActiveControl = panel1;

        }
        private void SetPlaceholder(System.Windows.Forms.TextBox txt, string placeholder, bool isPassword = false)
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
        private void LoginForm_Load(object sender, EventArgs e)
        {
            SetPlaceholder(textBox1, "Enter your username");
            SetPlaceholder(textBox2, "Enter your password", true);
            SetPlaceholder(textBox3, "Enter your email");
            SetPlaceholder(textBox4, "Enter your phone number");
            SetPlaceholder(textBox5, "Choose profile picture");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Login other = new Login();
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
            string user_name = textBox1.Text;
            if (string.IsNullOrWhiteSpace(user_name) || user_name == textBox1.Tag.ToString())
            {
                MessageBox.Show("Please enter username!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (Program.AppData.userCrud.IsUserNameExists(user_name))
            {
                MessageBox.Show("User name already exists! Please choose a different one.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox2.Focus();
                return;
            }
            string password = textBox2.Text;
            if (!UserCRUD.CheckPasswordStrength(password))
            {
                MessageBox.Show("Invalid Password! Must contain at least one letter, one number, and one special character.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string email = textBox3.Text;

            if (Program.AppData.userCrud.IsEmailExists(email))
            {
                MessageBox.Show("Email already exists! Please enter a different one.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                textBox3.Focus();
                return;
            }
            if (!email.EndsWith("@gmail.com"))
            {
                MessageBox.Show("Invalid Email! Please enter a valid Gmail address.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string phone = textBox4.Text;
            if (!UserCRUD.IsValidContact(phone))
            {
                MessageBox.Show("Invalid contact number! Please enter exactly 11 digits starting with 0 or +92.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
          
            string profilePicture = textBox5.Text;
            if (string.IsNullOrWhiteSpace(profilePicture) || profilePicture == textBox5.Tag.ToString())
            {
                MessageBox.Show("Please upload a profile picture first!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string profilePicturePath = ConvertToDatabasePath(profilePicture);

            DateTime createdAt = DateTime.Now;
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            User u = new User(user_name, email, hashedPassword, phone, profilePicturePath, createdAt);

            Program.AppData.userCrud.AddUser(u);

            string getUserQuery = $"SELECT * FROM users WHERE UserName = '{user_name}'";
            using (MySqlDataReader reader = DatabaseHelper.Instance.getData(getUserQuery))
            {
                if (reader.Read())
                {
                    LoggedInUser.userID = Convert.ToInt32(reader["UserID"]);
                    LoggedInUser.userName = reader["UserName"].ToString();
                    LoggedInUser.email = reader["Email"].ToString();
                    LoggedInUser.contact = reader["Contact"].ToString();
                    LoggedInUser.profilePicture = reader["ProfilePicture"].ToString();
                    LoggedInUser.createdAt = Convert.ToDateTime(reader["CreatedAt"]);
                }
                else
                {
                    MessageBox.Show("Error: User was not found after creation. Please login manually.", "Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            MessageBox.Show($"Welcome {user_name}! Your account has been created successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            UserMenu userMenu = new UserMenu();
            NavigationManager.NavigateTo(userMenu, this);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Select Profile Picture";
            ofd.Filter = "Image Files (*.jpg;*.jpeg;*.png;*.bmp;*.gif)|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files (*.*)|*.*";
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            ofd.CheckFileExists = true;
            ofd.CheckPathExists = true;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string selectedFilePath = ofd.FileName;

                    if (IsValidImageFile(selectedFilePath))
                    {
                        textBox5.Text = selectedFilePath;
                        textBox5.ForeColor = Color.Black;

                        MessageBox.Show("Profile picture selected successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Please select a valid image file (jpg, jpeg, png, bmp, gif).",
                            "Invalid File", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error selecting file: {ex.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private bool IsValidImageFile(string filePath)
        {
            try
            {
                string extension = Path.GetExtension(filePath).ToLower();
                string[] validExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" };

                if (!validExtensions.Contains(extension))
                    return false;

                using (Image img = Image.FromFile(filePath))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private string ConvertToDatabasePath(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return filePath;

            string databasePath = filePath.Replace("\\", "/");


            return databasePath;
        }

        
    }
}