using System;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using DSA_MidProject.BL;
using DSA_MidProject.DL;

namespace DSA_MidProject.UI
{
    public partial class CreatePost : Form
    {
        private const string PLACEHOLDER_TEXT = "Share your thoughts....";

        public CreatePost()
        {
            InitializeComponent();
            label1.Text = LoggedInUser.userName;
            InitializeCharCounter();
            SetPlaceholder(contentTextBox, PLACEHOLDER_TEXT);
            InitializeStatisticsPanel();
            this.ActiveControl = label6;
        }

        private void InitializeStatisticsPanel()
        {
            RefreshStatistics();
        }

        private void RefreshStatistics()
        {
            try
            {
                label27.Text = GetUserPostCount().ToString(); 
                label25.Text = GetMonthlyPostCount().ToString(); 
                label26.Text = GetTotalLikes().ToString();
                label24.Text = GetTotalComments().ToString(); 
                label23.Text = GetAverageLikes().ToString(); 
                label22.Text = GetMostLikedPostLikes().ToString() + " likes"; 
                label21.Text = GetEngagementRate().ToString("F1") + "%";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading statistics: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetUserPostCount()
        {
            try
            {
                var allPosts = Program.AppData.postCrud.GetAllPosts();
                return allPosts.Count(p => p.UserID == LoggedInUser.userID && !p.IsDeleted);
            }
            catch
            {
                return 0;
            }
        }

        private int GetMonthlyPostCount()
        {
            try
            {
                var allPosts = Program.AppData.postCrud.GetAllPosts();
                return allPosts.Count(p => p.UserID == LoggedInUser.userID &&
                                        p.CreatedAt.Month == DateTime.Now.Month &&
                                        p.CreatedAt.Year == DateTime.Now.Year &&
                                        !p.IsDeleted);
            }
            catch
            {
                return 0;
            }
        }

        private int GetTotalLikes()
        {
            try
            {
                var userPosts = Program.AppData.postCrud.GetAllPosts()
                    .Where(p => p.UserID == LoggedInUser.userID && !p.IsDeleted);
                return userPosts.Sum(p => Program.AppData.likeCrud.GetLikeCount(p.PostID));
            }
            catch
            {
                return 0;
            }
        }

        private int GetTotalComments()
        {
            try
            {
                var userPosts = Program.AppData.postCrud.GetAllPosts()
                    .Where(p => p.UserID == LoggedInUser.userID && !p.IsDeleted);
                return userPosts.Sum(p => Program.AppData.commentCRUD.GetCommentCount(p.PostID));
            }
            catch
            {
                return 0;
            }
        }

        private double GetAverageLikes()
        {
            try
            {
                var userPosts = Program.AppData.postCrud.GetAllPosts()
                    .Where(p => p.UserID == LoggedInUser.userID && !p.IsDeleted).ToList();
                if (userPosts.Count == 0) return 0;
                
                int totalLikes = GetTotalLikes();
                return Math.Round((double)totalLikes / userPosts.Count, 1);
            }
            catch
            {
                return 0;
            }
        }

        private int GetMostLikedPostLikes()
        {
            try
            {
                var userPosts = Program.AppData.postCrud.GetAllPosts()
                    .Where(p => p.UserID == LoggedInUser.userID && !p.IsDeleted).ToList();
                if (userPosts.Count == 0) return 0;
                
                return userPosts.Max(p => Program.AppData.likeCrud.GetLikeCount(p.PostID));
            }
            catch
            {
                return 0;
            }
        }

        private double GetEngagementRate()
        {
            try
            {
                int totalPosts = GetUserPostCount();
                if (totalPosts == 0) return 0;

                int totalInteractions = GetTotalLikes() + GetTotalComments();
                return Math.Round((totalInteractions / (double)totalPosts) * 100, 1);
            }
            catch
            {
                return 0;
            }
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
            return contentTextBox.Text == PLACEHOLDER_TEXT || string.IsNullOrWhiteSpace(contentTextBox.Text);
        }

        private string GetActualContent()
        {
            return IsPlaceholderActive() ? "" : contentTextBox.Text.Trim();
        }

        private void InitializeCharCounter()
        {
            charCountLabel = new Label();
            charCountLabel.Text = "0/500 characters";
            charCountLabel.Font = new Font("Arial", 9, FontStyle.Italic);
            charCountLabel.ForeColor = Color.Gray;
            charCountLabel.Location = new Point(contentTextBox.Left, contentTextBox.Bottom + 5);
            charCountLabel.AutoSize = true;
            this.Controls.Add(charCountLabel);

            contentTextBox.TextChanged += ContentTextBox_TextChanged;
        }

        private void ContentTextBox_TextChanged(object sender, EventArgs e)
        {
            string actualContent = GetActualContent();
            int currentLength = actualContent.Length;
            charCountLabel.Text = $"{currentLength}/500 characters";

            if (currentLength > 500)
            {
                charCountLabel.ForeColor = Color.Red;
                createButton.Enabled = false;
            }
            else if (currentLength > 450)
            {
                charCountLabel.ForeColor = Color.Orange;
                createButton.Enabled = true;
            }
            else
            {
                charCountLabel.ForeColor = IsPlaceholderActive() ? Color.Gray : Color.Green;
                createButton.Enabled = true;
            }

            createButton.Enabled = currentLength > 0 && currentLength <= 500;
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            contentTextBox.Clear();
            SetPlaceholder(contentTextBox, PLACEHOLDER_TEXT);
            createButton.Enabled = false;
            charCountLabel.Text = "0/500 characters";
            charCountLabel.ForeColor = Color.Gray;
        }

        private void CreateButton_Click(object sender, EventArgs e)
        {
            string content = GetActualContent();

            if (string.IsNullOrEmpty(content) || IsPlaceholderActive())
            {
                MessageBox.Show("Please enter some content for your post!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (content.Length > 500)
            {
                MessageBox.Show("Post content cannot exceed 500 characters! Current length: " + content.Length,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            createButton.Enabled = false;
            createButton.Text = "Creating...";

            try
            {
                bool success = Program.AppData.postCrud.CreatePost(LoggedInUser.userID, content);

                if (success)
                {
                    MessageBox.Show("Post created successfully! 🎉", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    contentTextBox.Clear();
                    SetPlaceholder(contentTextBox, PLACEHOLDER_TEXT);
                    charCountLabel.Text = "0/500 characters";
                    charCountLabel.ForeColor = Color.Gray;

                    Program.AppData.postCrud.LoadPostsFromDB();
                    RefreshStatistics();
                }
                else
                {
                    MessageBox.Show("Failed to create post. Please try again.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating post: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                string currentContent = GetActualContent();
                createButton.Enabled = currentContent.Length > 0 && currentContent.Length <= 500;
                createButton.Text = "Create";
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
            NavigationManager.NavigateTo(other, this);
        }

        private void label6_Click(object sender, EventArgs e)
        {
            NavigationManager.GoBack(this);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            RefreshStatistics();
        }
    }
}