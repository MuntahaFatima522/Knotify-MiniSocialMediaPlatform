using System;

namespace DSA_MidProject.BL
{
    public class LikeDetail
    {
        public int LikeID { get; set; }
        public int PostID { get; set; }
        public int UserID { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Username { get; set; }
        public string ProfilePicture { get; set; }

        public LikeDetail(int likeId, int postId, int userId, DateTime createdAt, string username, string profilePicture)
        {
            LikeID = likeId;
            PostID = postId;
            UserID = userId;
            CreatedAt = createdAt;
            Username = username;
            ProfilePicture = profilePicture;
        }
    }
}