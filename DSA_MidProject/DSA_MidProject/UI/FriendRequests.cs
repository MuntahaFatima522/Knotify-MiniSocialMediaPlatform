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
using Microsoft.VisualBasic.ApplicationServices;

namespace DSA_MidProject.UI
{
    public partial class FriendRequests : Form
    {
        private List<FriendRequest> pendingRequests;

        public FriendRequests()
        {
            InitializeComponent();
            label1.Text = LoggedInUser.userName;
            InitializePanel6();
            LoadFriendRequests();
            DisplayFriendRequests();
        }

        private void InitializePanel6()
        {
            panel6.Location = new Point(53, 91);
            panel6.Size = new Size(1012, 555);
            panel6.AutoScroll = true;
            panel6.BorderStyle = BorderStyle.FixedSingle;
            panel6.BackColor = Color.White;
        }

        private void LoadFriendRequests()
        {
            pendingRequests = Program.AppData.friendCrud.GetPendingRequestsWithDetails(LoggedInUser.userID);
        }

        private void DisplayFriendRequests()
        {
            panel6.Controls.Clear();

            if (pendingRequests == null || pendingRequests.Count == 0)
            {
                Label noRequestsLabel = new Label();
                noRequestsLabel.Text = "No pending friend requests";
                noRequestsLabel.Font = new Font("Arial", 12, FontStyle.Bold);
                noRequestsLabel.ForeColor = Color.Gray;
                noRequestsLabel.AutoSize = true;
                noRequestsLabel.TextAlign = ContentAlignment.MiddleCenter;
                noRequestsLabel.Dock = DockStyle.Fill;
                panel6.Controls.Add(noRequestsLabel);
                return;
            }

            int yPosition = 5; 
            int requestWidth = 980;
            int requestHeight = 80;
            int requestSpacing = 15; 

            foreach (FriendRequest request in pendingRequests)
            {
                Panel requestPanel = CreateRequestPanel(request, yPosition, requestWidth, requestHeight);
                panel6.Controls.Add(requestPanel);
                yPosition += requestHeight + requestSpacing;
            }

            yPosition += 20;

            panel6.AutoScrollMinSize = new Size(0, yPosition);
        }

        private Panel CreateRequestPanel(FriendRequest request, int yPosition, int width, int height)
        {
            Panel panel = new Panel();
            panel.Location = new Point(15, yPosition);
            panel.Size = new Size(width, height);
            panel.BackColor = Color.White;
            panel.BorderStyle = BorderStyle.FixedSingle;
            panel.Margin = new Padding(0, 0, 0, 10);

            PictureBox pictureBox = new PictureBox();
            pictureBox.Size = new Size(50, 50);
            pictureBox.Location = new Point(15, 15);
            pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox.BorderStyle = BorderStyle.FixedSingle;

            if (!string.IsNullOrEmpty(request.SenderProfilePicture) && System.IO.File.Exists(request.SenderProfilePicture))
            {
                try
                {
                    pictureBox.Image = Image.FromFile(request.SenderProfilePicture);
                }
                catch
                {
                    SetDefaultAvatar(pictureBox, request.SenderName);
                }
            }
            else
            {
                SetDefaultAvatar(pictureBox, request.SenderName);
            }

            Label nameLabel = new Label();
            nameLabel.Text = request.SenderName;
            nameLabel.Font = new Font("Arial", 11, FontStyle.Bold);
            nameLabel.Location = new Point(80, 15);
            nameLabel.AutoSize = true;

            Label timeLabel = new Label();
            timeLabel.Text = $"Requested: {GetTimeAgo(request.SentAt)}";
            timeLabel.Font = new Font("Arial", 9);
            timeLabel.ForeColor = Color.Gray;
            timeLabel.Location = new Point(80, 40);
            timeLabel.AutoSize = true;

            Button profileButton = new Button();
            profileButton.Text = "👤 Profile";
            profileButton.Size = new Size(140, 35);
            profileButton.Location = new Point(600, 22);
            profileButton.BackColor = Color.LightBlue;
            profileButton.ForeColor = Color.DarkBlue;
            profileButton.Font = new Font("Arial", 9, FontStyle.Bold);
            profileButton.Tag = request.SenderID;
            profileButton.Click += ProfileButton_Click;

            Button acceptButton = new Button();
            acceptButton.Text = "Accept";
            acceptButton.Size = new Size(80, 35);
            acceptButton.Location = new Point(750, 22);
            acceptButton.BackColor = Color.LimeGreen;
            acceptButton.ForeColor = Color.White;
            acceptButton.Font = new Font("Arial", 9, FontStyle.Bold);
            acceptButton.Tag = request.RequestID;
            acceptButton.Click += AcceptRequestButton_Click;

            Button rejectButton = new Button();
            rejectButton.Text = "Reject";
            rejectButton.Size = new Size(80, 35);
            rejectButton.Location = new Point(840, 22);
            rejectButton.BackColor = Color.Crimson;
            rejectButton.ForeColor = Color.White;
            rejectButton.Font = new Font("Arial", 9, FontStyle.Bold);
            rejectButton.Tag = request.RequestID;
            rejectButton.Click += RejectRequestButton_Click;

            panel.Controls.Add(pictureBox);
            panel.Controls.Add(nameLabel);
            panel.Controls.Add(timeLabel);
            panel.Controls.Add(profileButton);
            panel.Controls.Add(acceptButton);
            panel.Controls.Add(rejectButton);

            return panel;
        }

