using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSA_MidProject.BL
{
    internal class PostWithUser
    {
        public Post Post { get; set; }
        public User User { get; set; }

        public PostWithUser(Post post, User user)
        {
            Post = post;
            User = user;
        }
    }
}
