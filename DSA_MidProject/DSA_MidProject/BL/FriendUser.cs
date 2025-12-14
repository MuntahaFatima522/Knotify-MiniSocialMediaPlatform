using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSA_MidProject.BL
{
    internal class FriendUser
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string ProfilePicture { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime BecameFriendsAt { get; set; }
    }
}
