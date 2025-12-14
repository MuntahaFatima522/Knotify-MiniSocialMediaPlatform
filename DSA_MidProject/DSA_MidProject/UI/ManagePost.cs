using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DSA_MidProject.BL;
using DSA_MidProject.DataStructures;
using DSA_MidProject.DL;

namespace DSA_MidProject.UI
{
    public partial class ManagePost : Form
    {
        private List<Post> userPosts;
        private Label noPostsLabel;

        public ManagePost()
        {
            InitializeComponent();
            label1.Text = LoggedInUser.userName;
            InitializeCustomComponents();
            this.ActiveControl = label6;
            LoadUserPosts();

            undoButton.Visible = false;
        }

        private void InitializeCustomComponents()
        {
            noPostsLabel = new Label();
            noPostsLabel.Text = "You haven't created any posts yet.\nClick 'Create Post' to share your first post!";
            noPostsLabel.Font = new Font("Arial", 12, FontStyle.Italic);
            noPostsLabel.ForeColor = Color.Gray;
            noPostsLabel.TextAlign = ContentAlignment.MiddleCenter;
            noPostsLabel.AutoSize = false;
            noPostsLabel.Size = new Size(400, 60);
            noPostsLabel.Location = new Point(350, 250);
            noPostsLabel.Visible = false;
            this.Controls.Add(noPostsLabel);
        }

        private void LoadUserPosts()
        {
            var allPosts = Program.AppData.postCrud.PostQueue.ToList();
            userPosts = allPosts.Where(p => p.UserID == LoggedInUser.userID && !p.IsDeleted).ToList();

            DisplayAllPosts();
        }

        private void DisplayAllPosts()
        {
            postsPanel.Controls.Clear();

            if (userPosts == null || userPosts.Count == 0)
            {
                noPostsLabel.Visible = true;
                postsPanel.Visible = false;
                return;
            }

            noPostsLabel.Visible = false;
            postsPanel.Visible = true;

            int yPosition = 5;
            int postWidth = 950;
            int postHeight = 250;

            foreach (var post in userPosts)
            {
                Panel postPanel = CreatePostPanel(post, yPosition, postWidth, postHeight);
                postsPanel.Controls.Add(postPanel);
                yPosition += postHeight + 20;
            }
            yPosition += 10;

            postsPanel.AutoScrollMinSize = new Size(0, yPosition);
        }

