 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSA_MidProject.BL;

namespace DSA_MidProject.DataStructures
{
    internal class UserNode
    {
        public User data;
        public UserNode next;

        public UserNode(User data)
        {
            this.data = data;
            next = null;
        }
    }
}
