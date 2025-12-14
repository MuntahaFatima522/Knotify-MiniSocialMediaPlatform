using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSA_MidProject.BL
{
    internal class Post
    {
        public int PostID { get; set; }
        public int UserID { get; set; }
        public string Content { get; set; }
        public int LikeCount { get; set; }
        public int CommentCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }


        public Post(int postID, int userID, string content,
                   int likeCount, int commentCount, DateTime createdAt,
                   DateTime? updatedAt = null, bool isDeleted = false)
        {
            PostID = postID;
            UserID = userID;
            Content = content;
            LikeCount = likeCount;
            CommentCount = commentCount;
            CreatedAt = createdAt;
            UpdatedAt = updatedAt;
            IsDeleted = isDeleted;
        }
    }
}