        private Panel CreatePostPanel(Post post, int yPosition, int width, int height)
        {
            Panel postPanel = new Panel();
            postPanel.Location = new Point(20, yPosition);
            postPanel.Size = new Size(width, height);
            postPanel.BorderStyle = BorderStyle.FixedSingle;
            postPanel.BackColor = Color.White;
            postPanel.Padding = new Padding(10);

            int contentY = 15;

            Panel headerPanel = new Panel();
            headerPanel.Location = new Point(15, contentY);
            headerPanel.Size = new Size(width - 50, 40);
            headerPanel.BorderStyle = BorderStyle.None;
            headerPanel.BackColor = Color.White;
            postPanel.Controls.Add(headerPanel);

            PictureBox profilePicture = new PictureBox();
            profilePicture.Size = new Size(35, 35);
            profilePicture.Location = new Point(0, 2);
            profilePicture.SizeMode = PictureBoxSizeMode.Zoom;
            profilePicture.BorderStyle = BorderStyle.FixedSingle;
            profilePicture.BackColor = Color.LightGray;

            string profilePicturePath = GetUserProfilePicture(LoggedInUser.userID);
            if (!string.IsNullOrEmpty(profilePicturePath) && File.Exists(profilePicturePath))
            {
                try
                {
                    profilePicture.Image = Image.FromFile(profilePicturePath);
                }
                catch
                {
                    CreateInitialProfilePicture(profilePicture, LoggedInUser.userName);
                }
            }
            else
            {
                CreateInitialProfilePicture(profilePicture, LoggedInUser.userName);
            }
            headerPanel.Controls.Add(profilePicture);

            Label userLabel = new Label();
            userLabel.Text = LoggedInUser.userName;
            userLabel.Font = new Font("Arial", 11, FontStyle.Bold);
            userLabel.ForeColor = Color.DarkBlue;
            userLabel.Location = new Point(45, 5);
            userLabel.AutoSize = true;
            headerPanel.Controls.Add(userLabel);

            Label dateLabel = new Label();
            dateLabel.Text = "• " + post.CreatedAt.ToString("MMM dd, yyyy 'at' hh:mm tt");
            dateLabel.Font = new Font("Arial", 9, FontStyle.Regular);
            dateLabel.ForeColor = Color.Gray;
            dateLabel.Location = new Point(userLabel.Right + 10, 7);
            dateLabel.AutoSize = true;
            headerPanel.Controls.Add(dateLabel);

            contentY += 50;

            Panel contentPanel = new Panel();
            contentPanel.Location = new Point(15, contentY);
            contentPanel.Size = new Size(width - 50, 80);
            contentPanel.BorderStyle = BorderStyle.None;
            contentPanel.BackColor = Color.White;
            postPanel.Controls.Add(contentPanel);

            TextBox contentLabel = new TextBox();
            contentLabel.Text = post.Content;
            contentLabel.Font = new Font("Arial", 11, FontStyle.Regular);
            contentLabel.ForeColor = Color.Black;
            contentLabel.Location = new Point(0, 0);
            contentLabel.Size = new Size(width - 50, 80);
            contentLabel.Multiline = true;
            contentLabel.ScrollBars = ScrollBars.Vertical;
            contentLabel.BorderStyle = BorderStyle.None;
            contentLabel.BackColor = Color.White;
            contentLabel.ReadOnly = true;
            contentLabel.WordWrap = true;
            contentPanel.Controls.Add(contentLabel);

            contentY += 90;

            Panel statsPanel = new Panel();
            statsPanel.Location = new Point(15, contentY);
            statsPanel.Size = new Size(width - 50, 35);
            statsPanel.BorderStyle = BorderStyle.None;
            statsPanel.BackColor = Color.White;
            postPanel.Controls.Add(statsPanel);

            int actualLikeCount = Program.AppData.likeCrud.GetLikeCount(post.PostID);
            int actualCommentCount = Program.AppData.commentCRUD.GetCommentCount(post.PostID);

            Button likesButton = new Button();
            likesButton.Text = $"❤️ {actualLikeCount} Likes";
            likesButton.Tag = post;
            likesButton.Size = new Size(120, 30);
            likesButton.Location = new Point(0, 0);
            likesButton.BackColor = Color.White;
            likesButton.ForeColor = Color.DarkRed;
            likesButton.FlatStyle = FlatStyle.Flat;
            likesButton.Font = new Font("Arial", 9, FontStyle.Bold);
            likesButton.Click += LikesButton_Click;
            statsPanel.Controls.Add(likesButton);

            Button commentsButton = new Button();
            commentsButton.Text = $"💬 {actualCommentCount} Comments";
            commentsButton.Tag = post;
            commentsButton.Size = new Size(140, 30);
            commentsButton.Location = new Point(130, 0);
            commentsButton.BackColor = Color.White;
            commentsButton.ForeColor = Color.DarkBlue;
            commentsButton.FlatStyle = FlatStyle.Flat;
            commentsButton.Font = new Font("Arial", 9, FontStyle.Bold);
            commentsButton.Click += CommentsButton_Click;
            statsPanel.Controls.Add(commentsButton);

            contentY += 45;

            Panel actionPanel = new Panel();
            actionPanel.Location = new Point(15, contentY);
            actionPanel.Size = new Size(width - 50, 40);
            actionPanel.BorderStyle = BorderStyle.None;
            actionPanel.BackColor = Color.White;
            postPanel.Controls.Add(actionPanel);

            Button likeButton = new Button();
            likeButton.Tag = post;
            likeButton.Size = new Size(80, 30);
            likeButton.Location = new Point(0, 0);
            likeButton.FlatStyle = FlatStyle.Flat;
            likeButton.Font = new Font("Arial", 9, FontStyle.Bold);

            bool hasLiked = Program.AppData.likeCrud.HasUserLikedPost(post.PostID, LoggedInUser.userID);
            UpdateLikeButtonAppearance(likeButton, hasLiked);
            actionPanel.Controls.Add(likeButton);

            Button editButton = new Button();
            editButton.Text = "✏️ Edit";
            editButton.Tag = post;
            editButton.Size = new Size(80, 30);
            editButton.Location = new Point(90, 0);
            editButton.BackColor = Color.LightGreen;
            editButton.ForeColor = Color.DarkGreen;
            editButton.FlatStyle = FlatStyle.Flat;
            editButton.Font = new Font("Arial", 9, FontStyle.Bold);
            editButton.Click += EditButton_Click;
            actionPanel.Controls.Add(editButton);

            Button deleteButton = new Button();
            deleteButton.Text = "🗑️ Delete";
            deleteButton.Tag = post;
            deleteButton.Size = new Size(80, 30);
            deleteButton.Location = new Point(180, 0);
            deleteButton.BackColor = Color.LightCoral;
            deleteButton.ForeColor = Color.DarkRed;
            deleteButton.FlatStyle = FlatStyle.Flat;
            deleteButton.Font = new Font("Arial", 9, FontStyle.Bold);
            deleteButton.Click += DeleteButton_Click;
            actionPanel.Controls.Add(deleteButton);

            return postPanel;
        }

