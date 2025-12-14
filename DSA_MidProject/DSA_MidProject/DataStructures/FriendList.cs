using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSA_MidProject.BL;

namespace DSA_MidProject.DataStructures
{
    internal class FriendList
    {
      private FriendNode head;

            public void Add(Friend friend)
            {
                FriendNode newNode = new FriendNode(friend);

                if (head == null)
                {
                    head = newNode;
                    return;
                }

                FriendNode temp = head;
                while (temp.next != null)
                    temp = temp.next;

                temp.next = newNode;
            }

            public bool Remove(int friendshipID)
            {
                if (head == null) return false;

                if (head.data.FriendshipID == friendshipID)
                {
                    head = head.next;
                    return true;
                }

                FriendNode temp = head;
                while (temp.next != null)
                {
                    if (temp.next.data.FriendshipID == friendshipID)
                    {
                        temp.next = temp.next.next;
                        return true;
                    }
                    temp = temp.next;
                }
                return false;
            }

            public Friend SearchByUsers(int userID1, int userID2)
            {
                FriendNode temp = head;
                while (temp != null)
                {
                    if ((temp.data.UserID1 == userID1 && temp.data.UserID2 == userID2) ||
                        (temp.data.UserID1 == userID2 && temp.data.UserID2 == userID1))
                        return temp.data;
                    temp = temp.next;
                }
                return null;
            }

            public List<Friend> GetFriendsOfUser(int userID)
            {
                List<Friend> userFriends = new List<Friend>();
                FriendNode temp = head;
                while (temp != null)
                {
                    if (temp.data.UserID1 == userID || temp.data.UserID2 == userID)
                        userFriends.Add(temp.data);
                    temp = temp.next;
                }
                return userFriends;
            }

           
            public void Clear()
            {
                head = null;
            }

            public bool Contains(int userID1, int userID2)
            {
                return SearchByUsers(userID1, userID2) != null;
            }
        }
    }
