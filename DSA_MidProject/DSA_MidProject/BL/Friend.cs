using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSA_MidProject.BL
{
    internal class Friend
    {
        public int FriendshipID { get; set; }
        public int UserID1 { get; set; }
        public int UserID2 { get; set; }
        public DateTime FriendshipDate { get; set; }

        public Friend(int friendshipID, int userID1, int userID2, DateTime friendshipDate)
        {
            FriendshipID = friendshipID;
            UserID1 = userID1;
            UserID2 = userID2;
            FriendshipDate = friendshipDate;
        }
    }
}
