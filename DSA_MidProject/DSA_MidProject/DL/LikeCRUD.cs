using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DSA_MidProject.BL;
using DSA_MidProject.DataStructures;
using MySql.Data.MySqlClient;

namespace DSA_MidProject.DL
{
    internal class LikeCRUD
    {
        public static CustomList<LikeDetail> Likes { get; private set; } = new CustomList<LikeDetail>();
        private static int nextLikeId = 1;

        public void LoadFromDB()
        {
            Likes = new CustomList<LikeDetail>();
            string query = @"
                SELECT l.LikeID, l.PostID, l.UserID, l.CreatedAt, 
                       u.Username, u.ProfilePicture
                FROM likes l
                INNER JOIN users u ON l.UserID = u.UserID
                ORDER BY l.CreatedAt DESC";

            try
            {
                using (MySqlDataReader reader = DatabaseHelper.Instance.getData(query))
                {
                    while (reader.Read())
                    {
                        LikeDetail like = new LikeDetail(
                            Convert.ToInt32(reader["LikeID"]),
                            Convert.ToInt32(reader["PostID"]),
                            Convert.ToInt32(reader["UserID"]),
                            Convert.ToDateTime(reader["CreatedAt"]),
                            reader["Username"].ToString(),
                            reader["ProfilePicture"] == DBNull.Value ? null : reader["ProfilePicture"].ToString()
                        );
                        Likes.Add(like);
                    }
                }

                if (Likes.Count > 0)
                {
                    int maxId = 0;
                    var current = Likes.Head;
                    while (current != null)
                    {
                        if (current.Data.LikeID > maxId)
                            maxId = current.Data.LikeID;
                        current = current.Next;
                    }
                    nextLikeId = maxId + 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading likes: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public CustomList<LikeDetail> GetLikesForPost(int postId)
        {
            try
            {
                var postLikes = Likes.FindAll(like => like.PostID == postId);
                var tempList = postLikes.ToList();
                tempList.Sort((x, y) => y.CreatedAt.CompareTo(x.CreatedAt));

                CustomList<LikeDetail> sortedLikes = new CustomList<LikeDetail>();
                foreach (var like in tempList)
                {
                    sortedLikes.Add(like);
                }
                return sortedLikes;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading likes: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return new CustomList<LikeDetail>();
            }
        }

        public int GetLikeCount(int postId)
        {
            try
            {
                int count = Likes.CountWhere(like => like.PostID == postId);
                return count;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error getting like count: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
        }

        public bool AddLike(int postId, int userId)
        {
            try
            {
               
                string query = @"
            INSERT INTO likes (PostID, UserID, CreatedAt) 
            VALUES (@PostID, @UserID, NOW())";

                using (MySqlConnection conn = DatabaseHelper.Instance.getConnection())
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PostID", postId);
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        string getLastId = "SELECT LAST_INSERT_ID()";
                        using (MySqlCommand idCmd = new MySqlCommand(getLastId, conn))
                        {
                            int newLikeId = Convert.ToInt32(idCmd.ExecuteScalar());

                            string userQuery = $"SELECT UserName, ProfilePicture FROM users WHERE UserID = {userId}";
                            using (MySqlDataReader userReader = DatabaseHelper.Instance.getData(userQuery))
                            {
                                if (userReader.Read())
                                {
                                    string username = userReader["UserName"].ToString();
                                    string profilePicture = userReader["ProfilePicture"] == DBNull.Value ? null : userReader["ProfilePicture"].ToString();

                                    var newLike = new LikeDetail(
                                        newLikeId,  
                                        postId,
                                        userId,
                                        DateTime.Now,
                                        username,
                                        profilePicture
                                    );

                                    Likes.Add(newLike);
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
                if (mysqlEx.Number == 1062)
                {
                   
                }
                else
                {
                    MessageBox.Show($"Error adding like: {mysqlEx.Message}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding like: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool RemoveLike(int postId, int userId)
        {
            try
            {
                string checkQuery = "SELECT COUNT(*) FROM likes WHERE PostID = @PostID AND UserID = @UserID";
                using (MySqlCommand checkCmd = new MySqlCommand(checkQuery, DatabaseHelper.Instance.getConnection()))
                {
                    checkCmd.Parameters.AddWithValue("@PostID", postId);
                    checkCmd.Parameters.AddWithValue("@UserID", userId);

                    object result = checkCmd.ExecuteScalar();
                    int likeCount = result != null ? Convert.ToInt32(result) : 0;

                    if (likeCount == 0)
                    {
                        RemoveLikeFromList(postId, userId);
                        return true;
                    }
                }

                string deleteQuery = @"
                    DELETE FROM likes 
                    WHERE PostID = @PostID AND UserID = @UserID";

                using (MySqlCommand cmd = new MySqlCommand(deleteQuery, DatabaseHelper.Instance.getConnection()))
                {
                    cmd.Parameters.AddWithValue("@PostID", postId);
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        RemoveLikeFromList(postId, userId);
                        return true;
                    }
                    else
                    {
                        RemoveLikeFromList(postId, userId);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error removing like: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                RemoveLikeFromList(postId, userId);
                return false;
            }
        }

        public bool HasUserLikedPost(int postId, int userId)
        {
            try
            {
                var userLikes = Likes.FindAll(like => like.PostID == postId && like.UserID == userId);
                return userLikes.Count > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking like: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void RemoveLikeFromList(int postId, int userId)
        {
            if (Likes.Head == null) return;

            if (Likes.Head.Data.PostID == postId && Likes.Head.Data.UserID == userId)
            {
                Likes.Head = Likes.Head.Next;
                Likes.Count--;
                return;
            }

            Node<LikeDetail> current = Likes.Head;
            while (current.Next != null)
            {
                if (current.Next.Data.PostID == postId && current.Next.Data.UserID == userId)
                {
                    current.Next = current.Next.Next;
                    Likes.Count--;
                    return;
                }
                current = current.Next;
            }
        }
    }
}