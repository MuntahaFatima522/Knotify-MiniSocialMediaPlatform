using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DSA_MidProject.BL;
using MySql.Data.MySqlClient;
using DSA_MidProject.DataStructures;

namespace DSA_MidProject.DL
{
    internal class PostCRUD
    {
        public DataStructures.Queue<Post> PostQueue { get; private set; } = new DataStructures.Queue<Post>();
        public DataStructures.Stack<PostAction> UndoStack { get; private set; } = new DataStructures.Stack<PostAction>();

        public void LoadPostsFromDB()
        {
            PostQueue.Clear();
            string query = "SELECT * FROM posts WHERE IsDeleted = 0 ORDER BY CreatedAt ASC";

            using (MySqlDataReader reader = DatabaseHelper.Instance.getData(query))
            {
                while (reader.Read())
                {
                    Post post = new Post(
                        Convert.ToInt32(reader["PostID"]),
                        Convert.ToInt32(reader["UserID"]),
                        reader["Content"].ToString(),
                        Convert.ToInt32(reader["LikeCount"]),
                        Convert.ToInt32(reader["CommentCount"]),
                        Convert.ToDateTime(reader["CreatedAt"]),
                        reader["UpdatedAt"] == DBNull.Value ? null : (DateTime?)Convert.ToDateTime(reader["UpdatedAt"]),
                        Convert.ToBoolean(reader["IsDeleted"])
                    );
                    PostQueue.Enqueue(post);
                }
            }
        }

