using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSA_MidProject.BL
{
    internal class LoggedInUser
    {
        public static int userID;
        public static string userName { get; set; }

        public static string email { get; set; }
        public static string contact { get; set; }
        public static string profilePicture{ get; set; }
        public static DateTime createdAt { get; set; }

    }
}
