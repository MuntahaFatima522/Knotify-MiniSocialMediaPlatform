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

namespace DSA_MidProject.UI
{
    public partial class Friends : Form
    {
        private List<FriendUser> allFriends;
        private List<FriendUser> currentDisplayedFriends;
        private const string PLACEHOLDER_TEXT = "Search Friend...";

        public Friends()
        {
            InitializeComponent();
            InitializeCustomComponents();
            label1.Text = LoggedInUser.userName;
            LoadFriendsData();
            DisplayFriends();
        }

        private void InitializeCustomComponents()
        {
            panel6.Location = new Point(55, 127);
            panel6.Size = new Size(1010, 518);
            panel6.AutoScroll = true;
            panel6.BorderStyle = BorderStyle.FixedSingle;
            panel6.BackColor = Color.White;

            InitializePlaceholder();
        }

        private void InitializePlaceholder()
        {
            SetPlaceholder(textBox1, PLACEHOLDER_TEXT);
            this.ActiveControl = label6; 
        }

        private void SetPlaceholder(TextBox txt, string placeholder)
        {
            txt.Text = placeholder;
            txt.ForeColor = Color.Gray;

            txt.GotFocus += (s, e) =>
            {
                if (txt.Text == placeholder)
                {
                    txt.Text = "";
                    txt.ForeColor = Color.Black;
                }
            };

            txt.LostFocus += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txt.Text))
                {
                    txt.Text = placeholder;
                    txt.ForeColor = Color.Gray;
                }
            };
        }

        private bool IsPlaceholderActive()
        {
            return textBox1.Text == PLACEHOLDER_TEXT || string.IsNullOrWhiteSpace(textBox1.Text);
        }

        private string GetActualSearchText()
        {
            return IsPlaceholderActive() ? "" : textBox1.Text.Trim();
        }

        private void LoadFriendsData()
        {
            allFriends = Program.AppData.friendCrud.GetFriendsWithDetails(LoggedInUser.userID);
            currentDisplayedFriends = allFriends;
        }

        private void DisplayFriends()
        {
            panel6.Controls.Clear();

            if (currentDisplayedFriends == null || currentDisplayedFriends.Count == 0)
            {
                Label noFriendsLabel = new Label();
                noFriendsLabel.Text = "No friends found";
                noFriendsLabel.Font = new Font("Arial", 12, FontStyle.Bold);
                noFriendsLabel.ForeColor = Color.Gray;
                noFriendsLabel.AutoSize = true;
                noFriendsLabel.TextAlign = ContentAlignment.MiddleCenter;
                noFriendsLabel.Dock = DockStyle.Fill;
                panel6.Controls.Add(noFriendsLabel);
                return;
            }

            int yPosition = 10; 
            int friendPanelWidth = 980;
            int friendPanelHeight = 70; 
            int spacing = 10; 

            foreach (FriendUser friend in currentDisplayedFriends)
            {
                Panel friendPanel = CreateFriendPanel(friend, yPosition, friendPanelWidth, friendPanelHeight);
                panel6.Controls.Add(friendPanel);
                yPosition += friendPanelHeight + spacing;
            }

            yPosition += 10;
            panel6.AutoScrollMinSize = new Size(0, yPosition);
        }

        private Panel CreateFriendPanel(FriendUser friend, int yPosition, int width, int height)
        {
            Panel panel = new Panel();
            panel.Location = new Point(15, yPosition);
            panel.Size = new Size(width, height);
            panel.BackColor = Color.White;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Margin = new Padding(0, 0, 0, 5);

            PictureBox pictureBox = new PictureBox();
            pictureBox.Size = new Size(50, 50);
            pictureBox.Location = new Point(15, 10);
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;

            if (!string.IsNullOrEmpty(friend.ProfilePicture) && System.IO.File.Exists(friend.ProfilePicture))
            {
                try
                {
                    pictureBox.Image = Image.FromFile(friend.ProfilePicture);
                }
                catch
                {
                    SetDefaultAvatar(pictureBox, friend.Username);
                }
            }
            else
            {
                SetDefaultAvatar(pictureBox, friend.Username);
            }

            Label nameLabel = new Label();
            nameLabel.Text = friend.Username;
            nameLabel.Font = new Font("Arial", 11, FontStyle.Bold);
            nameLabel.Location = new Point(80, 12);
            nameLabel.AutoSize = true;

            Label dateLabel = new Label();
            dateLabel.Text = $"Friends since: {friend.BecameFriendsAt:MMM dd, yyyy}";
            dateLabel.Font = new Font("Arial", 9);
            dateLabel.ForeColor = Color.DarkBlue;
            dateLabel.Location = new Point(80, 38);
            dateLabel.AutoSize = true;

            Button viewButton = new Button();
            viewButton.Text = "👤 View";
            viewButton.Size = new Size(120, 35);
            viewButton.Location = new Point(700, 17);
            viewButton.BackColor = Color.LightBlue;
            viewButton.ForeColor = Color.DarkBlue;
            viewButton.Font = new Font("Arial", 9, FontStyle.Bold);
            viewButton.Tag = friend.UserID;
            viewButton.Click += ViewButton_Click;

            Button removeButton = new Button();
            removeButton.Text = "Remove";
            removeButton.Size = new Size(120, 35);
            removeButton.Location = new Point(830, 17);
            removeButton.BackColor = Color.Crimson;
            removeButton.ForeColor = Color.White;
            removeButton.Font = new Font("Arial", 9, FontStyle.Bold);
            removeButton.Tag = friend.UserID;
            removeButton.Click += RemoveFriendButton_Click;

            panel.Controls.Add(pictureBox);
            panel.Controls.Add(nameLabel);
            panel.Controls.Add(dateLabel);
            panel.Controls.Add(viewButton);
            panel.Controls.Add(removeButton);

            return panel;
        }

        private void ViewButton_Click(object sender, EventArgs e)
        {
            Button viewButton = sender as Button;
            if (viewButton?.Tag is int userID)
            {
                ShowUserProfile(userID);
            }
        }

        private void ShowUserProfile(int userID)
        {
            User user = Program.AppData.userCrud.Users.SearchByID(userID);
            if (user != null)
            {
                var mutualFriends = GetMutualFriends(user.UserID);
                int mutualFriendsCount = mutualFriends.Count;

                Form profileForm = new Form();
                profileForm.Text = $"User Profile - {user.UserName}";
                profileForm.Size = new Size(450, 300);
                profileForm.StartPosition = FormStartPosition.CenterParent;
                profileForm.FormBorderStyle = FormBorderStyle.FixedDialog;
                profileForm.MaximizeBox = false;
                profileForm.MinimizeBox = false;
                profileForm.BackColor = Color.White;
                profileForm.AutoScroll = true;
                profileForm.Padding = new Padding(20);

                Panel mainPanel = new Panel();
                mainPanel.Location = new Point(20, 20);
                mainPanel.Size = new Size(390, 330);
                mainPanel.BackColor = Color.White;
                mainPanel.AutoScroll = true;
                profileForm.Controls.Add(mainPanel);

                PictureBox profilePic = new PictureBox();
                profilePic.Size = new Size(60, 60);
                profilePic.Location = new Point(20, 20);
                profilePic.SizeMode = PictureBoxSizeMode.Zoom;
                profilePic.BorderStyle = BorderStyle.FixedSingle;
                profilePic.BackColor = Color.LightGray;

                if (!string.IsNullOrEmpty(user.ProfilePicture) && File.Exists(user.ProfilePicture))
                {
                    try
                    {
                        profilePic.Image = Image.FromFile(user.ProfilePicture);
                    }
                    catch
                    {
                        CreateInitialProfilePicture(profilePic, user.UserName);
                    }
                }
                else
                {
                    CreateInitialProfilePicture(profilePic, user.UserName);
                }
                mainPanel.Controls.Add(profilePic);

                int infoX = 100;
                int infoY = 20;

                Label nameLabel = new Label();
                nameLabel.Text = user.UserName;
                nameLabel.Font = new Font("Arial", 12, FontStyle.Bold);
                nameLabel.Location = new Point(infoX, infoY);
                nameLabel.AutoSize = true;
                mainPanel.Controls.Add(nameLabel);

                Label emailLabel = new Label();
                emailLabel.Text = user.Email;
                emailLabel.Font = new Font("Arial", 9, FontStyle.Regular);
                emailLabel.Location = new Point(infoX, infoY + 25);
                emailLabel.AutoSize = true;
                mainPanel.Controls.Add(emailLabel);

                Label joinDateLabel = new Label();
                joinDateLabel.Text = $"Joined: {user.CreatedAt:MMMM dd, yyyy}";
                joinDateLabel.Font = new Font("Arial", 8, FontStyle.Italic);
                joinDateLabel.ForeColor = Color.Gray;
                joinDateLabel.Location = new Point(infoX, infoY + 45);
                joinDateLabel.AutoSize = true;
                mainPanel.Controls.Add(joinDateLabel);

                int mutualY = 130;

                Label mutualFriendsHeader = new Label();
                mutualFriendsHeader.Font = new Font("Arial", 10, FontStyle.Bold);
                mutualFriendsHeader.Location = new Point(20, mutualY);
                mutualFriendsHeader.AutoSize = true;

                if (mutualFriendsCount > 0)
                {
                    mutualFriendsHeader.Text = $"🤝 {mutualFriendsCount} mutual friend{(mutualFriendsCount > 1 ? "s" : "")}:";
                    mutualFriendsHeader.ForeColor = Color.Green;
                }
                else
                {
                    mutualFriendsHeader.Text = "No mutual friends";
                    mutualFriendsHeader.ForeColor = Color.Red;
                }
                mainPanel.Controls.Add(mutualFriendsHeader);

                mutualY += 35;

                if (mutualFriendsCount > 0)
                {
                    foreach (var mutualFriend in mutualFriends)
                    {
                        Panel mutualFriendPanel = CreateMutualFriendPanel(mutualFriend, mutualY, 350);
                        mainPanel.Controls.Add(mutualFriendPanel);
                        mutualY += 40;
                    }
                }

                profileForm.ShowDialog();
            }
        }
        private List<FriendUser> GetMutualFriends(int targetUserID)
        {
            var currentUserFriends = Program.AppData.friendCrud.GetFriendsWithDetails(LoggedInUser.userID);

            var targetUserFriends = Program.AppData.friendCrud.GetFriendsWithDetails(targetUserID);

            var mutualFriends = currentUserFriends
                .Where(f1 => targetUserFriends.Any(f2 => f2.UserID == f1.UserID))
                .ToList();

            return mutualFriends;
        }

        private Panel CreateMutualFriendPanel(FriendUser mutualFriend, int yPosition, int width)
        {
            Panel panel = new Panel();
            panel.Location = new Point(20, yPosition);
            panel.Size = new Size(width, 30);
            panel.BorderStyle = BorderStyle.None;
            panel.BackColor = Color.WhiteSmoke;

            PictureBox mutualPic = new PictureBox();
            mutualPic.Size = new Size(25, 25);
            mutualPic.Location = new Point(5, 2);
            mutualPic.SizeMode = PictureBoxSizeMode.Zoom;
            mutualPic.BorderStyle = BorderStyle.FixedSingle;

            if (!string.IsNullOrEmpty(mutualFriend.ProfilePicture) && File.Exists(mutualFriend.ProfilePicture))
            {
                try
                {
                    mutualPic.Image = Image.FromFile(mutualFriend.ProfilePicture);
                }
                catch
                {
                    SetDefaultAvatar(mutualPic, mutualFriend.Username);
                }
            }
            else
            {
                SetDefaultAvatar(mutualPic, mutualFriend.Username);
            }
            panel.Controls.Add(mutualPic);

            Label mutualName = new Label();
            mutualName.Text = mutualFriend.Username;
            mutualName.Font = new Font("Arial", 9, FontStyle.Regular);
            mutualName.Location = new Point(35, 5);
            mutualName.AutoSize = true;
            panel.Controls.Add(mutualName);

            return panel;
        }

        private void SetDefaultAvatar(PictureBox pictureBox, string username)
        {
            Bitmap bmp = new Bitmap(pictureBox.Width, pictureBox.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                Color[] colors = { Color.LightBlue, Color.LightGreen, Color.LightCoral,
                                 Color.LightGoldenrodYellow, Color.LightSkyBlue, Color.LightPink };
                int colorIndex = Math.Abs(username.GetHashCode()) % colors.Length;

                g.Clear(Color.White);
                using (Brush brush = new SolidBrush(colors[colorIndex]))
                {
                    g.FillEllipse(brush, 0, 0, pictureBox.Width - 2, pictureBox.Height - 2);
                }

                using (Font font = new Font("Arial", 14, FontStyle.Bold))
                using (Brush textBrush = new SolidBrush(Color.White))
                {
                    string firstLetter = username.Length > 0 ? username[0].ToString().ToUpper() : "?";
                    SizeF textSize = g.MeasureString(firstLetter, font);
                    g.DrawString(firstLetter, font, textBrush,
                                (pictureBox.Width - textSize.Width) / 2,
                                (pictureBox.Height - textSize.Height) / 2);
                }
            }
            pictureBox.Image = bmp;
        }

        private void CreateInitialProfilePicture(PictureBox pictureBox, string username)
        {
            Bitmap bmp = new Bitmap(pictureBox.Width, pictureBox.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                Rectangle rect = new Rectangle(0, 0, pictureBox.Width, pictureBox.Height);
                using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    rect, Color.LightBlue, Color.DarkBlue, 45f))
                {
                    g.FillEllipse(brush, rect);
                }

                using (Font font = new Font("Arial", 16, FontStyle.Bold))
                using (Brush textBrush = new SolidBrush(Color.White))
                {
                    string initial = username.Length > 0 ? username[0].ToString().ToUpper() : "U";
                    SizeF textSize = g.MeasureString(initial, font);
                    g.DrawString(initial, font, textBrush,
                                (pictureBox.Width - textSize.Width) / 2,
                                (pictureBox.Height - textSize.Height) / 2);
                }
            }
            pictureBox.Image = bmp;
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            string searchTerm = GetActualSearchText();

            if (string.IsNullOrEmpty(searchTerm))
            {
                currentDisplayedFriends = allFriends;
            }
            else
            {
                currentDisplayedFriends = Program.AppData.friendCrud.SearchFriends(allFriends, searchTerm);
            }

            DisplayFriends();
        }

        private void SortButton_Click(object sender, EventArgs e)
        {
            currentDisplayedFriends = Program.AppData.friendCrud.SortFriendsByUsername(currentDisplayedFriends, true);
            DisplayFriends();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            currentDisplayedFriends = Program.AppData.friendCrud.SortFriendsByDate(currentDisplayedFriends, false);
            DisplayFriends();
        }

        private void RemoveFriendButton_Click(object sender, EventArgs e)
        {
            Button removeButton = (Button)sender;
            int friendID = (int)removeButton.Tag;

            DialogResult result = MessageBox.Show(
                "Are you sure you want to remove this friend?",
                "Confirm Removal",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                bool success = Program.AppData.friendCrud.RemoveFriend(LoggedInUser.userID, friendID);
                if (success)
                {
                    MessageBox.Show("Friend removed successfully!", "Success",
                                 MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadFriendsData();
                    DisplayFriends();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text = PLACEHOLDER_TEXT;
            textBox1.ForeColor = Color.Gray;
            currentDisplayedFriends = allFriends;
            DisplayFriends();
            this.ActiveControl = label6;
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

        private void label12_Click(object sender, EventArgs e)
        {
            Friends other = new Friends();
            NavigationManager.NavigateTo(other, this);
        }

        private void label11_Click(object sender, EventArgs e)
        {
            FindFriends other = new FindFriends();
            NavigationManager.NavigateTo(other, this);
        }

        private void label10_Click(object sender, EventArgs e)
        {
            FriendRequests other = new FriendRequests();
            NavigationManager.NavigateTo(other, this);
        }

        private void label6_Click(object sender, EventArgs e)
        {
            NavigationManager.GoBack(this);
        }

     
    }
}