        private void SetDefaultAvatar(PictureBox pictureBox, string username)
        {
            Bitmap bmp = new Bitmap(50, 50);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                Color[] colors = { Color.LightBlue, Color.LightGreen, Color.LightCoral,
                                 Color.LightGoldenrodYellow, Color.LightSkyBlue, Color.LightPink };
                int colorIndex = Math.Abs(username.GetHashCode()) % colors.Length;

                g.Clear(Color.White);
                using (Brush brush = new SolidBrush(colors[colorIndex]))
                {
                    g.FillEllipse(brush, 0, 0, 48, 48);
                }

                using (Font font = new Font("Arial", 14, FontStyle.Bold))
                using (Brush textBrush = new SolidBrush(Color.White))
                {
                    string firstLetter = username.Length > 0 ? username[0].ToString().ToUpper() : "?";
                    SizeF textSize = g.MeasureString(firstLetter, font);
                    g.DrawString(firstLetter, font, textBrush,
                                (50 - textSize.Width) / 2, (50 - textSize.Height) / 2);
                }
            }
            pictureBox.Image = bmp;
        }

        private void ProfileButton_Click(object sender, EventArgs e)
        {
            Button profileButton = sender as Button;
            if (profileButton?.Tag is int senderID)
            {
                ShowDetailedProfile(senderID);
            }
        }

        private void ShowDetailedProfile(int senderID)
        {
            DSA_MidProject.BL.User user = Program.AppData.userCrud.Users.SearchByID(senderID);
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

        private void CreateInitialProfilePicture(PictureBox pictureBox, string username)
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

        private string GetTimeAgo(DateTime requestTime)
        {
            TimeSpan timeSince = DateTime.Now - requestTime;

            if (timeSince.TotalMinutes < 1) return "just now";
            if (timeSince.TotalMinutes < 60) return $"{(int)timeSince.TotalMinutes}m ago";
            if (timeSince.TotalHours < 24) return $"{(int)timeSince.TotalHours}h ago";
            if (timeSince.TotalDays < 7) return $"{(int)timeSince.TotalDays}d ago";
            return requestTime.ToString("MMM dd, yyyy");
        }

        private void AcceptRequestButton_Click(object sender, EventArgs e)
        {
            Button acceptButton = (Button)sender;
            int requestID = (int)acceptButton.Tag;

            DialogResult result = MessageBox.Show(
                "Are you sure you want to accept this friend request?",
                "Confirm Acceptance",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                bool success = Program.AppData.friendCrud.AcceptFriendRequest(requestID, LoggedInUser.userID);
                if (success)
                {
                    MessageBox.Show("Friend request accepted!", "Success",
                                 MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadFriendRequests();
                    DisplayFriendRequests();
                }
            }
        }

        private void RejectRequestButton_Click(object sender, EventArgs e)
        {
            Button rejectButton = (Button)sender;
            int requestID = (int)rejectButton.Tag;

            DialogResult result = MessageBox.Show(
                "Are you sure you want to reject this friend request?",
                "Confirm Rejection",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result == DialogResult.Yes)
            {
                bool success = Program.AppData.friendCrud.RejectFriendRequest(requestID, LoggedInUser.userID);
                if (success)
                {
                    MessageBox.Show("Friend request rejected.", "Success",
                                 MessageBoxButtons.OK, MessageBoxIcon.Information);

                    LoadFriendRequests();
                    DisplayFriendRequests();
                }
            }
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
            this.Hide();
            other.ShowDialog();
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