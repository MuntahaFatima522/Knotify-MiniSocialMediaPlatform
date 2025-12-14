using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSA_MidProject.BL
{
    internal class User
    {
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Contact { get; set; }
        public string ProfilePicture{ get; set; }
        public DateTime CreatedAt { get; set; }

       public User(int userID, string userName, string email, string passwordHash, string contact, string profilePicture, DateTime createdAt)
        {
            UserID = userID;
            UserName = userName;
            Email = email;
            PasswordHash = passwordHash;
            Contact = contact;
            ProfilePicture = profilePicture;
            CreatedAt = createdAt;
        }
        public User(string userName, string email, string passwordHash, string contact, string profilePicture, DateTime createdAt)
        {
            UserName = userName;
            Email = email;
            PasswordHash = passwordHash;
            Contact = contact;
            ProfilePicture = profilePicture;
            CreatedAt = createdAt;
        }

        public User(int userID,string userName, string email, string contact, string profilePicture)
        {
            UserID = userID;
            UserName = userName;
            Email = email;
            Contact = contact;
            ProfilePicture = profilePicture;
        }
    }
}