        private void UpdateLikeButtonAppearance(Button likeButton, bool hasLiked)
        {
            if (hasLiked)
            {
                likeButton.Text = "❤️ Liked";
                likeButton.BackColor = Color.LightCoral;
                likeButton.ForeColor = Color.DarkRed;
                likeButton.Click -= LikeButton_Click;
                likeButton.Click += UnlikeButton_Click;
            }
            else
            {
                likeButton.Text = "🤍 Like";
                likeButton.BackColor = Color.LightGray;
                likeButton.ForeColor = Color.DarkGray;
                likeButton.Click -= UnlikeButton_Click;
                likeButton.Click += LikeButton_Click;
            }
        }

        private void LikesButton_Click(object sender, EventArgs e)
        {
            Button likesButton = sender as Button;
            if (likesButton != null && likesButton.Tag is Post post)
            {
                ShowLikesDialog(post);
            }
        }

        private void ShowLikesDialog(Post post)
        {
            Form likesForm = new Form();
            likesForm.Text = $"Likes for Post by {LoggedInUser.userName}";
            likesForm.Size = new Size(500, 400);
            likesForm.StartPosition = FormStartPosition.CenterParent;
            likesForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            likesForm.MaximizeBox = false;
            likesForm.MinimizeBox = false;

            Panel likesPanel = new Panel();
            likesPanel.Location = new Point(10, 10);
            likesPanel.Size = new Size(465, 320);
            likesPanel.BorderStyle = BorderStyle.FixedSingle;
            likesPanel.AutoScroll = true;
            likesForm.Controls.Add(likesPanel);

            CustomList<LikeDetail> likes = Program.AppData.postCrud.GetLikesForPost(post.PostID);

            if (likes.Count == 0)
            {
                Label noLikesLabel = new Label();
                noLikesLabel.Text = "No likes yet.\nBe the first to like this post!";
                noLikesLabel.Font = new Font("Arial", 11, FontStyle.Italic);
                noLikesLabel.ForeColor = Color.Gray;
                noLikesLabel.TextAlign = ContentAlignment.MiddleCenter;
                noLikesLabel.Size = new Size(465, 50);
                noLikesLabel.Location = new Point(0, 10);
                likesPanel.Controls.Add(noLikesLabel);
            }
            else
            {
                int likeY = 10;
                foreach (LikeDetail like in likes.ToList())
                {
                    Panel likePanel = CreateLikePanel(like, likeY, 445);
                    likesPanel.Controls.Add(likePanel);
                    likeY += 60;
                }
            }

            Button closeButton = new Button();
            closeButton.Text = "Close";
            closeButton.Size = new Size(80, 30);
            closeButton.Location = new Point(210, 340);
            closeButton.Click += (s, e) => likesForm.Close();
            likesForm.Controls.Add(closeButton);

            likesForm.ShowDialog();
        }

