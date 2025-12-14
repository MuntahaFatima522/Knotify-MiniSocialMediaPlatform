using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSA_MidProject.BL;

namespace DSA_MidProject.DataStructures
{
    internal class FriendRequestNode
    {
        public FriendRequest data;
        public FriendRequestNode next;

        public FriendRequestNode(FriendRequest data)
        {
            this.data = data;
            next = null;
        }
    }
}
