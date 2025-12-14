using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSA_MidProject.BL;

namespace DSA_MidProject.DataStructures
{
    internal class UserList
    {
        private UserNode head;

        public void Add(User u)
        {
            UserNode newNode = new UserNode(u);

            if (head == null)
            {
                head = newNode;
                return;
            }

            UserNode temp = head;
            while (temp.next != null)
                temp = temp.next;

            temp.next = newNode;
        }

        public bool Remove(string username)
        {
            if (head == null) return false;

            if (head.data.UserName == username)
            {
                head = head.next;
                return true;
            }

            UserNode temp = head;
            while (temp.next != null)
            {
                if (temp.next.data.UserName == username)
                {
                    temp.next = temp.next.next;
                    return true;
                }
                temp = temp.next;
            }
            return false;
        }

        public User Search(string username)
        {
            UserNode temp = head;
            while (temp != null)
            {
                if (temp.data.UserName == username)
                    return temp.data;
                temp = temp.next;
            }
            return null;
        }

        public User SearchByEmail(string email)
        {
            UserNode temp = head;
            while (temp != null)
            {
                if (temp.data.Email==email)
                    return temp.data;
                temp = temp.next;
            }
            return null;
        }

        public User SearchByID(int ID)
        {
            UserNode temp = head;
            while (temp != null)
            {
                if (temp.data.UserID==ID)
                    return temp.data;
                temp = temp.next;
            }
            return null;
        }

        public List<User> GetAll()
        {
            List<User> allUsers = new List<User>();
            UserNode temp = head;
            while (temp != null)
            {
                allUsers.Add(temp.data);
                temp = temp.next;
            }
            return allUsers;
        }

        public void Clear()
        {
            head = null;
        }
        public bool UserExists(int userID)
        {
            return SearchByID(userID) != null;
        }

    }
}
