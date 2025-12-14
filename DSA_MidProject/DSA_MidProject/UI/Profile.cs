using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DSA_MidProject.BL;
using DSA_MidProject.DL;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace DSA_MidProject.UI
{
    public partial class Profile : Form
    {
        private string currentProfilePicturePath;

        public Profile()
        {
            InitializeComponent();
            LoadUserData();
        }

        private void LoadUserData()
        {
            label1.Text = LoggedInUser.userName;
            textBox1.Text = LoggedInUser.userName;
            textBox2.Text = LoggedInUser.email;
            textBox3.Text = LoggedInUser.contact;
            label14.Text = LoggedInUser.createdAt.ToString("MMMM dd, yyyy");

            currentProfilePicturePath = ConvertToSystemPath(LoggedInUser.profilePicture);
            LoadProfilePicture(currentProfilePicturePath);

            textBox1.Enabled = false;
            textBox2.Enabled = false;
            textBox3.Enabled = false;
        }

        private void LoadProfilePicture(string imagePath)
        {
            try
            {
                if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
                {
                    pictureBox2.Image = Image.FromFile(imagePath);
                    pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
                }
                else
                {
                    SetDefaultProfilePicture();
                }
            }
            catch (Exception ex)
            {
                SetDefaultProfilePicture();
                Console.WriteLine($"Error loading profile picture: {ex.Message}");
            }
        }

        private void SetDefaultProfilePicture()
        {
            Bitmap defaultImage = new Bitmap(150, 150);
            using (Graphics g = Graphics.FromImage(defaultImage))
            {
                g.Clear(Color.LightGray);
                using (Font font = new Font("Arial", 10))
                using (Brush brush = new SolidBrush(Color.DarkGray))
                {
                    g.DrawString("No Profile\nPicture", font, brush, 40, 55);
                }
                g.DrawRectangle(new Pen(Color.DarkGray, 2), 60, 30, 30, 25);
                g.FillEllipse(new SolidBrush(Color.DarkGray), 65, 55, 20, 20);
            }
            pictureBox2.Image = defaultImage;
            pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private string ConvertToSystemPath(string databasePath)
        {
            if (string.IsNullOrEmpty(databasePath))
                return databasePath;

            return databasePath.Replace("/", "\\");
        }

        private string ConvertToDatabasePath(string systemPath)
        {
            if (string.IsNullOrEmpty(systemPath))
                return systemPath;

            return systemPath.Replace("\\", "/");
        }

        private void RefreshForm()
        {
            LoadUserData();
        }

        private void label2_Click(object sender, EventArgs e)
        {
            UserMenu other = new UserMenu();
            NavigationManager.NavigateTo(other, this);

        }

        private void label5_Click(object sender, EventArgs e)
        {
            SortedFeed other = new SortedFeed();
            NavigationManager.NavigateTo(other, this);

        }

        private void label4_Click(object sender, EventArgs e)
        {
            SearchFeed other = new SearchFeed();
            NavigationManager.NavigateTo(other, this);

        }

        private void label3_Click(object sender, EventArgs e)
        {
            CreatePost other = new CreatePost();
            NavigationManager.NavigateTo(other, this);

        }

        private void label7_Click(object sender, EventArgs e)
        {
            ManagePost other = new ManagePost();
            NavigationManager.NavigateTo(other, this);

        }

        private void label9_Click(object sender, EventArgs e)
        {
            Friends other = new Friends();
            NavigationManager.NavigateTo(other, this);

        }

        private void label8_Click(object sender, EventArgs e)
        {
            Profile other = new Profile();
            NavigationManager.NavigateTo(other, this);

        }
        private void label6_Click(object sender, EventArgs e)
        {
            NavigationManager.GoBack(this);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int user_id = LoggedInUser.userID;
            string user_name = textBox1.Text;

            if (user_name != LoggedInUser.userName)
            {
                if (Program.AppData.userCrud.IsUserNameExists(user_name))
                {
                    MessageBox.Show("User name already exists! Please choose a different one.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox1.Focus();
                    return;
                }
            }

            string email = textBox2.Text;
            if (email != LoggedInUser.email)
            {
                if (Program.AppData.userCrud.IsEmailExists(email))
                {
                    MessageBox.Show("Email already exists! Please enter a different one.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    textBox2.Focus();
                    return;
                }
            }

            if (!email.EndsWith("@gmail.com"))
            {
                MessageBox.Show("Invalid Email! Please enter a valid Gmail address.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string phone = textBox3.Text;
            if (!UserCRUD.IsValidContact(phone))
            {
                MessageBox.Show("Invalid contact number! Please enter exactly 11 digits starting with 0 or +92.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string profilePictureForDb = ConvertToDatabasePath(currentProfilePicturePath);

            User updatedUser = new User(user_id, user_name, email, phone, profilePictureForDb);
            Program.AppData.userCrud.UpdateUser(updatedUser);
            MessageBox.Show("Your details have been updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            RefreshForm();

        }

        private void button5_Click(object sender, EventArgs e)
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
                        currentProfilePicturePath = selectedFilePath;
                        LoadProfilePicture(currentProfilePicturePath);

                        MessageBox.Show("Profile picture updated! Click 'Apply Changes' to save changes permanently.",
                            "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Enabled = !textBox1.Enabled;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox2.Enabled = !textBox2.Enabled;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox3.Enabled = !textBox3.Enabled;

        }

        

    }
}