        public bool CreatePost(int userID, string content)
        {
            if (!VerifyUserExists(userID))
            {
                MessageBox.Show($"User with ID {userID} does not exist in the system. Please make sure you are properly logged in.", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            string query = @"INSERT INTO posts (UserID, Content, LikeCount, CommentCount, CreatedAt) 
                     VALUES (@UserID, @Content, 0, 0, NOW())";

            try
            {
                var parameters = new Dictionary<string, object>
                {
                    { "@UserID", userID },
                    { "@Content", content }
                };

                int rowsAffected = DatabaseHelper.Instance.Update(query, parameters);

                if (rowsAffected == 0)
                {
                    MessageBox.Show("Failed to create post. No rows were affected.", "Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                string getLastId = "SELECT LAST_INSERT_ID()";
                int postId = Convert.ToInt32(DatabaseHelper.Instance.GetScalarValue(getLastId));

                Post newPost = new Post(postId, userID, content, 0, 0, DateTime.Now);
                PostQueue.Enqueue(newPost);

               
                return true;
            }
            catch (MySqlException mysqlEx)
            {
                if (mysqlEx.Number == 1452) 
                {
                    MessageBox.Show($"Cannot create post: User ID {userID} does not exist in the database. " +
                                  "Please make sure you are properly logged in and try again.",
                                  "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show($"Database error: {mysqlEx.Message}", "Error",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating post: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool VerifyUserExists(int userID)
        {
            if (Program.AppData.userCrud.Users.UserExists(userID))
                return true;

            string query = "SELECT COUNT(*) FROM users WHERE UserID = @UserID";
            try
            {
                var parameters = new Dictionary<string, object> { { "@UserID", userID } };
                int count = Convert.ToInt32(DatabaseHelper.Instance.GetScalarValue(query, parameters));

                if (count > 0)
                {
                    Program.AppData.userCrud.LoadFromDB();
                    return true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error verifying user: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (LoggedInUser.userID == userID)
            {
                Program.AppData.userCrud.LoadFromDB();
                return Program.AppData.userCrud.Users.UserExists(userID);
            }

            return false;
        }
        public bool UpdatePost(int postID, string newContent)
        {
            var post = GetPostByID(postID);
            if (post == null)
            {
                MessageBox.Show("Post not found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (post.UserID != LoggedInUser.userID)
            {
                MessageBox.Show("You can only edit your own posts!", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            string query = $"UPDATE posts SET Content = '{MySqlHelper.EscapeString(newContent)}', " +
                          $"UpdatedAt = NOW() WHERE PostID = {postID}";

            try
            {
                int rowsAffected = DatabaseHelper.Instance.Update(query);
                if (rowsAffected > 0)
                {
                    UpdatePostInQueue(postID, newContent);
                    MessageBox.Show("Post updated successfully!", "Success",
                                  MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating post: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool DeletePost(int postID)
        {
            var post = GetPostByID(postID);
            if (post == null || post.UserID != LoggedInUser.userID) return false;

            string query = $"UPDATE posts SET IsDeleted = 1, UpdatedAt = NOW() WHERE PostID = {postID}";

            try
            {
                DatabaseHelper.Instance.Update(query);
                RemovePostFromQueue(postID);

                UndoStack.Push(new PostAction(PostAction.ActionType.Delete, postID, LoggedInUser.userID, post));
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting post: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool UndoLastAction()
        {
            if (UndoStack.IsEmpty)
            {
                MessageBox.Show("No actions to undo!", "Info",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            var lastAction = UndoStack.Pop();

            try
            {
                switch (lastAction.Type)
                {
                    case PostAction.ActionType.Delete:
                        var deletedPost = lastAction.ActionData as Post;
                        string restoreQuery = $"UPDATE posts SET IsDeleted = 0, UpdatedAt = NOW() " +
                                            $"WHERE PostID = {lastAction.PostID}";
                        DatabaseHelper.Instance.Update(restoreQuery);
                        PostQueue.Enqueue(deletedPost);
                        break;

                    case PostAction.ActionType.Like:
                        UnlikePost(lastAction.PostID, lastAction.UserID);
                        break;

                    case PostAction.ActionType.Unlike:
                        LikePost(lastAction.PostID, lastAction.UserID);
                        break;
                }

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error undoing action: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool LikePost(int postID, int userID)
        {
            if (Program.AppData.likeCrud.HasUserLikedPost(postID, userID))
            {
                MessageBox.Show("You have already liked this post!", "Info",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            string insertLike = $"INSERT INTO likes (PostID, UserID, CreatedAt) VALUES ({postID}, {userID}, NOW())";
            string updateCount = $"UPDATE posts SET LikeCount = LikeCount + 1 WHERE PostID = {postID}";

            try
            {
                DatabaseHelper.Instance.Update(insertLike);
                DatabaseHelper.Instance.Update(updateCount);

                IncrementLikeCountInQueue(postID);
                Program.AppData.likeCrud.AddLike(postID, userID);

                UndoStack.Push(new PostAction(PostAction.ActionType.Like, postID, userID));
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error liking post: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool UnlikePost(int postID, int userID)
        {
            if (!Program.AppData.likeCrud.HasUserLikedPost(postID, userID))
            {
                MessageBox.Show("You haven't liked this post!", "Info",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            string deleteLike = $"DELETE FROM likes WHERE PostID = {postID} AND UserID = {userID}";
            string updateCount = $"UPDATE posts SET LikeCount = LikeCount - 1 WHERE PostID = {postID}";

            try
            {
                DatabaseHelper.Instance.Update(deleteLike);
                DatabaseHelper.Instance.Update(updateCount);

                DecrementLikeCountInQueue(postID);
                Program.AppData.likeCrud.RemoveLike(postID, userID);

                UndoStack.Push(new PostAction(PostAction.ActionType.Unlike, postID, userID));
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error unliking post: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public bool HasUserLikedPost(int postID, int userID)
        {
            return Program.AppData.likeCrud.HasUserLikedPost(postID, userID);
        }

        public CustomList<LikeDetail> GetLikesForPost(int postId)
        {
            return Program.AppData.likeCrud.GetLikesForPost(postId);
        }

        public CustomList<Comment> GetCommentsForPost(int postId)
        {
            return Program.AppData.commentCRUD.GetCommentsForPost(postId);
        }

        public bool AddComment(int postId, int userId, string commentText)
        {
            bool success = Program.AppData.commentCRUD.AddComment(postId, userId, commentText);
            if (success)
            {
                RefreshPostCommentCount(postId);
            }
            return success;
        }

        public int GetLikeCount(int postId)
        {
            return Program.AppData.likeCrud.GetLikeCount(postId);
        }

        private void RefreshPostCommentCount(int postId)
        {
            string query = $"SELECT CommentCount FROM posts WHERE PostID = {postId}";
            try
            {
                int newCommentCount = Convert.ToInt32(DatabaseHelper.Instance.GetScalarValue(query));
                UpdateCommentCountInQueue(postId, newCommentCount);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error refreshing comment count: {ex.Message}", "Error",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private Post GetPostByID(int postID)
        {
            var postsList = PostQueue.ToList();
            return postsList.FirstOrDefault(p => p.PostID == postID);
        }

        private void UpdatePostInQueue(int postID, string newContent)
        {
            var tempQueue = new DataStructures.Queue<Post>();
            bool found = false;

            while (!PostQueue.IsEmpty)
            {
                var post = PostQueue.Dequeue();
                if (post.PostID == postID)
                {
                    post.Content = newContent;
                    post.UpdatedAt = DateTime.Now;
                    found = true;
                }
                tempQueue.Enqueue(post);
            }

            while (!tempQueue.IsEmpty)
            {
                PostQueue.Enqueue(tempQueue.Dequeue());
            }
        }

        private void RemovePostFromQueue(int postID)
        {
            var tempQueue = new DataStructures.Queue<Post>();

            while (!PostQueue.IsEmpty)
            {
                var post = PostQueue.Dequeue();
                if (post.PostID != postID)
                {
                    tempQueue.Enqueue(post);
                }
            }

            while (!tempQueue.IsEmpty)
            {
                PostQueue.Enqueue(tempQueue.Dequeue());
            }
        }

        private void IncrementLikeCountInQueue(int postID)
        {
            var tempQueue = new DataStructures.Queue<Post>();

            while (!PostQueue.IsEmpty)
            {
                var post = PostQueue.Dequeue();
                if (post.PostID == postID)
                {
                    post.LikeCount++;
                }
                tempQueue.Enqueue(post);
            }

            while (!tempQueue.IsEmpty)
            {
                PostQueue.Enqueue(tempQueue.Dequeue());
            }
        }

        private void DecrementLikeCountInQueue(int postID)
        {
            var tempQueue = new DataStructures.Queue<Post>();

            while (!PostQueue.IsEmpty)
            {
                var post = PostQueue.Dequeue();
                if (post.PostID == postID)
                {
                    post.LikeCount = Math.Max(0, post.LikeCount - 1);
                }
                tempQueue.Enqueue(post);
            }

            while (!tempQueue.IsEmpty)
            {
                PostQueue.Enqueue(tempQueue.Dequeue());
            }
        }

        private void UpdateCommentCountInQueue(int postID, int newCount)
        {
            var tempQueue = new DataStructures.Queue<Post>();

            while (!PostQueue.IsEmpty)
            {
                var post = PostQueue.Dequeue();
                if (post.PostID == postID)
                {
                    post.CommentCount = newCount;
                }
                tempQueue.Enqueue(post);
            }

            while (!tempQueue.IsEmpty)
            {
                PostQueue.Enqueue(tempQueue.Dequeue());
            }
        }

        public List<Post> SortPostsByDate(List<Post> posts, bool ascending = true)
        {
            if (posts == null || posts.Count <= 1)
                return posts;

            var sortedPosts = new List<Post>(posts);
            QuickSortByDate(sortedPosts, 0, sortedPosts.Count - 1, ascending);
            return sortedPosts;
        }

        private void QuickSortByDate(List<Post> posts, int low, int high, bool ascending)
        {
            if (low < high)
            {
                int pivotIndex = PartitionByDate(posts, low, high, ascending);
                QuickSortByDate(posts, low, pivotIndex - 1, ascending);
                QuickSortByDate(posts, pivotIndex + 1, high, ascending);
            }
        }

        private int PartitionByDate(List<Post> posts, int low, int high, bool ascending)
        {
            DateTime pivot = posts[high].CreatedAt;
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                bool shouldSwap = ascending ?
                    posts[j].CreatedAt > pivot :
                    posts[j].CreatedAt < pivot;

                if (shouldSwap)
                {
                    i++;
                    SwapPosts(posts, i, j);
                }
            }

            SwapPosts(posts, i + 1, high);
            return i + 1;
        }

        public List<Post> SortPostsByLikes(List<Post> posts, bool ascending = false)
        {
            if (posts == null || posts.Count <= 1)
                return posts;

            var sortedPosts = new List<Post>(posts);
            QuickSortByLikes(sortedPosts, 0, sortedPosts.Count - 1, ascending);
            return sortedPosts;
        }

        private void QuickSortByLikes(List<Post> posts, int low, int high, bool ascending)
        {
            if (low < high)
            {
                int pivotIndex = PartitionByLikes(posts, low, high, ascending);
                QuickSortByLikes(posts, low, pivotIndex - 1, ascending);
                QuickSortByLikes(posts, pivotIndex + 1, high, ascending);
            }
        }

        private int PartitionByLikes(List<Post> posts, int low, int high, bool ascending)
        {
            int pivot = GetLikeCount(posts[high].PostID);
            int i = low - 1;

            for (int j = low; j < high; j++)
            {
                int currentLikes = GetLikeCount(posts[j].PostID);
                bool shouldSwap = ascending ?
                    currentLikes < pivot :
                    currentLikes > pivot;

                if (shouldSwap)
                {
                    i++;
                    SwapPosts(posts, i, j);
                }
            }

            SwapPosts(posts, i + 1, high);
            return i + 1;
        }

        private void SwapPosts(List<Post> posts, int i, int j)
        {
            var temp = posts[i];
            posts[i] = posts[j];
            posts[j] = temp;
        }

        public List<Post> GetAllPostsExceptCurrentUser(int currentUserID)
        {
            var allPosts = PostQueue.ToList();
            return allPosts.Where(p => p.UserID != currentUserID && !p.IsDeleted).ToList();
        }

        public List<Post> GetPostsByUser(int userID)
        {
            var userPosts = new List<Post>();
            var tempQueue = new DataStructures.Queue<Post>();

            while (!PostQueue.IsEmpty)
            {
                var post = PostQueue.Dequeue();
                if (post.UserID == userID && !post.IsDeleted)
                {
                    userPosts.Add(post);
                }
                tempQueue.Enqueue(post);
            }

            while (!tempQueue.IsEmpty)
            {
                PostQueue.Enqueue(tempQueue.Dequeue());
            }

            return userPosts;
        }

        public List<Post> GetAllPosts()
        {
            return PostQueue.ToList().Where(p => !p.IsDeleted).ToList();
        }
        public List<Post> SearchPosts(List<Post> posts, string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
                return posts;

            var contentResults = SearchPostsByContent(posts, searchTerm);
            var usernameResults = SearchPostsByUsername(posts, searchTerm);

            var combinedResults = contentResults.Union(usernameResults).ToList();
            return combinedResults;
        }

        public List<Post> SearchPostsByContent(List<Post> posts, string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm) || posts == null || posts.Count == 0)
                return posts;

            var results = new List<Post>();
            searchTerm = searchTerm.ToLower();

            var sortedPosts = posts.OrderBy(p => p.Content.ToLower()).ToList();

            int exactMatchIndex = BinarySearchExactContent(sortedPosts, searchTerm);
            if (exactMatchIndex != -1)
            {
                results.Add(sortedPosts[exactMatchIndex]);

                int left = exactMatchIndex - 1;
                while (left >= 0 && sortedPosts[left].Content.ToLower() == searchTerm)
                {
                    results.Add(sortedPosts[left]);
                    left--;
                }

                int right = exactMatchIndex + 1;
                while (right < sortedPosts.Count && sortedPosts[right].Content.ToLower() == searchTerm)
                {
                    results.Add(sortedPosts[right]);
                    right++;
                }
            }

            foreach (var post in posts)
            {
                if (post.Content.ToLower().Contains(searchTerm) &&
                    !results.Any(r => r.PostID == post.PostID))
                {
                    results.Add(post);
                }
            }

            return results;
        }

        private int BinarySearchExactContent(List<Post> posts, string searchTerm)
        {
            int left = 0;
            int right = posts.Count - 1;

            while (left <= right)
            {
                int mid = left + (right - left) / 2;
                string midContent = posts[mid].Content.ToLower();
                int comparison = string.Compare(midContent, searchTerm);

                if (comparison == 0)
                    return mid;
                else if (comparison < 0)
                    left = mid + 1;
                else
                    right = mid - 1;
            }

            return -1;
        }

        public List<Post> SearchPostsByUsername(List<Post> posts, string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm) || posts == null || posts.Count == 0)
                return posts;

            var results = new List<Post>();
            searchTerm = searchTerm.ToLower();

            foreach (var post in posts)
            {
                User author = Program.AppData.userCrud.Users.SearchByID(post.UserID);
                if (author != null && author.UserName.ToLower().Contains(searchTerm))
                {
                    results.Add(post);
                }
            }

            return results;
        }
    }
}