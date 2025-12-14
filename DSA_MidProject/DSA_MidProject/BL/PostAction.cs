using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSA_MidProject.BL
{
    internal class PostAction
    {
        public enum ActionType { Like, Unlike, View, Delete }

        public ActionType Type { get; set; }
        public int PostID { get; set; }
        public int UserID { get; set; }
        public DateTime Timestamp { get; set; }
        public object ActionData { get; set; } 

        public PostAction(ActionType type, int postID, int userID, object actionData = null)
        {
            Type = type;
            PostID = postID;
            UserID = userID;
            Timestamp = DateTime.Now;
            ActionData = actionData;
        }
    }
}