        private Panel CreateLikePanel(LikeDetail like, int yPosition, int width)
        {
            Panel likePanel = new Panel();
            likePanel.Location = new Point(10, yPosition);
            likePanel.Size = new Size(width, 50);
            likePanel.BorderStyle = BorderStyle.FixedSingle;
            likePanel.BackColor = Color.WhiteSmoke;

            PictureBox likerPic = new PictureBox();
            likerPic.Size = new Size(30, 30);
            likerPic.Location = new Point(5, 10);
            likerPic.SizeMode = PictureBoxSizeMode.Zoom;
            likerPic.BackColor = Color.LightBlue;
            likerPic.BorderStyle = BorderStyle.FixedSingle;

            if (!string.IsNullOrEmpty(like.ProfilePicture) && File.Exists(like.ProfilePicture))
            {
                likerPic.Image = Image.FromFile(like.ProfilePicture);
            }
            else
            {
                CreateInitialProfilePicture(likerPic, like.Username);
            }
            likePanel.Controls.Add(likerPic);

            Label likerName = new Label();
            likerName.Text = like.Username;
            likerName.Font = new Font("Arial", 9, FontStyle.Bold);
            likerName.Location = new Point(40, 10);
            likerName.AutoSize = true;
            likePanel.Controls.Add(likerName);

            Label likeTime = new Label();
            likeTime.Text = "• " + GetTimeAgo(like.CreatedAt);
            likeTime.Font = new Font("Arial", 8, FontStyle.Italic);
            likeTime.ForeColor = Color.DarkGray;
            likeTime.Location = new Point(likerName.Right + 5, 11);
            likeTime.AutoSize = true;
            likePanel.Controls.Add(likeTime);

            Label heartIcon = new Label();
            heartIcon.Text = "❤️";
            heartIcon.Font = new Font("Arial", 12);
            heartIcon.Location = new Point(width - 40, 15);
            heartIcon.AutoSize = true;
            likePanel.Controls.Add(heartIcon);

            return likePanel;
        }

        private void LikeButton_Click(object sender, EventArgs e)
        {
            Button likeButton = sender as Button;
            if (likeButton != null && likeButton.Tag is Post post)
            {
                if (Program.AppData.postCrud.LikePost(post.PostID, LoggedInUser.userID))
                {
                    Program.AppData.likeCrud.LoadFromDB();
                    LoadUserPosts();
                    UpdateUndoButton();
                }
            }
        }

        private void UnlikeButton_Click(object sender, EventArgs e)
        {
            Button unlikeButton = sender as Button;
            if (unlikeButton != null && unlikeButton.Tag is Post post)
            {
                if (Program.AppData.postCrud.UnlikePost(post.PostID, LoggedInUser.userID))
                {
                    Program.AppData.likeCrud.LoadFromDB();
                    LoadUserPosts();
                    UpdateUndoButton();
                }
            }
        }

        private void CommentsButton_Click(object sender, EventArgs e)
        {
            Button commentsButton = sender as Button;
            if (commentsButton != null && commentsButton.Tag is Post post)
            {
                ShowCommentsDialog(post);
            }
        }

