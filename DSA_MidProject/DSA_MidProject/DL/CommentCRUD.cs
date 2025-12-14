using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DSA_MidProject.BL;
using DSA_MidProject.DataStructures;
using MySql.Data.MySqlClient;

namespace DSA_MidProject.DL
{
    internal class CommentCRUD
    {
        public static CustomList<Comment> Comments { get; private set; } = new CustomList<Comment>();

        public void LoadFromDB()
        {
            Comments = new CustomList<Comment>();
            string query = @"
                SELECT c.CommentID, c.PostID, c.UserID, c.CommentText, c.CreatedAt, 
                       u.UserName, u.ProfilePicture
                FROM comments c
                INNER JOIN users u ON c.UserID = u.UserID
                ORDER BY c.CreatedAt ASC";

            try
            {
                using (MySqlDataReader reader = DatabaseHelper.Instance.getData(query))
                {
                    while (reader.Read())
                    {
                        Comment comment = new Comment(
                            Convert.ToInt32(reader["CommentID"]),
                            Convert.ToInt32(reader["PostID"]),
                            Convert.ToInt32(reader["UserID"]),
                            reader["CommentText"].ToString(),
                            Convert.ToDateTime(reader["CreatedAt"]),
                            reader["UserName"].ToString(),
                            reader["ProfilePicture"] == DBNull.Value ? null : reader["ProfilePicture"].ToString()
                        );
                        Comments.Add(comment);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading comments: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public CustomList<Comment> GetCommentsForPost(int postId)
        {
            try
            {
                var postComments = Comments.FindAll(comment => comment.PostID == postId);
                var tempList = postComments.ToList();
                tempList.Sort((x, y) => x.CreatedAt.CompareTo(y.CreatedAt));

                CustomList<Comment> sortedComments = new CustomList<Comment>();
                foreach (var comment in tempList)
                {
                    sortedComments.Add(comment);
                }
                return sortedComments;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading comments: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new CustomList<Comment>();
            }
        }

        public bool AddComment(int postId, int userId, string commentText)
        {
            if (string.IsNullOrWhiteSpace(commentText))
            {
                MessageBox.Show("Please enter a comment!", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            try
            {
                string query = @"
                    INSERT INTO comments (PostID, UserID, CommentText, CreatedAt) 
                    VALUES (@PostID, @UserID, @CommentText, NOW())";

                using (MySqlConnection conn = DatabaseHelper.Instance.getConnection())
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PostID", postId);
                    cmd.Parameters.AddWithValue("@UserID", userId);
                    cmd.Parameters.AddWithValue("@CommentText", commentText);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        string updateCommentCount = $"UPDATE posts SET CommentCount = CommentCount + 1 WHERE PostID = {postId}";
                        DatabaseHelper.Instance.Update(updateCommentCount);

                        string getLastId = "SELECT LAST_INSERT_ID()";
                        using (MySqlCommand idCmd = new MySqlCommand(getLastId, conn))
                        {
                            int newCommentId = Convert.ToInt32(idCmd.ExecuteScalar());

                            string userQuery = $"SELECT UserName, ProfilePicture FROM users WHERE UserID = {userId}";
                            using (MySqlDataReader userReader = DatabaseHelper.Instance.getData(userQuery))
                            {
                                if (userReader.Read())
                                {
                                    string username = userReader["UserName"].ToString();
                                    string profilePicture = userReader["ProfilePicture"] == DBNull.Value ? null : userReader["ProfilePicture"].ToString();

                                    var newComment = new Comment(
                                        newCommentId,  
                                        postId,
                                        userId,
                                        commentText.Trim(),
                                        DateTime.Now,
                                        username,
                                        profilePicture
                                    );

                                    Comments.Add(newComment);

                                    
                                }
                            }
                        }
                        return true;
                    }
                }
                return false;
            }
            catch (MySqlException mysqlEx)
            {
                if (mysqlEx.Number == 1452) 
                {
                    MessageBox.Show("Cannot add comment: Post or user does not exist.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show($"Database error adding comment: {mysqlEx.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding comment: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }



        public int GetCommentCount(int postId)
        {
            try
            {
                int count = Comments.CountWhere(comment => comment.PostID == postId);
                return count;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting comment count: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

    

        
       
    }
}