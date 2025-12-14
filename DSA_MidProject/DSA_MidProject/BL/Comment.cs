using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSA_MidProject.BL
{
    internal class Comment
    {
        public int CommentID { get; set; }
        public int PostID { get; set; }
        public int UserID { get; set; }
        public string CommentText { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Username { get; set; }
        public string ProfilePicture { get; set; }

        public Comment(int commentId, int postId, int userId, string commentText, DateTime createdAt, string username, string profilePicture)
        {
            CommentID = commentId;
            PostID = postId;
            UserID = userId;
            CommentText = commentText;
            CreatedAt = createdAt;
            Username = username;
            ProfilePicture = profilePicture;
        }
    }
}
