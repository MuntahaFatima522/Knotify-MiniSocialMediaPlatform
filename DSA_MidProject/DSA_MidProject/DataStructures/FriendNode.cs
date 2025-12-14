using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSA_MidProject.BL;

namespace DSA_MidProject.DataStructures
{
    internal class FriendNode
    {
        public Friend data;
        public FriendNode next;

        public FriendNode(Friend data)
        {
            this.data = data;
            next = null;
        }
    }
}