        private void ShowCommentsDialog(Post post)
        {
            Form commentsForm = new Form();
            commentsForm.Text = $"Comments for Post by {LoggedInUser.userName}";
            commentsForm.Size = new Size(600, 500);
            commentsForm.StartPosition = FormStartPosition.CenterParent;
            commentsForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            commentsForm.MaximizeBox = false;
            commentsForm.MinimizeBox = false;

            Panel commentsPanel = new Panel();
            commentsPanel.Location = new Point(10, 10);
            commentsPanel.Size = new Size(565, 350);
            commentsPanel.BorderStyle = BorderStyle.FixedSingle;
            commentsPanel.AutoScroll = true;
            commentsForm.Controls.Add(commentsPanel);

            CustomList<Comment> comments = Program.AppData.postCrud.GetCommentsForPost(post.PostID);

            if (comments.Count == 0)
            {
                Label noCommentsLabel = new Label();
                noCommentsLabel.Text = "No comments yet.\nBe the first to comment!";
                noCommentsLabel.Font = new Font("Arial", 11, FontStyle.Italic);
                noCommentsLabel.ForeColor = Color.Gray;
                noCommentsLabel.TextAlign = ContentAlignment.MiddleCenter;
                noCommentsLabel.Size = new Size(565, 50);
                noCommentsLabel.Location = new Point(0, 10);
                commentsPanel.Controls.Add(noCommentsLabel);
            }
            else
            {
                int commentY = 10;
                foreach (Comment comment in comments.ToList())
                {
                    Panel commentPanel = CreateCommentPanel(comment, commentY, 545);
                    commentsPanel.Controls.Add(commentPanel);
                    commentY += 80;
                }
            }

            Panel addCommentPanel = new Panel();
            addCommentPanel.Location = new Point(10, 370);
            addCommentPanel.Size = new Size(565, 60);
            commentsForm.Controls.Add(addCommentPanel);

            Label addCommentLabel = new Label();
            addCommentLabel.Text = "Add a comment:";
            addCommentLabel.Font = new Font("Arial", 10, FontStyle.Bold);
            addCommentLabel.Location = new Point(0, 5);
            addCommentLabel.AutoSize = true;
            addCommentPanel.Controls.Add(addCommentLabel);

            TextBox commentTextBox = new TextBox();
            commentTextBox.Size = new Size(400, 30);
            commentTextBox.Location = new Point(0, 30);
            commentTextBox.PlaceholderText = "Write your comment here...";
            addCommentPanel.Controls.Add(commentTextBox);

            Button addCommentButton = new Button();
            addCommentButton.Text = "Add Comment";
            addCommentButton.Size = new Size(100, 30);
            addCommentButton.Location = new Point(410, 30);
            addCommentButton.BackColor = Color.LightGreen;
            addCommentButton.ForeColor = Color.DarkGreen;
            addCommentButton.Click += (s, e) =>
            {
                if (Program.AppData.postCrud.AddComment(post.PostID, LoggedInUser.userID, commentTextBox.Text))
                {
                    MessageBox.Show("Comment added successfully!", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Program.AppData.postCrud.LoadPostsFromDB();
                    Program.AppData.commentCRUD.LoadFromDB(); 
                    LoadUserPosts();
                    commentsForm.Close();
                }
            };
            addCommentPanel.Controls.Add(addCommentButton);

            Button closeButton = new Button();
            closeButton.Text = "Close";
            closeButton.Size = new Size(80, 30);
            closeButton.Location = new Point(260, 440);
            closeButton.Click += (s, e) => commentsForm.Close();
            commentsForm.Controls.Add(closeButton);

            commentsForm.ShowDialog();
        }

        private Panel CreateCommentPanel(Comment comment, int yPosition, int width)
        {
            Panel commentPanel = new Panel();
            commentPanel.Location = new Point(10, yPosition);
            commentPanel.Size = new Size(width, 70);
            commentPanel.BorderStyle = BorderStyle.FixedSingle;
            commentPanel.BackColor = Color.WhiteSmoke;

            PictureBox commenterPic = new PictureBox();
            commenterPic.Size = new Size(30, 30);
            commenterPic.Location = new Point(5, 5);
            commenterPic.SizeMode = PictureBoxSizeMode.Zoom;
            commenterPic.BackColor = Color.LightGreen;
            commenterPic.BorderStyle = BorderStyle.FixedSingle;

            if (!string.IsNullOrEmpty(comment.ProfilePicture) && File.Exists(comment.ProfilePicture))
            {
                commenterPic.Image = Image.FromFile(comment.ProfilePicture);
            }
            else
            {
                CreateInitialProfilePicture(commenterPic, comment.Username);
            }
            commentPanel.Controls.Add(commenterPic);

            Label commenterName = new Label();
            commenterName.Text = comment.Username;
            commenterName.Font = new Font("Arial", 9, FontStyle.Bold);
            commenterName.Location = new Point(40, 5);
            commenterName.AutoSize = true;
            commentPanel.Controls.Add(commenterName);

            Label commentTime = new Label();
            commentTime.Text = "• " + GetTimeAgo(comment.CreatedAt);
            commentTime.Font = new Font("Arial", 8, FontStyle.Italic);
            commentTime.ForeColor = Color.DarkGray;
            commentTime.Location = new Point(commenterName.Right + 5, 6);
            commentTime.AutoSize = true;
            commentPanel.Controls.Add(commentTime);

            TextBox commentContent = new TextBox();
            commentContent.Text = comment.CommentText;
            commentContent.Font = new Font("Arial", 9, FontStyle.Regular);
            commentContent.Location = new Point(40, 25);
            commentContent.Size = new Size(width - 50, 40);
            commentContent.Multiline = true;
            commentContent.BorderStyle = BorderStyle.None;
            commentContent.BackColor = Color.WhiteSmoke;
            commentContent.ReadOnly = true;
            commentContent.ScrollBars = ScrollBars.Vertical;
            commentPanel.Controls.Add(commentContent);

            return commentPanel;
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            Button editButton = sender as Button;
            if (editButton != null && editButton.Tag is Post post)
            {
                ShowEditDialog(post);
            }
        }

        private void ShowEditDialog(Post post)
        {
            Form editForm = new Form();
            editForm.Text = "Edit Post";
            editForm.Size = new Size(600, 400);
            editForm.StartPosition = FormStartPosition.CenterParent;
            editForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            editForm.MaximizeBox = false;
            editForm.MinimizeBox = false;

            Label contentLabel = new Label();
            contentLabel.Text = "Edit your post content:";
            contentLabel.Font = new Font("Arial", 10, FontStyle.Bold);
            contentLabel.Location = new Point(20, 20);
            contentLabel.AutoSize = true;
            editForm.Controls.Add(contentLabel);

            TextBox contentTextBox = new TextBox();
            contentTextBox.Text = post.Content;
            contentTextBox.Multiline = true;
            contentTextBox.Size = new Size(540, 200);
            contentTextBox.Location = new Point(20, 50);
            contentTextBox.ScrollBars = ScrollBars.Vertical;
            contentTextBox.Font = new Font("Arial", 10);
            editForm.Controls.Add(contentTextBox);

            Label charCountLabel = new Label();
            charCountLabel.Text = $"{post.Content.Length}/500 characters";
            charCountLabel.Location = new Point(20, 260);
            charCountLabel.AutoSize = true;
            editForm.Controls.Add(charCountLabel);

            contentTextBox.TextChanged += (s, ev) =>
            {
                int currentLength = contentTextBox.Text.Length;
                charCountLabel.Text = $"{currentLength}/500 characters";
                charCountLabel.ForeColor = currentLength > 500 ? Color.Red : Color.Black;
            };


            Button saveButton = new Button();
            saveButton.Text = "Save";
            saveButton.Size = new Size(80, 30);
            saveButton.Location = new Point(200, 300);
            saveButton.BackColor = Color.LightGreen;
            saveButton.Click += (s, ev) =>
            {
                string newContent = contentTextBox.Text.Trim();
                if (string.IsNullOrEmpty(newContent))
                {
                    MessageBox.Show("Post content cannot be empty!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (newContent.Length > 500)
                {
                    MessageBox.Show("Post content cannot exceed 500 characters!", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (Program.AppData.postCrud.UpdatePost(post.PostID, newContent))
                {
                    LoadUserPosts();
                    editForm.Close();
                }
            };
            editForm.Controls.Add(saveButton);

            Button cancelButton = new Button();
            cancelButton.Text = "Cancel";
            cancelButton.Size = new Size(80, 30);
            cancelButton.Location = new Point(300, 300);
            cancelButton.BackColor = Color.LightCoral;
            cancelButton.Click += (s, ev) => editForm.Close();
            editForm.Controls.Add(cancelButton);

            editForm.ShowDialog();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            Button deleteButton = sender as Button;
            if (deleteButton != null && deleteButton.Tag is Post post)
            {
                DialogResult result = MessageBox.Show(
                    $"Are you sure you want to delete this post?\n\n\"{post.Content}\"\n\nThis action can be undone using the Undo button.",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning
                );

                if (result == DialogResult.Yes)
                {
                    bool success = Program.AppData.postCrud.DeletePost(post.PostID);
                    if (success)
                    {
                        MessageBox.Show("Post deleted successfully!", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        LoadUserPosts();
                        UpdateUndoButton();
                    }
                }
            }
        }

        private void UndoButton_Click(object sender, EventArgs e)
        {
            bool success = Program.AppData.postCrud.UndoLastAction();
            if (success)
            {
                Program.AppData.postCrud.LoadPostsFromDB();
                Program.AppData.likeCrud.LoadFromDB();
                LoadUserPosts();
                UpdateUndoButton();
                MessageBox.Show("Action undone successfully!", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void UpdateUndoButton()
        {
            undoButton.Visible = !Program.AppData.postCrud.UndoStack.IsEmpty;
        }

        private string GetUserProfilePicture(int userId)
        {
            User user = Program.AppData.userCrud.Users.SearchByID(userId);
            return user?.ProfilePicture;
        }

        private void CreateInitialProfilePicture(PictureBox pictureBox, string username)
        {
            Bitmap profileBmp = new Bitmap(pictureBox.Width, pictureBox.Height);
            using (Graphics g = Graphics.FromImage(profileBmp))
            {
                Rectangle rect = new Rectangle(0, 0, pictureBox.Width, pictureBox.Height);
                using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                    rect, Color.LightBlue, Color.DarkBlue, 45f))
                {
                    g.FillEllipse(brush, rect);
                }

                using (Font font = new Font("Arial", 14, FontStyle.Bold))
                using (Brush textBrush = new SolidBrush(Color.White))
                {
                    string initial = username.Length > 0 ? username[0].ToString().ToUpper() : "U";
                    SizeF textSize = g.MeasureString(initial, font);
                    g.DrawString(initial, font, textBrush,
                                (pictureBox.Width - textSize.Width) / 2,
                                (pictureBox.Height - textSize.Height) / 2);
                }
            }
            pictureBox.Image = profileBmp;
        }

        private string GetTimeAgo(DateTime commentTime)
        {
            TimeSpan timeSince = DateTime.Now - commentTime;

            if (timeSince.TotalMinutes < 1) return "just now";
            if (timeSince.TotalMinutes < 60) return $"{(int)timeSince.TotalMinutes}m ago";
            if (timeSince.TotalHours < 24) return $"{(int)timeSince.TotalHours}h ago";
            if (timeSince.TotalDays < 7) return $"{(int)timeSince.TotalDays}d ago";
            return commentTime.ToString("MMM dd, yyyy");
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

       
    }
}